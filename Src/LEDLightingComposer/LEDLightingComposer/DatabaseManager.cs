﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace LEDLightingComposer
{
    public class DatabaseManager
    {
        //Declare global variables
        private LEDLightingComposerCS llc;
        private Button add2Project, openProject, editRecord, send2MCUViaHTTP, send2SDCard;
        private DataGridView projectGrid;
        private Label projectName;
        private readonly String MCU_PINS = "MCU_PINS", MCU_PINS_OPTION1 = "PROJECTMCU", LED_PROJECT = "LED_PROJECT",
            LIGHTING_EFFECTS = "LIGHTING_EFFECTS", MCU = "MCU";
        MySqlConnection con, con2, con3;
        private String connString = "Server=localhost;Database=test;Uid=root;Pwd=user1234";

        #region Public Methods

        public DatabaseManager(LEDLightingComposerCS LLC, Label ProjectName, Button Add2Project, Button EditRecord, Button Send2MCUViaHTTP, Button Send2SDCard, Button OpenProject, DataGridView ProjectGrid)
        {
            //Set global variables to passed variables
            this.llc = LLC;
            this.projectName = ProjectName;
            this.add2Project = Add2Project;
            this.editRecord = EditRecord;
            this.send2MCUViaHTTP = Send2MCUViaHTTP;
            this.send2SDCard = Send2SDCard;
            this.openProject = OpenProject;
            this.projectGrid = ProjectGrid;
        }

        /*
           Function loadCBoxByType:

        */
        public void loadCBoxByType(String Type, ComboBox CBox, String Value1)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;

            //Open db connection
            OpenDBConnection();

            try
            {
                cmd = con.CreateCommand();

                switch (Type)
                {
                    case "PROJECTNAMES":
                        cmd.CommandText = "Select Project_Name, Description from LED_Project";
                        break;
                    case "MCUNAMES":
                        cmd.CommandText = "Select MCU_Name, Description from MCU";
                        break;
                    case "MCUPINS":
                        cmd.CommandText = "Select Data_Pin, Clock_Pin, Description from MCU_Pins where MCU_Name = @MName";
                        cmd.Parameters.AddWithValue("@MName", Value1);
                        break;
                    case "LIGHTINGEFFECTS":
                        cmd.CommandText = "Select Lighting_Effect, Description from Lighting_Effects where Lighting_Effect <> -1";
                        break;
                    case "MCU":
                        cmd.CommandText = "Select MCU_Name, Description from MCU order by MCU_Name";
                        break;
                }
                
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (Type.Equals("MCUPINS"))
                    {
                        CBox.Items.Add(rdr.GetString(0) + ";" + rdr.GetString(1) + ";" + rdr.GetString(2));
                    }
                    else
                    {
                        CBox.Items.Add(rdr.GetString(0) + " - " + rdr.GetString(1));
                    }
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading " + Type + " to combobox: " + ex.Message);
            }

            //Close db connection
            CloseDBConnection();
        }

        /*
        */
        public void loadAllProjects2SearchGrid(DataGridView dgv)
        {
            //Declare variables
            MySqlCommand cmd = null;
            MySqlDataAdapter adap = null;
            DataSet ds = null;

            try
            {
                //Open db connection
                OpenDBConnection();

                cmd = con.CreateCommand();
                cmd.CommandText = "Select * from LED_Project";
                adap = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adap.Fill(ds);
                dgv.DataSource = ds.Tables[0].DefaultView;

                //Close db connection
                CloseDBConnection();
            }catch(Exception ex)
            {
                MessageBox.Show("Error loading all projects to search browser...: " + ex.Message);
            }
        }

        /*
        */
        public int loadProjects2ProjectGrid(String ProjectName, DataGridView dgv)
        {
            //Declare variables
            MySqlCommand cmd = null;
            MySqlDataAdapter adap = null;
            DataSet ds = null;
            int iret = -1;

            try
            {
                //Add fillers to database before loading project to grid
                addFillerEffectsToProject(ProjectName);

                //Open db connection
                OpenDBConnection();

                cmd = con.CreateCommand();
                cmd.CommandText = "Select LE.EFFECT_NUM, LE.PROJECT_NAME, LP.Description as PROJECT_DESC, LE.MCU_NAME, M.Description as MCU_DESC, LE.PIN_SETUP, MP.DATA_PIN, MP.CLOCK_PIN, " +
                    "LE.NUM_LEDS, LE.LIGHTING_EFFECT, LES.DESCRIPTION, LE.EFFECT_START, LE.EFFECT_DURATION, (LE.Effect_Start + LE.Effect_Duration) as ENDOFEFFECT, LE.DELAY_TIME, LE.LED_POSITION_ARRAY, LE.LED_COLOR_ARRAY, LE.ITERATIONS, LE.BOUNCES, " +
                    "LE.BRIGHTNESS, LE.INCR_BRIGHTNESS, LE.BRIGHTNESS_DELAYTIME from Led_Effect LE, MCU_Pins MP, Lighting_Effects LES, MCU M, LED_Project LP where MP.PIN_SETUP = LE.PIN_SETUP and LES.Lighting_Effect = LE.Lighting_Effect " +
                    "and M.MCU_NAME = LE.MCU_NAME and LP.Project_Name = LE.Project_Name and LE.Project_Name = @PName order by LE.Pin_Setup, LE.Effect_Start";
                cmd.Parameters.AddWithValue("@PName", ProjectName);
                adap = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adap.Fill(ds);
                dgv.DataSource = ds.Tables[0].DefaultView;

                //Set Project Label
                this.projectName.Text = ProjectName;
                this.projectName.Update();

                iret = 1;

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading " + ProjectName +  " project to Project Grid...: " + ex.Message);
            }

            return iret;
        }

        /*
        */
        public int addFillerEffectsToProject(String ProjectName)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            List<int> pinSetups = new List<int>();
            int iCount = 0;
            double lastEffectEnd = 0, effectStart = 0;

            try
            {
                //Add fillers to database before loading project to grid
                deleteFillerEffectsFromProject(ProjectName);

                //Get list of distinct pin setups
                pinSetups = getDistinctPinSetups(ProjectName);

                //Open db connection
                OpenDBConnection2();

                //Loop through records for distinct pins
                foreach (int pin in pinSetups)
                {
                    //Reset variables
                    iCount = 0;
                    lastEffectEnd = 0;

                    cmd = con2.CreateCommand();
                    cmd.CommandText = "Select PROJECT_NAME, MCU_NAME, PIN_SETUP, NUM_LEDS, LIGHTING_EFFECT, EFFECT_START, " + 
                        "EFFECT_DURATION, (Effect_Start + Effect_Duration) as ENDOFEFFECT, DELAY_TIME, LED_POSITION_ARRAY, " +
                        "LED_COLOR_ARRAY, ITERATIONS, BOUNCES, BRIGHTNESS, INCR_BRIGHTNESS, BRIGHTNESS_DELAYTIME " + 
                        "from LED_Effect where Project_Name = @PName and Pin_Setup = @PinSetup order by Effect_Start";
                    cmd.Parameters.AddWithValue("@PName", ProjectName);
                    cmd.Parameters.AddWithValue("@PinSetup", pin);

                    rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        //Conjure effectStart
                        effectStart = Convert.ToDouble(rdr["EFFECT_START"].ToString());

                        //If this effect's effect_start does not equal last effects end, create a filler record from last effect's end to this effect's start
                        if (effectStart != lastEffectEnd)
                        {
                            insertRecordIntoDBReturnIncr("LED_EFFECT", rdr["PROJECT_NAME"].ToString(), rdr["MCU_NAME"].ToString(),
                                rdr["PIN_SETUP"].ToString(), rdr["NUM_LEDS"].ToString(),"-1", lastEffectEnd.ToString(), 
                                (effectStart - lastEffectEnd).ToString(), rdr["LED_POSITION_ARRAY"].ToString(), rdr["LED_COLOR_ARRAY"].ToString(), 
                                rdr["DELAY_TIME"].ToString(), rdr["ITERATIONS"].ToString(), rdr["BOUNCES"].ToString(), rdr["BRIGHTNESS"].ToString(),
                                rdr["INCR_BRIGHTNESS"].ToString(), rdr["BRIGHTNESS_DELAYTIME"].ToString());

                            //Increment iCount to keep record of how many fillers were created...
                            iCount++;
                        }

                        //Set lastEffectEnd to this effect's effect_start + effect_duration
                        lastEffectEnd = Convert.ToDouble(rdr["ENDOFEFFECT"].ToString());
                    }
                    rdr.Close();
                }

                //Close db connection
                CloseDBConnection2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating filler effects: " + ex.Message);
            }

            return iCount;
        }

        /*
        */
        public int deleteFillerEffectsFromProject(String ProjectName)
        {
            //Declare variables
            MySqlCommand cmd;
            int iret = -1;

            try {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                cmd.CommandText = "Delete from LED_Effect where Project_Name = @PName and Lighting_Effect = -1";
                cmd.Parameters.AddWithValue("@PName", ProjectName);

                iret = cmd.ExecuteNonQuery();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting filler effect records from LED_Effect table: " + ex.Message);

            }

            return iret;
        }

        /*
        */
        public bool updateProjectsInProjectGrid(String ProjectName)
        {
            //Declare variables
            bool bRet = false;

            try
            {
                //Update datagridview
                loadProjects2ProjectGrid(ProjectName, projectGrid);

                //Update Effect Manager's strips
                EffectsManager.replaceStrips(projectGrid);

                //Clear drawing manager's led strip array
                llc.DrawManager.DrawableObjects.Clear();

                //Invalidate screen so leds will be redrawn
                llc.Invalidate();

                //Load led strips and effects into drawing manager
                loadLEDStripEffectsIntoDrawingManager(projectGrid, llc.DrawManager, llc.getDrawingBottom(), llc.getDrawingRight());

                //Set Project Label
                this.projectName.Text = ProjectName;
                this.projectName.Update();

                bRet = true;
            }catch(Exception ex)
            {

            }

            return bRet;
        }

        /*
        */
        public int loadLEDStripEffectsIntoDrawingManager(DataGridView dgv, DrawingManager dmanager, int bottom, int right)
        {
            int pinSetup, iret = 0, tp = 0, numLeds = 0;
            String[] ledColorArray = { "" }, ledPositionArray = { "" };
            bool skip = false, add = true;

            try
            {
                //Add all led strips from grid into drawing manager
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    iret += 1;

                    //Set pin setup
                    pinSetup = int.Parse(row.Cells["PIN_SETUP"].Value.ToString().Trim());

                    //If the Drawing Manager's led strip is empty, then start top - left at 20-20, otherwise start at last led strips' last led top + 30
                    if (dmanager.DrawableObjects.Count < 1)
                    {
                        //Start at top 20, left 20
                        tp = 20;
                    }
                    else
                    {
                        //Start at top == last strips' led top + 30, left 20
                        tp = dmanager.DrawableObjects[(dmanager.DrawableObjects.Count - 1)].Leds[(dmanager.DrawableObjects[(dmanager.DrawableObjects.Count - 1)].Leds.Count - 1)].Top + 30;
                    }

                    //Add LEDs to strip if strip not already added
                    skip = false;
                    foreach (DrawableObject dbo in dmanager.DrawableObjects)
                    {
                        if (dbo.PinSetup == pinSetup)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (!skip)
                    {
                        //Get needed values
                        numLeds = int.Parse(row.Cells["NUM_LEDS"].Value.ToString().Trim());
                        ledColorArray = row.Cells["LED_COLOR_ARRAY"].Value.ToString().Trim().Split(',');
                        ledPositionArray = row.Cells["LED_POSITION_ARRAY"].Value.ToString().Trim().Split(',');

                        //Don't add to screen if object will be outside of designated window area
                        if (tp > llc.getDrawingBottom())
                        {
                            add = false;
                        }else
                        {
                            add = true;
                        }

                        //Add drawable object to drawings managers list of objects to draw
                        dmanager.DrawableObjects.Add(new DrawableObject("LINE", pinSetup, numLeds, ledColorArray, ledPositionArray, tp, 20, bottom, right, 0, add));
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Error in DatabaseManager - loadLEDStripEffectsIntoDrawingManager: " + ex.Message);
            }
            return iret;
        }

        /*
        */
        public bool verifyNameExistsInDatabase(String Table, String Option1, String Value1, String Value2, String Value3)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            bool bret = false;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                switch (Table)
                {
                    case "LED_PROJECT":
                        cmd.CommandText = "Select Project_Name from LED_Project where Project_Name = @Name";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        break;
                    case "LIGHTING_EFFECTS":
                        cmd.CommandText = "Select Lighting_Effect from Lighting_Effects where Lighting_Effect = @Name";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        break;
                    case "MCU":
                        cmd.CommandText = "Select MCU_Name from MCU where MCU_Name = @MName and Project_Name = @PName";
                        cmd.Parameters.AddWithValue("@MName", Value1);
                        cmd.Parameters.AddWithValue("@PName", Value2);
                        break;
                    case "MCU_PINS":
                        if (Option1.Equals(MCU_PINS_OPTION1))
                        {
                            cmd.CommandText = "Select Pin_Setup from MCU_Pins where MCU_Name = @CName";
                            cmd.Parameters.AddWithValue("@CName", Value1);
                        }
                        else
                        {
                            cmd.CommandText = "Select Pin_Setup from MCU_Pins where MCU_Name = @CName and Data_Pin = @DPin and Clock_Pin = @CPin";
                            cmd.Parameters.AddWithValue("@CName", Value1);
                            cmd.Parameters.AddWithValue("@DPin", int.Parse(Value2));
                            cmd.Parameters.AddWithValue("@CPin", int.Parse(Value3));
                        }
                        break;
                    default:
                        return false;
                }

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    bret = true;
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error verifying name exists in table " + Table + ": " + ex.Message);

            }

            return bret;
        }

        /*
        */
        public bool verifyOverlappingLightingEffects(String ProjectName, int TruePinSetupVal, float EffectStart, float EffectDuration)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            bool bret = false;
            float endOfEffect = EffectStart + EffectDuration, iret = 0;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                cmd.CommandText = "Select Effect_Start, (Effect_Start + Effect_Duration) as EndOfEffect from LED_Effect where Project_Name = @Name "+
                    "and Pin_Setup = @PSetup and Effect_Start between @BegEffect and @EndEffect and Effect_Start <> @EndEffect and Lighting_Effect <> -1 "+
                    "or Project_Name = @Name and Pin_Setup = @PSetup and (Effect_Start + Effect_Duration) between @BegEffect and @EndEffect " +
                    "and (Effect_Start + Effect_Duration) <> @BegEffect and Lighting_Effect <> -1";
                cmd.Parameters.AddWithValue("@Name", ProjectName);
                cmd.Parameters.AddWithValue("@PSetup", TruePinSetupVal);
                cmd.Parameters.AddWithValue("@BegEffect", EffectStart);
                cmd.Parameters.AddWithValue("@EndEffect", endOfEffect);

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    iret = rdr.GetInt32(0);
                    bret = true;
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error verifying if overlapping exists: " + ex.Message);

            }

            return bret;
        }

        /*
        */
        public int insertRecordIntoDB(String Table, String Value1, String Value2, String Value3, String Value4)
        {
            //Declare variables
            MySqlCommand cmd;
            int iret = 0;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                switch (Table)
                {
                    case "LED_PROJECT":
                        cmd.CommandText = "Insert into LED_Project (Project_Name, Description) Values(@Name, @Desc)";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        cmd.Parameters.AddWithValue("@Desc", Value2);
                        break;
                    case "LIGHTING_EFFECTS":
                        cmd.CommandText = "Insert into Lighting_Effects Values(@Name, @Desc)";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        cmd.Parameters.AddWithValue("@Desc", Value2);
                        break;
                    case "MCU":
                        cmd.CommandText = "Insert into MCU Values(@Name, @Desc, @PName)";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        cmd.Parameters.AddWithValue("@Desc", Value2);
                        cmd.Parameters.AddWithValue("@PName", Value3);
                        break;
                    default:
                        return 0;
                }

                iret = cmd.ExecuteNonQuery();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting record into table " + Table + ": " + ex.Message);

            }

            return iret;
        }

        /*
           Function updateRecordInDB:

           Param1 = Effect_Num
           Value1 = Num_Leds, Value2 = LED_Position_Array, Value3 = LED_Color_Array, Value4 = Lighting_Effect,
           Value5 = Effect_Start, Value6 = Effect_Duration
        */
        public int updateRecordInDB(String Table, String Param1, String Value1, String Value2, String Value3, String Value4, String Value5, String Value6, String Value7, String Value8, String Value9, String Value10, String Value11)
        {
            //Declare variables
            MySqlCommand cmd;
            int iret = 0;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                switch (Table)
                {
                    case "LED_EFFECT":
                        cmd.CommandText = "Update LED_Effect Set LED_Position_Array = @LPA, LED_Color_Array = @LCA, "+
                            "Lighting_Effect = @LEffect, Effect_Start = @EStart, Effect_Duration = @EDuration, Delay_Time = @DTime, " +
                            "Iterations = @Iterations, Bounces = @Bounces, Brightness = @Brightness, Incr_Brightness = @IncrBrightness, " +
                            "Brightness_DelayTime = @BrightnessDT where Effect_Num = @ENum";
                        cmd.Parameters.AddWithValue("@ENum", int.Parse(Param1));
                        cmd.Parameters.AddWithValue("@LPA", Value1);
                        cmd.Parameters.AddWithValue("@LCA", Value2);
                        cmd.Parameters.AddWithValue("@LEffect", int.Parse(Value3));
                        cmd.Parameters.AddWithValue("@EStart", float.Parse(Value4));
                        cmd.Parameters.AddWithValue("@EDuration", float.Parse(Value5));
                        cmd.Parameters.AddWithValue("@DTime", float.Parse(Value6));
                        cmd.Parameters.AddWithValue("@Iterations", int.Parse(Value7));
                        cmd.Parameters.AddWithValue("@Bounces", int.Parse(Value8));
                        cmd.Parameters.AddWithValue("@Brightness", int.Parse(Value9));
                        cmd.Parameters.AddWithValue("@IncrBrightness", int.Parse(Value10));
                        cmd.Parameters.AddWithValue("@BrightnessDT", float.Parse(Value11));
                        break;
                    case "LED_PROJECT":

                        break;
                    case "LIGHTING_EFFECTS":

                        break;
                    case "MCU":

                        break;
                    default:
                        return 0;
                }

                iret = cmd.ExecuteNonQuery();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating record in table " + Table + ": " + ex.Message);

            }

            return iret;
        }

        /*
        */
        public int deleteRecordFromDB(String Table, String Param1)
        {
            //Declare variables
            MySqlCommand cmd;
            int iret = 0;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                switch (Table)
                {
                    case "LED_EFFECT":
                        cmd.CommandText = "Delete from LED_Effect where Effect_Num = @ENum";
                        cmd.Parameters.AddWithValue("@ENum", int.Parse(Param1));
                        break;
                    case "LED_PROJECT":
                        
                        break;
                    case "LIGHTING_EFFECTS":
                        
                        break;
                    case "MCU":
                        
                        break;
                    default:
                        return 0;
                }

                iret = cmd.ExecuteNonQuery();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting record from table " + Table + ": " + ex.Message);

            }

            return iret;
        }

        /*
        */
        public int insertRecordIntoDBReturnIncr(String Table, String Value1, String Value2, String Value3, String Value4, String Value5, String Value6, String Value7, String Value8, String Value9, String Value10, String Value11, String Value12, String Value13, String Value14, String Value15)
        {
            //Declare variables
            MySqlCommand cmd;
            int iret = -1;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                switch (Table)
                {
                    case "LED_EFFECT":
                        //Invoke default constraints on values for certain lighting effects
                        switch (int.Parse(Value5))
                        {
                            case Effects.ALLCLEAR:
                                //Delay time should equal duration time so no unnecessary updating is done
                                Value10 = Value7;
                                break;
                            case Effects.RAINBOW:
                                //Default iterations if necessary
                                if (int.Parse(Value11) == 0)
                                {
                                    Value11 = "1";
                                }
                                break;
                            case Effects.LOADCOLOR:
                                //Delay time should equal duration time so no unnecessary updating is done
                                Value10 = Value7;
                                break;
                            case Effects.BOUNCEBACK:
                                //Default iterations if necessary
                                if (int.Parse(Value11) == 0)
                                {
                                    Value11 = "1";
                                }

                                //Default bounces if necessary
                                if (int.Parse(Value12) == 0)
                                {
                                    Value12 = "2";
                                }
                                break;
                            case Effects.FLOWTHROUGH:
                                //Default iterations if necessary
                                if (int.Parse(Value11) == 0)
                                {
                                    Value11 = "1";
                                }
                                break;
                        }
                        cmd.CommandText = "Insert into LED_Effect (Project_Name, MCU_Name, Pin_Setup, Num_LEDs, Lighting_Effect, " + 
                            "Effect_Start, Effect_Duration, Delay_Time, LED_Position_Array, LED_Color_Array, Iterations, Bounces) " + 
                            "Values(@PName, @CName, @PSetup, @NumLEDs, @LEffect, @EffectStart, @EffectDuration, @DTime, @LEDPArray, " +
                            "@LEDCArray, @Iterations, @Bounces)";
                        cmd.Parameters.AddWithValue("@PName", Value1);
                        cmd.Parameters.AddWithValue("@CName", Value2);
                        cmd.Parameters.AddWithValue("@PSetup", int.Parse(Value3));
                        cmd.Parameters.AddWithValue("@NumLEDs", int.Parse(Value4));
                        cmd.Parameters.AddWithValue("@LEffect", int.Parse(Value5));
                        cmd.Parameters.AddWithValue("@EffectStart", float.Parse(Value6));
                        cmd.Parameters.AddWithValue("@EffectDuration", float.Parse(Value7));
                        cmd.Parameters.AddWithValue("@LEDPArray", Value8);
                        cmd.Parameters.AddWithValue("@LEDCArray", Value9);
                        cmd.Parameters.AddWithValue("@DTime", float.Parse(Value10));
                        cmd.Parameters.AddWithValue("@Iterations", int.Parse(Value11));
                        cmd.Parameters.AddWithValue("@Bounces", int.Parse(Value12));
                        break;
                    case "MCU_PINS":
                        cmd.CommandText = "Insert into MCU_Pins (Description, MCU_Name, Data_Pin, Clock_Pin) Values(@Desc, @MName, @DPin, @CPin)";
                        cmd.Parameters.AddWithValue("@Desc", Value1);
                        cmd.Parameters.AddWithValue("@MName", Value2);
                        cmd.Parameters.AddWithValue("@DPin", int.Parse(Value3));
                        cmd.Parameters.AddWithValue("@CPin", int.Parse(Value4));
                        break;
                    default:
                        return -1;
                }

                iret = cmd.ExecuteNonQuery();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error verifying name exists in table " + Table + ": " + ex.Message);

            }

            return iret;
        }

        /*
        */
        public void writeProjectFromDB2StructsLocalFile(String ProjectName, FileStream SetupFile)
        {
            //Declare variables
            StreamWriter setupFileWrite = new StreamWriter(SetupFile);
            List<String> mcuNames = null, pinSetups = null, lEffects = null, pins = null;
            String[] lSeq = { "" }, dataClockPins = { "" };
            String stemp = "";
            int stripCount = 0, i = 0, j = 0, iret = 0;

            try
            {
                //Get distinct mcu names
                mcuNames = getMultipleStringValuesFromDB("LED_EFFECT", "MCUS", ProjectName, null);

                //Loop through distinct mcu names (which represent separate microcontrollers)
                foreach (String mcuName in mcuNames)
                {

                    //Write header for allocating lighting effect memory section
                    setupFileWrite.Write("//Allocate Lighting Effect Memory for MCU: " + mcuName + "**************************************************************************************" + System.Environment.NewLine + System.Environment.NewLine);

                    //Write first part of memory allocation section to file for effect variables, effects manager, strips, and lighting sequences
                    setupFileWrite.Write("//Effects specific variables" + System.Environment.NewLine
                        + "unsigned long localElapsedTime, temp;" + System.Environment.NewLine + "int8_t ");

                    //Get distinct pin setups
                    pinSetups = getMultipleStringValuesFromDB("LED_EFFECT", "PINSETUPS", ProjectName, mcuName);

                    //Get number of strips
                    stripCount = pinSetups.Count;

                    //Write number of strips to file
                    setupFileWrite.Write(" numStrips = " + stripCount.ToString() + ", ");

                    //Loop for each distinct pin setup
                    for (i = 0; i < pinSetups.Count; i++)
                    {
                        //Get number of lighting effects
                        if ((stemp = getValueFromDB("LED_EFFECT", "NUMLEFFECTS", ProjectName, getValueFromDB("MCU_PINS", "MCUNAME", pinSetups[i], null, null), pinSetups[i])).Equals("") || stemp.Equals("0")) {/*This pin setup will be skipped (this should never happen as we're only looping through information that is in the database...)*/}
                        else
                        {
                            int.TryParse(stemp, out iret);
                        }

                        //Write numEffects variable
                        if (i == (pinSetups.Count - 1))
                        {
                            setupFileWrite.Write("numEffects" + (i + 1) + " = " + iret + ";" + System.Environment.NewLine);
                        }
                        else
                        {
                            setupFileWrite.Write("numEffects" + (i + 1) + " = " + iret + ", ");
                        }
                    }
                    setupFileWrite.Flush();

                    //Write beginning of numPixels line
                    setupFileWrite.Write("uint16_t ");

                    //Loop for each distinct pin setup
                    for (i = 0; i < pinSetups.Count; i++)
                    {
                        //Get number of LEDs
                        if ((stemp = getValueFromDB("LED_EFFECT", "DISTINCTNUMLEDS", ProjectName, getValueFromDB("MCU_PINS", "MCUNAME", pinSetups[i], null, null), pinSetups[i])).Equals("") || stemp.Equals("0")) {/*This pin setup will be skipped (this should never happen as we're only looping through information that is in the database...)*/}
                        else
                        {
                            int.TryParse(stemp, out iret);
                        }

                        //Write numPixels variable
                        if (i == (pinSetups.Count - 1))
                        {
                            setupFileWrite.Write("numPixels" + (i + 1) + " = " + iret + ";" + System.Environment.NewLine + System.Environment.NewLine);
                        }
                        else
                        {
                            setupFileWrite.Write("numPixels" + (i + 1) + " = " + iret + ", ");
                        }
                    }
                    setupFileWrite.Flush();

                    //Write last part of memory allocation section to file for effect variables, effects manager, strips, and lighting sequences
                    setupFileWrite.Write("//Allocate memory for effects manager and steup" + System.Environment.NewLine +
                        "EffectsManager effectsManager;" + System.Environment.NewLine +
                        "EffectsManagerUpdateReturn *uRet = (EffectsManagerUpdateReturn*)calloc(1, sizeof(EffectsManagerUpdateReturn));" + System.Environment.NewLine + System.Environment.NewLine);
                    setupFileWrite.Write("//Allocate memory for strips" + System.Environment.NewLine +
                        "Strip *strips = (Strip*)calloc(numStrips, sizeof(Strip));" + System.Environment.NewLine + System.Environment.NewLine);

                    //Start writing strip memory allocation section
                    setupFileWrite.Write("//Allocation memory for Lighting Sequences" + System.Environment.NewLine);

                    //Write last part of strip memory allocation section
                    for (i = 0; i < stripCount; i++)
                    {
                        if (i == (stripCount - 1))
                        {
                            setupFileWrite.Write("LightingSequence* seqs" + (i + 1) + " = (LightingSequence*)calloc(numEffects" + (i + 1) + ", sizeof(LightingSequence));" + System.Environment.NewLine + System.Environment.NewLine);
                        }
                        else
                        {
                            setupFileWrite.Write("LightingSequence* seqs" + (i + 1) + " = (LightingSequence*)calloc(numEffects" + (i + 1) + ", sizeof(LightingSequence));" + System.Environment.NewLine);
                        }
                    }
                    setupFileWrite.Write("//End of Allocate Lighting Effect Memory for MCU: " + mcuName + "**************************************************************************************" + System.Environment.NewLine + System.Environment.NewLine);
                    setupFileWrite.Flush();

                    //Write header for initializing lighting effects section
                    setupFileWrite.Write("//Initialize Lighting Effects for MCU: " + mcuName + "*****************************************************************************************" + System.Environment.NewLine + System.Environment.NewLine);

                    //Loop through pin setups and gather lighting effects
                    for (i = 0; i < pinSetups.Count; i++)
                    {
                        //Get all lighting effects for strip
                        lEffects = getMultipleStringValuesFromDB("LED_EFFECT", "LEFFECTS", ProjectName, pinSetups[i]);

                        //Loop all effects
                        for (j = 0; j < lEffects.Count; j++)
                        {
                            //Split lighting effects variables
                            lSeq = lEffects[j].Split(';');

                            //Write lighting effects initialization
                            setupFileWrite.Write(createStructInfo("LEFFECT", j.ToString(), lSeq[0], "numPixels", lSeq[1], lSeq[2], lSeq[3], lSeq[4], lSeq[5], lSeq[6], lSeq[7], lSeq[8], (i + 1).ToString()) + System.Environment.NewLine);
                        }
                        setupFileWrite.Write(System.Environment.NewLine);
                    }
                    setupFileWrite.Write("//End of Initialize Lighting Effects for MCU: " + mcuName + "*****************************************************************************************" + System.Environment.NewLine + System.Environment.NewLine);
                    setupFileWrite.Flush();

                    //Write header for initializing strips section
                    setupFileWrite.Write("//Initialize Strips for MCU: " + mcuName +"***************************************************************************************************" + System.Environment.NewLine + System.Environment.NewLine);

                    //Loop through pin setups and gather lighting effects
                    for (i = 0; i < pinSetups.Count; i++)
                    {
                        //Get pin setup pins
                        pins = getMultipleStringValuesFromDB("MCU_PINS", "PINS", pinSetups[i], null);

                        //Split pins
                        dataClockPins = pins[0].Split(';');

                        //Write strips initialization
                        setupFileWrite.Write(createStructInfo("STRIP", i.ToString(), "numPixels" + (i + 1), dataClockPins[0], dataClockPins[1], "seqs" + (i + 1), "numEffects" + (i + 1), null, null, null, null, null, null));
                    }
                    setupFileWrite.Write(System.Environment.NewLine);
                    setupFileWrite.Write("//End of Initialize Strips for MCU: " + mcuName + "***************************************************************************************************" + System.Environment.NewLine + System.Environment.NewLine);
                    setupFileWrite.Flush();

                    //Write header for initializing effects manager section
                    setupFileWrite.Write("//Initialize Effects Manager for MCU: " + mcuName + "***************************************************************************************" + System.Environment.NewLine + System.Environment.NewLine);

                    //Write effects manager initialization
                    setupFileWrite.Write("//Initialize effects manager" + System.Environment.NewLine +
                        "effectsManager = EffectsManager(strips, numStrips);" + System.Environment.NewLine +
                        "effectsManager.effectsManagerUpdateRet = uRet;" + System.Environment.NewLine + System.Environment.NewLine);

                    setupFileWrite.Write("//End of Initialize Effects Manager for MCU: " + mcuName + "***************************************************************************************" + System.Environment.NewLine + System.Environment.NewLine);
                    setupFileWrite.Write(System.Environment.NewLine + System.Environment.NewLine);
                    setupFileWrite.Flush();
                }

                //Notify user
                MessageBox.Show("Project successfully written to file as structs...");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error writing Project from DB to structs within file...: " + ex.Message);
                CloseDBConnection();
                CloseDBConnection3();
            }
        }

        /*
        */
        public int getPinSetupValue(String MCUName, String[] PinSetup)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            int iret = -1;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string
                cmd.CommandText = "Select Pin_Setup from MCU_Pins where MCU_Name = @CName and Data_Pin = @DPin and Clock_Pin = @CPin";
                cmd.Parameters.AddWithValue("@CName", MCUName);
                cmd.Parameters.AddWithValue("@DPin", int.Parse(PinSetup[0].Trim()));
                cmd.Parameters.AddWithValue("@CPin", int.Parse(PinSetup[1].Trim()));

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    iret = rdr.GetInt32(0);
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error getting true Pin Setup value...: " + ex.Message);
            }

            return iret;
        }

        /*
        */
        public List<int> getDistinctPinSetups(String ProjectName)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            List<int> pinSetups = new List<int>();

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string
                cmd.CommandText = "Select Distinct Pin_Setup from LED_Effect where Project_Name = @PName";
                cmd.Parameters.AddWithValue("@PName", ProjectName);

                rdr = cmd.ExecuteReader();
                while(rdr.Read())
                {
                    pinSetups.Add(rdr.GetInt32(0));
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting list of Pin Setups...: " + ex.Message);
            }

            return pinSetups;
        }

        /*
         * 
        */
        public int getNumLEDs(String ProjectName, String MCUName, int PinSetup)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            int iret = -1;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string
                cmd.CommandText = "Select Distinct Num_Leds from LED_Effect where Project_Name = @PName and MCU_Name = @CName and Pin_Setup = @Pin order by Num_Leds desc";
                cmd.Parameters.AddWithValue("@PName", ProjectName);
                cmd.Parameters.AddWithValue("@CName", MCUName);
                cmd.Parameters.AddWithValue("@Pin", PinSetup);

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    iret = rdr.GetInt32(0);
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting true Pin Setup value...: " + ex.Message);
            }

            return iret;
        }

        /*
        */
        public string getValueFromDB(String Table, String Option, String Param1, String Param2, String Param3)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            String sret = "";

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                switch (Table)
                {
                    case "LED_EFFECT":
                        if (Option.Equals("LEDPARRAY"))
                        {
                            //Conjure query string
                            cmd.CommandText = "Select LED_Position_Array from LED_Effect where Project_Name = @PName and MCU_Name = @MName and Pin_Setup = @PSetup";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@MName", Param2);
                            cmd.Parameters.AddWithValue("@PSetup", int.Parse(Param3));
                        }
                        else if (Option.Equals("DISTINCTNUMLEDS"))
                        {
                            //Conjure query string
                            cmd.CommandText = "Select Distinct Num_Leds from LED_Effect where Project_Name = @PName and MCU_Name = @CName and Pin_Setup = @Pin order by Num_Leds desc";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@CName", Param2);
                            cmd.Parameters.AddWithValue("@Pin", Param3);
                        }
                        else if (Option.Equals("NUMLEFFECTS"))
                        {
                            //Conjure query string
                            cmd.CommandText = "Select Count(Pin_Setup) from LED_Effect where Project_Name = @PName and MCU_Name = @CName and Pin_Setup = @Pin order by Num_Leds desc";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@CName", Param2);
                            cmd.Parameters.AddWithValue("@Pin", Param3);
                        }
                        else
                        {
                            //Conjure query string
                            cmd.CommandText = "Select LED_Color_Array from LED_Effect where Project_Name = @PName and MCU_Name = @MName and Pin_Setup = @PSetup";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@MName", Param2);
                            cmd.Parameters.AddWithValue("@PSetup", int.Parse(Param3));
                        }
                        break;
                    case "MCU_PINS":
                        if (Option.Equals("PINSETUP"))
                        {
                            //Conjure query string
                            cmd.CommandText = "Select Pin_Setup from MCU_Pins where MCU_Name = @CName and Data_Pin = @DPin and Clock_Pin = @CPin";
                            cmd.Parameters.AddWithValue("@CName", Param1);
                            cmd.Parameters.AddWithValue("@DPin", int.Parse(Param2));
                            cmd.Parameters.AddWithValue("@CPin", int.Parse(Param3));
                        }
                        else if (Option.Equals("MCUNAME"))
                        {
                            //Conjure query string
                            cmd.CommandText = "Select MCU_Name from MCU_Pins where Pin_Setup = @PSetup";
                            cmd.Parameters.AddWithValue("@PSetup", Param1);
                        }
                        else
                        {

                        }

                        break;
                }

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    sret = rdr.GetString(0);
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting " + Option + " from " + Table + ": " + ex.Message);
            }

            return sret;
        }

        /*
        */
        public List<String> getMultipleStringValuesFromDB(String Table, String Option, String Param1, String Param2)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            List<String> sret = null;
            String str = "";

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                switch (Table)
                {
                    case "LED_EFFECT":
                        if (Option.Equals("MCUS"))
                        {
                            //Conjure query string
                            cmd.CommandText = "Select Distinct MCU_Name from LED_Effect where Project_Name = @PName order by MCU_Name";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                        }
                        else if (Option.Equals("PINSETUPS"))
                        {
                            //Conjure query string
                            cmd.CommandText = "Select Distinct Pin_Setup from LED_Effect where Project_Name = @PName and MCU_Name = @MCName order by Pin_Setup";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@MCName", Param2);
                        }
                        else if (Option.Equals("LEFFECTS"))
                        {
                            cmd.CommandText = "Select LIGHTING_EFFECT, LED_COLOR_ARRAY, (DELAY_TIME * 1000) AS DELAY_TIME, (EFFECT_DURATION * 1000) as EFFECT_DURATION, BOUNCES, ITERATIONS " +
                                "BRIGHTNESS, INCR_BRIGHTNESS, (BRIGHTNESS_DELAYTIME * 1000) AS BRIGHTNESS_DELAYTIME from Led_Effect where Project_Name = @PName and Pin_Setup = @PSetup order by Effect_Start";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@PSetup", int.Parse(Param2));
                        }
                        else
                        {

                        }
                        break;
                    case "MCU_PINS":
                        if (Option.Equals("PINS"))
                        {
                            //Conjure query string
                            cmd.CommandText = "Select Data_Pin, Clock_Pin from MCU_PINS where Pin_Setup = @PSetup";
                            cmd.Parameters.AddWithValue("@PSetup", int.Parse(Param1));
                        }else
                        {

                        }
                        break;
                }

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (sret == null)
                    {
                        sret = new List<String>();
                    }

                    //Reset string value
                    str = "";

                    for (int i = 0; i < rdr.VisibleFieldCount; i++)
                    {
                        str += rdr[i].ToString().Trim() + ";";
                    }

                    //Trim last ; off
                    str = str.Substring(0, (str.Length - 1));

                    //sret.Add(rdr.GetString(0));
                    sret.Add(str);
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting " + Option + " from " + Table + ": " + ex.Message);
                sret = null;
            }

            return sret;
        }

        /*
            Method: getInstancesCount
                This method will query a designated table's count of particular instances.
            
            Parameters: String Table - specifies which table to query in database, String Option - allows user to choose different options
                of querying within the specified table, String Param1 - first parameter table to should match (optional), String param2 - 
                second parameter table should match  (optional)
            
            Returns: int - if query is successful a count of found instances will be returned, returns -1 if query fails
        */
        public int getInstancesCount(String Table, String Option, String Param1, String Param2)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            int iret = -1;

            try
            {
                //Open db connection
                OpenDBConnection2();

                cmd = con2.CreateCommand();

                //Conjure query string
                switch (Table)
                {
                    case "LED_EFFECT":
                        if (Option.Equals("STRIPS"))
                        {
                            cmd.CommandText = "Select Count(*) from LED_Effect where Project_Name = @PName and Pin_Setup = @Pin";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@Pin", Param2);
                        }
                        else if (Option.Equals("MCUS"))
                        {
                            cmd.CommandText = "Select Count(Distinct MCU_Name) from LED_Effect where Project_Name = @PName and Pin_Setup = @Pin";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@Pin", Param2);
                        }
                        else
                        {
                            cmd.CommandText = "Select Count(*) from LED_Effect where Project_Name = @PName and Pin_Setup = @Pin";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@Pin", Param2);
                        }
                        break;
                }

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    if (rdr.GetString(0).Trim().Equals(""))
                    {
                        iret = 0;
                    }
                    else
                    {
                        iret = rdr.GetInt32(0);
                    }
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting true Pin Setup value...: " + ex.Message);
            }

            return iret;
        }

        #endregion Public Methods

        #region Private Methods

        /*
           Method createStructInfo:
            This function will create struct like info for writing to file for passed info
        */
        private String createStructInfo(String Type, String Value1, String Value2, String Value3, String Value4, String Value5, String Value6, String Value7, String Value8, String Value9, String Value10, String Value11, String Value12)
        {
            //Declare variables
            String sRet = "", effect = "";
            String[] splitColorArray = { "" };
            Decimal dec = 0;
            int effectCode = 0, i = 0, delayTime = 0, duration = 0, bounces = 0, iterations = 0, brightness = 0, incrBrightness = 0, brightnessDelayTime = 0;

            //Conjure string by type
            switch (Type)
            {
                case "STRIP":
                    sRet += "strips[" + Value1 + "] = Strip(" + Value2 + ", " + Value3 + ", " + Value4 + ", DOTSTAR_RGB, " + Value5 + ", " + Value6 + ");" + System.Environment.NewLine;
                    sRet += "strips[" + Value1 + "].getStrip()->begin();" + System.Environment.NewLine;
                    sRet += "strips[" + Value1 + "].getStrip()->show();" + System.Environment.NewLine + System.Environment.NewLine;

                    break;
                case "LEFFECT":
                    //Get name of effect
                    int.TryParse(Value2, out effectCode);
                    effect = Effects.getEffectFromCode(effectCode);

                    //Split led color array (value 6) by comma then reassemble with only numbers for color specifications
                    try
                    {
                        splitColorArray = Value4.Split(',');
                        Value4 = "";
                        for (i = 0; i < splitColorArray.Length; i++)
                        {
                            Value4 += splitColorArray[i].Split('-')[0].Trim() + ",";
                        }

                        //Trim last ;
                        Value4 = Value4.Substring(0, (Value4.Length - 1)) + " ";
                    }
                    catch (Exception ex)
                    {
                        //Default value6 to 0
                        Value4 = "0";
                    }

                    //Setup delayTime, duration, bounces, iterations, brightness, incr brightness, and brightness delay time
                    if (Decimal.TryParse(Value5, out dec)) { delayTime = Decimal.ToInt32(dec); }else {delayTime = -1; }
                    if (Decimal.TryParse(Value6, out dec)) { duration = Decimal.ToInt32(dec); }else { duration = -1; }
                    if (Decimal.TryParse(Value7, out dec)) { bounces = Decimal.ToInt32(dec); }else { bounces = -1; }
                    if (Decimal.TryParse(Value8, out dec)) { iterations = Decimal.ToInt32(dec); }else { iterations = -1; }
                    if (Decimal.TryParse(Value9, out dec)) { brightness = Decimal.ToInt32(dec); } else { brightness = -1; }
                    if (Decimal.TryParse(Value10, out dec)) { incrBrightness = Decimal.ToInt32(dec); } else { incrBrightness = -1; }
                    if (Decimal.TryParse(Value11, out dec)) { brightnessDelayTime = Decimal.ToInt32(dec); } else { brightnessDelayTime = -1; }

                    //Add lighting effect info to strip
                    sRet += "seqs" + Value12 + "[" + Value1 + "] = {" + effect + ", " + Value3 + Value12 + ", \"" + Value4 + "\", " + delayTime + ", " + duration + ", " + bounces + ", " + iterations + ", " + brightness + ", " + incrBrightness + ", " + brightnessDelayTime + "}" + ";";

                    break;
            }

            return sRet;
        }

        /*
        */
        private bool OpenDBConnection()
        {
            bool bret = false;

            try {
                //Connect to database
                con = new MySqlConnection(connString);
                con.Open();
                bret = true;
            }catch(Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool OpenDBConnection2()
        {
            bool bret = false;

            try
            {
                //Connect to database
                con2 = new MySqlConnection(connString);
                con2.Open();
                bret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool OpenDBConnection3()
        {
            bool bret = false;

            try
            {
                //Connect to database
                con3 = new MySqlConnection(connString);
                con3.Open();
                bret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool CloseDBConnection()
        {
            bool bret = false;

            try
            {
                //Disconnect from database
                con.Close();
                bret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disconnecting from database: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool CloseDBConnection2()
        {
            bool bret = false;

            try
            {
                //Disconnect from database
                con2.Close();
                bret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disconnecting from database: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool CloseDBConnection3()
        {
            bool bret = false;

            try
            {
                //Disconnect from database
                con3.Close();
                bret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disconnecting from database: " + ex.Message);
            }

            return bret;
        }

        #endregion Private Methods


        #region Screen Events

        /*
        */
        public void dgvProjectData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnEditRecord_Click(null, null);
        }

        /*
        */
        public void btnEditRecord_Click(object sender, EventArgs e)
        {
            //Declare variables
            int selectedRow, numOfLEDs, effectNum, iterations, bounces, brightness, incrBrightness;
            float effectStart, effectDuration, delayTime, brightnessDelayTime;
            String projectName, mcuName, pinSetup, lightingEffect, currentSongPath;
            List<int> ledPArray = new List<int>();
            List<String> ledCArray = new List<string>();
            String[] temp = { "" };

            //Verify a record is selected within grid
            if (projectGrid.CurrentRow != null && projectGrid.CurrentCell != null)
            {

                //Get values from selected record
                selectedRow = projectGrid.CurrentRow.Index;
                currentSongPath = "";
                projectName = projectGrid.Rows[selectedRow].Cells["PROJECT_NAME"].Value.ToString().Trim() + " - " + projectGrid.Rows[selectedRow].Cells["PROJECT_DESC"].Value.ToString().Trim();
                mcuName = projectGrid.Rows[selectedRow].Cells["MCU_NAME"].Value.ToString().Trim() + " - " + projectGrid.Rows[selectedRow].Cells["MCU_DESC"].Value.ToString().Trim();
                pinSetup = projectGrid.Rows[selectedRow].Cells["DATA_PIN"].Value.ToString().Trim() + ";" + projectGrid.Rows[selectedRow].Cells["CLOCK_PIN"].Value.ToString().Trim() + ";";
                numOfLEDs = int.Parse(projectGrid.Rows[selectedRow].Cells["NUM_LEDS"].Value.ToString().Trim());

                temp = projectGrid.Rows[selectedRow].Cells["LED_POSITION_ARRAY"].Value.ToString().Split(',');
                foreach(String s in temp)
                {
                    ledPArray.Add(int.Parse(s));
                }
                temp = projectGrid.Rows[selectedRow].Cells["LED_COLOR_ARRAY"].Value.ToString().Split(',');
                foreach(String s in temp)
                {
                    ledCArray.Add(s);
                }
                lightingEffect = projectGrid.Rows[selectedRow].Cells["LIGHTING_EFFECT"].Value.ToString().Trim() + " - " + projectGrid.Rows[selectedRow].Cells["DESCRIPTION"].Value.ToString().Trim();
                effectStart = float.Parse(projectGrid.Rows[selectedRow].Cells["EFFECT_START"].Value.ToString().Trim());
                effectDuration = float.Parse(projectGrid.Rows[selectedRow].Cells["EFFECT_DURATION"].Value.ToString().Trim());
                delayTime = float.Parse(projectGrid.Rows[selectedRow].Cells["DELAY_TIME"].Value.ToString().Trim()); ;
                iterations = int.Parse(projectGrid.Rows[selectedRow].Cells["ITERATIONS"].Value.ToString().Trim()); ;
                bounces = int.Parse(projectGrid.Rows[selectedRow].Cells["BOUNCES"].Value.ToString().Trim()); ;
                effectNum = int.Parse(projectGrid.Rows[selectedRow].Cells["EFFECT_NUM"].Value.ToString().Trim());
                brightness = int.Parse(projectGrid.Rows[selectedRow].Cells["BRIGHTNESS"].Value.ToString().Trim());
                incrBrightness = int.Parse(projectGrid.Rows[selectedRow].Cells["INCR_BRIGHTNESS"].Value.ToString().Trim());
                brightnessDelayTime = float.Parse(projectGrid.Rows[selectedRow].Cells["BRIGHTNESS_DELAYTIME"].Value.ToString().Trim());

                //Open project form with initial values for timer and song file path
                Project pj = new Project(this, projectName, mcuName, pinSetup, numOfLEDs, ledPArray, ledCArray, lightingEffect, effectStart, effectDuration, delayTime, iterations, bounces, effectNum, brightness, incrBrightness, brightnessDelayTime, currentSongPath, false);
                //pj.Owner = (LEDLightingComposerCS)sender;
                pj.Show();
            }
        }

        /*
        */
        public void btnAdd2Project_Click(object sender, EventArgs e, LEDLightingComposerCS LLC, int TimerVal, String CurrentSongPath)
        {
            //Open project form with initial values for timer and song file path
            Project pj = new Project(this, TimerVal, CurrentSongPath, true);
            pj.Owner = LLC;
            pj.Show();
        }

        /*
        */
        public void btnOpenProject_Click(object sender, EventArgs e, LEDLightingComposerCS LLC, DrawingManager DManager)
        {
            //Open Project Name form where user can select project to load to grid...
            SearchBrowser sb = new SearchBrowser(this.projectGrid, this, LLC, DManager);
            sb.Owner = LLC;
            sb.Show();
        }

        /*
        */
        public void btnSendViaHTTP_Click(object sender, EventArgs e)
        {

        }

        /*
        */
        public void btnSend2SDCard_Click(object sender, EventArgs e)
        {

        }

        /*
           Function btnSend2LocalFile_Click:
           This function will write all project led effects to user designated file as structs for copying into microcontroller code
        */
        public void btnSend2LocalFile_Click(object sender, EventArgs e)
        {
            //Declare variables
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            FileStream setupFile = null;
            String projectName = "", stripSetupFilename = "";

            //Verify records exist within grid
            if (projectGrid.Rows.Count <= 0)
            {
                //Notify user records must be within grid before sending to local file
                MessageBox.Show("You must load a project into grid before sending microcontroller information to file...");
                return;
            }

            //Get project name from datagridview
            projectName = projectGrid.Rows[0].Cells["PROJECT_NAME"].Value.ToString().Trim();

            //Allow user to choose file path and name
            saveFileDialog1.Filter = "Notepad|*.txt";
            saveFileDialog1.Title = "Save Microcontroller Lighting Effects Text File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                //Conjure filenames for Strip and LightingEffects files
                stripSetupFilename = saveFileDialog1.FileName.Split('.')[0] + "-" + projectName + "MicrocontrollersSetup" + ".txt";

                // Saves the Text via a FileStream created by the OpenFile method.
                saveFileDialog1.FileName = stripSetupFilename;
                setupFile = (System.IO.FileStream)saveFileDialog1.OpenFile();
            }
            else
            {
                //Notify user file was not created successfully
                MessageBox.Show("File was not created successfully.  Please try again...");
                return;
            }

            //Loop through LED_Effect table for project and write structs to user selected file path and name
            writeProjectFromDB2StructsLocalFile(projectName, setupFile);

            //Close file
            setupFile.Close();
        }

        /*
        */
        public void btnClearGrid_Click(object sender, EventArgs e, DrawingManager dmanager)
        {
            //Clear grid
            this.projectGrid.DataSource = null;

            //Clear Drawing Manager's LED Strip Effect
            dmanager.DrawableObjects.Clear();

            //Clear project label
            this.projectName.Text = "";
        }

        #endregion Screen Events


        #region Getters & Setters

        public String LED_Project
        {
            get { return LED_PROJECT; }
        }

        public String Lighting_Effects
        {
            get { return LIGHTING_EFFECTS; }
        }

        public String Mcu
        {
            get { return MCU; }
        }

        public String MCU_Pins
        {
            get { return MCU_PINS; }
        }

        public String MCU_Pins_Option1
        {
            get { return MCU_PINS_OPTION1; }
        }

        public DataGridView ProjectGrid
        {
            get { return this.projectGrid; }
        }

        #endregion Getters & Setters
    }
}
