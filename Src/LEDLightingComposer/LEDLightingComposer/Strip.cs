﻿/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified:    
	Class: .cs
	Class Description:
		This class holds lighting sequence structs and information about simulated led strip that allows the leds to be
        drawn to screen.  It also updates the effects when needed and calls functions to change the led color on leds
        in the list of drawable objects that are drawn to screen
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LEDLightingComposer
{
    public class Strip
    {
        //Declare global variables
        private String stripName;
        private DrawingManager drawManager;
        private List<Structs.LightingSequence> lseqs = new List<Structs.LightingSequence>();
        private bool proceed = false, init = true, forward = true, isRainbow = false, incrementBrightness = false;
        private long currentDuration = 0, // so elapsedtime - this = time within this sequence
            prevDuration = -1, //Makes sure duration isn't processed more than once for sequence
            prevSeqTimesAccumulated = 0; //As sequences change, the duration times are accumulated to here (used to find current effect's duration)
        private int pinSetup, currentSequence = 0, countSeqs = 0, i = -1, j = -1, p0 = 0, p1 = 1, 
            p2 = 2, p3 = 3, p4 = 4, p5 = 5, tail = 0, head = 3, shiftPixelBy = 0, 
            counter1 = 0, counter2 = 0, bounces = 0;
        private int[] virtualPixelArray = null;

        /*
            Lighting effects are:
            0 - AllClear
            1 - Rainbow
            2 - LoadColor
            3 - BounceBack
            4 - Flowthrough

            Colors are:
            0 - Clear
            1 - White
            2 - Red
            3 - Green
            4 - Blue
            5 - Yellow
            6 - Cyan
            7 - Magenta
            8 - Orange
        */

        /*
            Function:

            Parameters:

            Returns:
        */
        public Strip(String StripName, int PinSetup, List<Structs.LightingSequence> LightingSequences, DrawingManager DManager)
        {
            this.stripName = StripName;
            this.drawManager = DManager;
            this.pinSetup = PinSetup;
            lseqs = LightingSequences;

            if(lseqs != null && lseqs.Count > 0)
            {
                virtualPixelArray = new int[lseqs[0].totalPixels];
            }else
            {
                //Default number of virtual pixels to 100
                virtualPixelArray = new int[100];
            }
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public bool update(long currentPerformanceTime)
        {
            //Declare variables
            bool bRet = false;
            Structs.LightingSequence seq;
            long roundedDuration = 0;
            int hd = 0, tl = 0;

            //Get current duration
            currentDuration = currentPerformanceTime - prevSeqTimesAccumulated;

            //Exit if currentSequence is > total sequences (call allClear function for strip)
            if (currentSequence >= lseqs.Count)
            {
                if (!proceed)
                {
                    try
                    {
                        //Decrement current sequence by 1 for updating last sequence, then call allClear for Strip
                        currentSequence--;
                        Effects.allClear(this, drawManager);

                        //Add 1 back to current sequence so no more effects will show for this strip
                        currentSequence++;
                    }catch(Exception ex)
                    {

                    }

                    //Reset global variables
                    resetGlobalVars();

                    //Set proceed to true so we don't do this allClear section over and over again after effects has stopped
                    proceed = true;
                }
                return true;
            }

            //Set proceed to false to not initially process lighting sequence
            proceed = false;

            //Set current sequence
            seq = lseqs[currentSequence];

            ////Move to next sequence if duration is greater than current duration
            if (currentDuration >= seq.duration)
            {
                //Add current lighting sequence's duration time to prevSeqTimesAccumulated to restart currentDuration
                prevSeqTimesAccumulated += seq.duration;

                //Move to next sequence and reset current duration to 0
                currentSequence++;
                currentDuration = 0;
                prevDuration = -1;

                //Attempt to change to next sequence if possible
                try
                {
                    if (currentSequence >= lseqs.Count)
                    {
                        proceed = false;
                        return true;
                    }else
                    {
                        //Set current sequence struct
                        seq = lseqs[currentSequence];
                    }

                    //Set update to update Brightness if incrBrightness <> 0
                    incrementBrightness = seq.incrBrightness != 0 ? true : false;
                }
                catch(Exception ex)
                {

                }
            }

            //Round duration down to thousandths and compare against last duration
            roundedDuration = (long)((float)currentDuration / (float)100) * 100;
            if (prevDuration == -1)
            {
                proceed = true;
                prevDuration = roundedDuration;

                //Partially reset global variables
                partialResetGlobalVars();

                //Set update to update Brightness if incrBrightness <> 0
                incrementBrightness = seq.incrBrightness != 0 ? true : false;

                //Update brightness to current sequence struct's
                Effects.updateBrightness(this, drawManager);
            }
            else if (roundedDuration != prevDuration)
            {
                proceed = true;
                prevDuration = roundedDuration;
            }

            //Set current duration to rounded duration
            currentDuration = roundedDuration;

            try
            {
                //Update brightness if necessary
                if (incrementBrightness && (roundedDuration % seq.brightnessDelayTime) == 0 && proceed)
                {
                    Effects.updateBrightness(this, drawManager);
                }
            }catch(Exception ex)
            {

            }

            try
            {
                //If delaytime % counter is zero, then perform next peformance of lighting effect
                if ((roundedDuration % seq.delayTime) == 0 && proceed)
                {
                    switch (seq.lightsequence)
                    {
                        case Effects.FILLER:
                            Effects.allClear(this, drawManager);
                            break;
                        case Effects.ALLCLEAR:
                            Effects.allClear(this, drawManager);
                            break;
                        case Effects.RAINBOW:
                            isRainbow = true;
                            Effects.flowThrough(this, drawManager);
                            break;
                        case Effects.LOADCOLOR:
                            Effects.loadColor(this, drawManager);
                            break;
                        case Effects.BOUNCEBACK:
                            hd = getHeadofLED();
                            tl = getTailofLED();
                            Effects.bounceBack(this, drawManager, hd, tl);
                            break;
                        case Effects.FLOWTHROUGH:
                            isRainbow = false;
                            Effects.flowThrough(this, drawManager);
                            break;
                    }
                }

                bRet = true;
            }
            catch (Exception ex)
            {

            }

            //Reset proceed to false (works with exiting if current sequence is greater and this placement is important)
            proceed = false;

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public bool findCurrentSeqFromPerformanceTime(long performanceTime)
        {
            //Declare variables
            bool bRet = true, seqFound = false;
            int i = 0, timesThroughDelayTime = 0;

            //Reset global variables
            resetPerformance();

            for(i = 0; i < lseqs.Count; i++)
            {
                //Verify if current prevSeqTimesAccumulated plus this record's duration is >= currentTime
                if ((prevSeqTimesAccumulated + lseqs[i].duration) > performanceTime)
                {
                    //Set currentSequence
                    currentSequence = i;

                    //Calculate previous duration time
                    timesThroughDelayTime = (int)Math.Floor((float)(performanceTime - prevSeqTimesAccumulated) / (float)lseqs[currentSequence].delayTime);

                    //Advance lighting effect to coincide with current duration within the effect
                    advanceEffectToDuration(timesThroughDelayTime);

                    //Exit loop since we're at the correct currentSequence
                    seqFound = true;
                    break;
                }
                else
                {
                    //Add to prevSeqTimesAccumulated
                    prevSeqTimesAccumulated += lseqs[i].duration;
                }
            }

            //If performance time is greater than all lighting sequences within strip, move current sequence past last sequence
            if (!seqFound)
            {
                //Clear leds for this strip
                Effects.allClear(this, drawManager);
                currentSequence = lseqs.Count;
                //prevSeqTimesAccumulated = 0;
            }

            //Reset prevDuration to -1
            prevDuration = -1;

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        private void advanceEffectToDuration(int TimesThroughDelayTime)
        {
            //Declare variables
            int i = 0, hd = 0, tl = 0; ;

            switch (lseqs[currentSequence].lightsequence)
            {
                case Effects.FILLER:
                    Effects.allClear(this, drawManager);
                    break;
                case Effects.ALLCLEAR:
                    Effects.allClear(this, drawManager);
                    break;
                case Effects.RAINBOW:
                    //Move rainbow pixels into position
                    this.i = -1;

                    //Initialize effect
                    isRainbow = true;
                    //Effects.rainbow(this, drawManager);

                    //for (i = 0; i < TimesThroughDelayTime; i++)
                    //{
                    //    Effects.rainbow(this, drawManager);
                    //}
                    Effects.flowThrough(this, drawManager);

                    //Move flowthrough pixels into position
                    for (i = 0; i < TimesThroughDelayTime; i++)
                    {
                        Effects.flowThrough(this, drawManager);
                    }
                    break;
                case Effects.LOADCOLOR:
                    Effects.loadColor(this, drawManager);
                    break;
                case Effects.BOUNCEBACK:
                    hd = getHeadofLED();
                    tl = getTailofLED();
                    this.init = true;

                    //Initialize effect
                    Effects.bounceBack(this, drawManager, hd, tl);

                    //Move bounce back pixels into position
                    for (i = 0; i < TimesThroughDelayTime; i++)
                    {
                        Effects.bounceBack(this, drawManager, hd, tl);
                    }
                    break;
                case Effects.FLOWTHROUGH:
                    //Initialize effect
                    isRainbow = false;
                    Effects.flowThrough(this, drawManager);

                    //Move flowthrough pixels into position
                    for (i = 0; i < TimesThroughDelayTime; i++)
                    {
                        Effects.flowThrough(this, drawManager);
                    }
                    break;
            }
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        private int accountForOverShift(int PixelPosition, int TotalPixels)
        {
            //Declare variables
            float remainingShifts = 0;
            int pixelShift = PixelPosition;

            //Determine if shiftPixelBy is greater than total pixels and further calculation for current led position is needed
            if (Math.Floor(remainingShifts = ((float)PixelPosition / (float)TotalPixels)) > 1)
            {
                remainingShifts = remainingShifts % 1;
                pixelShift = (int)Math.Floor((remainingShifts * TotalPixels));
            }

            return pixelShift;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public bool resetPerformance()
        {
            //Declare variables
            bool bRet = true;
            currentSequence = 0;

            bRet = resetGlobalVars();

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        private bool resetGlobalVars()
        {
            //countSeqs = 0;
            currentDuration = 0;
            prevDuration = -1;
            prevSeqTimesAccumulated = 0;
            proceed = false;
            init = true;
            forward = true;
            shiftPixelBy = 0;
            counter1 = 0;
            counter2 = 0;
            i = -1;
            j = -1;
            p0 = -1;
            p1 = 0;
            p2 = 1;
            p3 = 2;
            p4 = 3;
            p5 = 4;
            tail = 0;
            head = 0;
            bounces = 0;

            return true;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        private bool partialResetGlobalVars()
        {
            init = true;
            forward = true;
            shiftPixelBy = 0;
            incrementBrightness = false;
            counter1 = 0;
            counter2 = 0;
            i = -1;
            j = -1;
            p0 = 0;
            p1 = 1;
            p2 = 2;
            p3 = 3;
            p4 = 4;
            p5 = 5;
            tail = 0;
            head = 0;
            bounces = 0;

            return true;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        private int getHeadofLED()
        {
            //Declare variables
            int head = -1, i = 0;

            //Loop and find the last LED whose color is not clear
            for(i = 0; i < lseqs[currentSequence].colors.Length; i++)
            {
                if(int.Parse(lseqs[currentSequence].colors[i].Split('-')[0].Trim()) != Effects.CLEAR)
                {
                    head = i;
                }
            }

            return head;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        private int getTailofLED()
        {
            //Declare variables
            int tail = -1, i = 0;

            //Loop and find the first LED whose color is not clear
            for (i = 0; i < lseqs[currentSequence].colors.Length; i++)
            {
                if (int.Parse(lseqs[currentSequence].colors[i].Split('-')[0].Trim()) != Effects.CLEAR)
                {
                    tail = i;
                    break;
                }
            }

            return tail;
        }


        #region Getters & Setters

        public List<Structs.LightingSequence> LSeqs
        {
            get { return lseqs; }
        }

        public int CurrentSequence
        {
            get { return currentSequence; }
        }

        public int PinSetup
        {
            get { return pinSetup; }
        }

        public int CountSeqs
        {
            get
            {
                return countSeqs;
            }

            set
            {
                countSeqs = value;
            }
        }

        public int I
        {
            get
            {
                return i;
            }

            set
            {
                i = value;
            }
        }

        public int J
        {
            get
            {
                return j;
            }

            set
            {
                j = value;
            }
        }

        public int P0
        {
            get
            {
                return p0;
            }

            set
            {
                p0 = value;
            }
        }

        public int P1
        {
            get
            {
                return p1;
            }

            set
            {
                p1 = value;
            }
        }

        public int P2
        {
            get
            {
                return p2;
            }

            set
            {
                p2 = value;
            }
        }

        public int P3
        {
            get
            {
                return p3;
            }

            set
            {
                p3 = value;
            }
        }

        public int P4
        {
            get
            {
                return p4;
            }

            set
            {
                p4 = value;
            }
        }

        public int P5
        {
            get
            {
                return p5;
            }

            set
            {
                p5 = value;
            }
        }

        public int Tail
        {
            get
            {
                return tail;
            }

            set
            {
                tail = value;
            }
        }

        public int Head
        {
            get
            {
                return head;
            }

            set
            {
                head = value;
            }
        }

        public int Counter1
        {
            get
            {
                return counter1;
            }

            set
            {
                counter1 = value;
            }
        }

        public int Counter2
        {
            get
            {
                return counter2;
            }

            set
            {
                counter2 = value;
            }
        }

        public int Bounces
        {
            get
            {
                return bounces;
            }

            set
            {
                bounces = value;
            }
        }

        public int ShiftPixelBy
        {
            get
            {
                return shiftPixelBy;
            }

            set
            {
                shiftPixelBy = value;
            }
        }

        public bool Proceed
        {
            get
            {
                return proceed;
            }

            set
            {
                proceed = value;
            }
        }

        public bool Init
        {
            get
            {
                return init;
            }

            set
            {
                init = value;
            }
        }

        public bool Forward
        {
            get
            {
                return forward;
            }

            set
            {
                forward = value;
            }
        }

        public bool IsRainbow
        {
            get
            {
                return isRainbow;
            }
            set
            {
                isRainbow = value;
            }
        }

        public bool IncrementBrightness
        {
            get { return incrementBrightness; }
        }

        public int[] VirtualPixelArray
        {
            get
            {
                return virtualPixelArray;
            }

            set
            {
                virtualPixelArray = value;
            }
        }


        #endregion Getters & Setters
    }
}
