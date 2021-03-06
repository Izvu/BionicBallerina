﻿/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified:
	Class: Project.cs
	Class Description:
		This class handles the opening of a windows for that allows the user to add, edit, and delete effects to a project.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LEDLightingComposer
{
    public partial class Project : Form
    {
        //Declare global variables
        private DatabaseManager dbmanager;
        private List<int> ledPArray = new List<int>();
        private List<String> ledCArray = new List<string>();
        private int effectNum, lastNumLEDs = 0;
        private bool add;

        /*
            Function: 

            Parameters:

            Returns: 
        */
        public Project(DatabaseManager DBManager, bool Add)
        {
            InitializeComponent();

            //Set local to passed
            this.dbmanager = DBManager;
            this.add = Add;

            if (add)
            {
                this.btnSave2Project.Text = "Save 2 Project";
            }
            else
            {
                this.btnSave2Project.Text = "Edit Project";

                this.cBoxProjectName.Enabled = false;
                this.cBoxMCUName.Enabled = false;
                this.cBoxPinSetup.Enabled = false;
                this.btnDelete.Visible = true;
                this.btnResetScreen.Visible = false;
            }

            //Setup screen values
            setupScreenValues();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        public Project(DatabaseManager DBManager, int TimerVal, String CurrentSongPath, bool Add)
        {
            //Initialize Components
            InitializeComponent();

            //Set global variables to passed variables
            this.dbmanager = DBManager;
            this.add = Add;

            //Load Project Names from LED_Project table
            dbmanager.loadCBoxByType(DatabaseManager.TYPE.ProjectNames,this.cBoxProjectName,"");

            //Load MCU Names from MCU table
            dbmanager.loadCBoxByType(DatabaseManager.TYPE.MCUNames,this.cBoxMCUName, getProjectName());

            //Load Lighting Effects from Lighting_Effects table
            dbmanager.loadCBoxByType(DatabaseManager.TYPE.LightingEffects,this.cBoxLEffect,"");

            //Setup screen values
            setupScreenValues();

            //Set screen values to default
            this.txtEffectStartTime.Text = TimerVal.ToString();
            this.txtEffectStartTime.Update();
            this.txtSongPath.Text = CurrentSongPath;
            this.txtSongPath.Update();

            if (add)
            {
                this.btnSave2Project.Text = "Save 2 Project";
            }
            else
            {
                this.btnSave2Project.Text = "Edit Project";

                this.cBoxProjectName.Enabled = false;
                this.cBoxMCUName.Enabled = false;
                this.cBoxPinSetup.Enabled = false;
                this.btnDelete.Visible = true;
                this.btnResetScreen.Visible = false;
            }
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        public Project(DatabaseManager DBManager, String ProjectName, String MCUName, String PinSetup, int NumOfLEDs, List<int> LEDPArray, List<String> LEDCArray, String LightingEffect, float EffectStart, float EffectDuration, float DelayTime, int Iterations, int Bounces, int EffectNum, int Brightness, int IncrBrightness, float BrightnessDelayTime, String CurrentSongPath, bool Add)
        {
            //Initialize Components
            InitializeComponent();

            //Declare variables


            //Set local to passed
            this.dbmanager = DBManager;
            this.effectNum = EffectNum;
            this.add = Add;

            //Load Project Names from LED_Project table
            dbmanager.loadCBoxByType(DatabaseManager.TYPE.ProjectNames, this.cBoxProjectName, "");

            //Load MCU Names from MCU table
            dbmanager.loadCBoxByType(DatabaseManager.TYPE.MCUNames, this.cBoxMCUName, getProjectName());

            //Load Lighting Effects from Lighting_Effects table
            dbmanager.loadCBoxByType(DatabaseManager.TYPE.LightingEffects, this.cBoxLEffect, "");

            //Set fields on-screen
            this.cBoxProjectName.Text = ProjectName;
            this.cBoxMCUName.Text = MCUName;
            this.cBoxPinSetup.Text = PinSetup;
            this.cBoxLEffect.Text = LightingEffect;

            this.txtNumLEDs.Text = NumOfLEDs.ToString();
            this.txtEffectStartTime.Text = EffectStart.ToString();
            this.txtEffectDuration.Text = EffectDuration.ToString();
            this.txtDelayTime.Text = DelayTime.ToString();
            this.txtIterations.Text = Iterations.ToString();
            this.txtBounces.Text = Bounces.ToString();
            this.txtBrightness.Text = Brightness.ToString();
            this.txtIncrBrightness.Text = IncrBrightness.ToString();
            this.txtBrightnessDelayTime.Text = BrightnessDelayTime.ToString();
            this.txtSongPath.Text = CurrentSongPath;

            this.ledPArray = LEDPArray;
            this.ledCArray = LEDCArray;

            //Disable and setup certain fields
            if (add)
            {
                this.btnSave2Project.Text = "Save 2 Project";
            }
            else
            {
                this.btnSave2Project.Text = "Edit Project";

                this.cBoxProjectName.Enabled = false;
                this.cBoxMCUName.Enabled = false;
                this.cBoxPinSetup.Enabled = false;
                this.txtNumLEDs.Enabled = false;
                this.btnDelete.Visible = true;
                this.btnResetScreen.Visible = false;
            }
        }

        #region Verification Methods

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private bool veriProjectName()
        {
            //Declare variables
            bool bret = false;
            String[] pName = { "" };

            try {
                //Verify a project name has been entered
                if ((pName[0] = this.cBoxProjectName.Text.ToString().Trim().ToUpper()).Equals(""))
                {
                    //Notify user a Project name is required then return false
                    MessageBox.Show("Please enter a Project Name before trying to save...");
                    return bret;
                }

                //Verify if project exists, and if not create
                pName = pName[0].Split('-');
                if (!dbmanager.verifyExistenceInDatabase(DatabaseManager.DBTABLES.LED_Project, DatabaseManager.OPTIONS.NONE, pName[0].Trim(), "", ""))
                {
                    if(dbmanager.insertRecordIntoDBReturnIncr(DatabaseManager.DBTABLES.LED_Project, pName[0].Trim(), pName[1].Trim(), "", "","","","","","","","","","","","") < 1)
                    {
                        bret = false;
                    }
                    else
                    {
                        bret = true;
                    }
                }
                else
                {
                    bret = true;
                }

                //Update textbox string
                this.cBoxProjectName.Text = pName[0].Trim() + " - " + pName[1].Trim();
                this.cBoxProjectName.Update();
            }catch(Exception ex)
            {
                MessageBox.Show("Error verifying Project Name... : " + ex.Message);
            }

            return bret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private bool veriMCUName()
        {
            //Declare variables
            String[] mcuName = { "" };
            bool bret = false;

            try {
                if ((mcuName[0] = this.cBoxMCUName.Text.ToString().Trim().ToUpper()).Equals(""))
                {
                    //Notify user a mcu name is required
                    MessageBox.Show("Please enter/select a MCU Name before trying to save...");
                    return bret;
                }
                else
                {
                    //Create MCU Name in database if necessary
                    mcuName = mcuName[0].Split('-');
                    if (!dbmanager.verifyExistenceInDatabase(DatabaseManager.DBTABLES.MCU, DatabaseManager.OPTIONS.NONE, mcuName[0].Trim(), getProjectName(), ""))
                    {
                        if(dbmanager.insertRecordIntoDBReturnIncr(DatabaseManager.DBTABLES.MCU, mcuName[0].Trim(), mcuName[1].Trim(), getProjectName(), "","","","","","","","","","","","") < 1)
                        {
                            bret = false;
                        }
                        else
                        {
                            bret = true;
                        }
                    }
                    else
                    {
                        bret = true;
                    }

                    //Update textbox string
                    this.cBoxMCUName.Text = mcuName[0].Trim() + " - " + mcuName[1].Trim();
                    this.cBoxMCUName.Update();
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Error while verifying MCU Name...: " + ex.Message);
            }

            return bret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private bool veriPinSetup()
        {
            //Declare variables
            String[] pinSetup = { "" };
            String projectName = (this.cBoxProjectName.Text.ToString().Trim().ToUpper().Split('-'))[0].Trim(),
                mcuName = (this.cBoxMCUName.Text.ToString().Trim().ToUpper().Split('-'))[0].Trim();
            bool bret = false;

            try
            {
                if ((pinSetup[0] = this.cBoxPinSetup.Text.ToString().Trim().ToUpper()).Equals(""))
                {
                    //Notify user a pin setup is required
                    MessageBox.Show("Please enter/select a Pin Setup before trying to save...");
                    return bret;
                }
                else
                {
                    //Create MCU Name in database
                    pinSetup = pinSetup[0].Split(';');
                    if (!dbmanager.verifyExistenceInDatabase(DatabaseManager.DBTABLES.MCU_Pins, DatabaseManager.OPTIONS.NONE, mcuName, pinSetup[0].Trim(), pinSetup[1].Trim()))
                    {
                        if(dbmanager.insertRecordIntoDBReturnIncr(DatabaseManager.DBTABLES.MCU_Pins, pinSetup[2].Trim(), mcuName, 
                            pinSetup[0].Trim(), pinSetup[1].Trim(), "", "", "", null, null, null, null, null, null, null, null) < 1)
                        {
                            //Set bret to false since insert was not successful
                            bret = false;
                        }
                        else
                        {
                            //Set bret to true since insert was successful
                            bret = true;
                        }
                    }
                    else
                    {
                        //Set bret to true since record exists in table
                        bret = true;
                    }

                    //Update textbox string
                    this.cBoxPinSetup.Text = pinSetup[0].Trim() + ";" + pinSetup[1].Trim() + ";" + pinSetup[2].Trim();
                    this.cBoxPinSetup.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while verifying Pin Setup...: " + ex.Message);
            }

            return bret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private bool veriLEDPositionArray()
        {
            //Declare variables
            bool bret = false;

            try {
                if (ledPArray.Count == (int.Parse(this.txtNumLEDs.Text.ToString().Trim()))) { bret = true; }
            }catch(Exception ex)
            {
                MessageBox.Show("There was an error verifying LED Position Array count match...: " + ex.Message);
            }

            return bret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private bool veriLEDColorArray()
        {
            //Declare variables
            bool bret = false;

            try
            {
                if (ledCArray.Count == (int.Parse(this.txtNumLEDs.Text.ToString().Trim()))) { bret = true; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error verifying LED Position Array count match...: " + ex.Message);
            }

            return bret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private bool veriLightingEffect()
        {
            //Declare variables
            String[] lEffect = { "" };
            bool bret = false;

            try
            {
                if ((lEffect[0] = this.cBoxLEffect.Text.ToString().Trim().ToUpper()).Equals(""))
                {
                    //Notify user a mcu name is required
                    MessageBox.Show("Please enter/select an Lighting Effect before trying to save...");
                    return bret;
                }
                else
                {
                    //Create MCU Name in database if necessary
                    lEffect = lEffect[0].Split('-');
                    if (!dbmanager.verifyExistenceInDatabase(DatabaseManager.DBTABLES.Lighting_Effects, DatabaseManager.OPTIONS.NONE, lEffect[0].Trim(), "", ""))
                    {
                        if(dbmanager.insertRecordIntoDBReturnIncr(DatabaseManager.DBTABLES.Lighting_Effects, lEffect[0].Trim(), lEffect[1].Trim(), "", "","","","","","","","","","","","") < 1)
                        {
                            bret = false;
                        }
                        else
                        {
                            bret = true;
                        }
                    }
                    else
                    {
                        bret = true;
                    }

                    //Update textbox string
                    this.cBoxLEffect.Text = lEffect[0].Trim() + " - " + lEffect[1].Trim();
                    this.cBoxLEffect.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while verifying Lighting Effect...: " + ex.Message);
            }

            return bret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private int veriIntTextbox(TextBox txt)
        {
            //Declare variables
            int iret = -1;

            try
            {
                iret = int.Parse(txt.Text.ToString().Trim());
            }catch(Exception ex)
            {
                MessageBox.Show("Error verifying int in textbox...: " + ex.Message);
            }

            return iret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private float veriFloatTextbox(TextBox txt)
        {
            //Declare variables
            float fret = -1;

            try
            {
                fret = float.Parse(txt.Text.ToString().Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error verifying float in textbox...: " + ex.Message);
            }

            return fret;
        }

        #endregion Verification Methods

        #region Private Methods

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void setupScreenValues()
        {
            //Reset cbox values
            this.cBoxProjectName.Enabled = true;
            this.cBoxProjectName.SelectedIndex = -1;
            this.cBoxProjectName.Text = "Project Name - Description";
            this.cBoxProjectName.Update();
            this.cBoxMCUName.Enabled = true;
            this.cBoxMCUName.SelectedIndex = -1;
            this.cBoxMCUName.Text = "MCU Name - Description";
            this.cBoxMCUName.Update();
            this.cBoxPinSetup.Enabled = true;
            this.cBoxPinSetup.SelectedIndex = -1;
            this.cBoxPinSetup.Text = "DataPin#;ClockPin#;Description";
            this.cBoxPinSetup.Update();
            this.cBoxLEffect.SelectedIndex = -1;
            //this.cBoxLEffect.Text = "Effect# - Description";
            this.cBoxLEffect.SelectedIndex = 0;
            this.cBoxLEffect.Update();

            //Reset textbox values
            this.txtNumLEDs.Enabled = true;
            this.txtNumLEDs.Text = "2";
            this.txtNumLEDs.Update();
            this.txtEffectStartTime.Text = "0";
            this.txtEffectStartTime.Update();
            this.txtEffectDuration.Text = "0";
            this.txtEffectDuration.Update();
            this.txtDelayTime.Text = "0";
            this.txtDelayTime.Update();
            this.txtIterations.Text = "0";
            this.txtIterations.Update();
            this.txtBounces.Text = "0";
            this.txtBounces.Update();
            this.txtBrightness.Text = "255";
            this.txtBrightness.Update();
            this.txtIncrBrightness.Text = "0";
            this.txtIncrBrightness.Update();
            this.txtBrightnessDelayTime.Text = "0";
            this.txtBrightnessDelayTime.Update();

            //Reset btns
            this.btnSave2Project.Text = "Save 2 Project";
            this.btnDelete.Visible = false;
            this.btnResetScreen.Visible = true;

            //Reset arrays
            ledPArray.Clear();
            ledCArray.Clear();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void lockFields()
        {
            //Lock Project Name, MCU Name, Pin Setup, and # of LEDs fields
            this.cBoxProjectName.Enabled = false;
            this.cBoxMCUName.Enabled = false;
            this.cBoxPinSetup.Enabled = false;
            this.txtNumLEDs.Enabled = false;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        public String getProjectName()
        {
            return (this.cBoxProjectName.Text.ToString().Trim().Split('-'))[0].Trim();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private String getMCUName()
        {
            return this.cBoxMCUName.Text.ToString().Trim().ToUpper().Split('-')[0].Trim();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private String[] getPinSetup()
        {
            return this.cBoxPinSetup.Text.ToString().Trim().Split(';');
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private int convertTextBoxToInt(TextBox TxtBox)
        {
            int iret = -1;

            try
            {
                iret = int.Parse(TxtBox.Text.ToString().Trim());
            }catch(Exception ex)
            {

            }

            return iret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private float convertTextBoxToFloat(TextBox TxtBox)
        {
            float fret = -1;

            try
            {
                fret = float.Parse(TxtBox.Text.ToString().Trim());
            }
            catch (Exception ex)
            {

            }

            return fret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private int getLightingEffect()
        {
            return int.Parse(this.cBoxLEffect.Text.ToString().Trim().Split('-')[0].Trim());
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private String getLEDPArray()
        {
            String sret = "";

            for (int i = 0; i < ledPArray.Count; i++)
            {
                if (i == (ledPArray.Count - 1))
                {
                    sret += ledPArray[i].ToString().Trim();
                }
                else
                {
                    sret += ledPArray[i].ToString().Trim() + ",";
                }
            }

            return sret;
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private String getLEDCArray()
        {
            String sret = "";

            for (int i = 0; i < ledCArray.Count; i++)
            {
                if (i == (ledCArray.Count - 1))
                {
                    sret += ledCArray[i].ToString().Trim();
                }
                else
                {
                    sret += ledCArray[i].ToString().Trim() + ",";
                }
            }

            return sret;
        }

        #endregion Private Methods

        #region Screen events

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void btnLEDPArray_Click(object sender, EventArgs e)
        {
            int leds = 0;

            if (this.txtNumLEDs.Text.ToString().Trim().Equals(""))
            {
                //Notify user they must enter # of leds first
                MessageBox.Show("Please enter number of leds before clicking this button...");
                return;
            }
            else
            {
                try
                {
                    leds = Convert.ToInt32(this.txtNumLEDs.Text.ToString().Trim());
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            ScreenArraySelections ledp = new ScreenArraySelections("LED", leds, ledPArray, null, null);
            ledp.Owner = this;
            ledp.Show();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void btnLEDCArray_Click(object sender, EventArgs e)
        {
            int leds = 0;

            if (this.txtNumLEDs.Text.ToString().Trim().Equals(""))
            {
                //Notify user they must enter # of leds first
                MessageBox.Show("Please enter number of leds before clicking this button...");
                return;
            }
            else
            {
                try
                {
                    leds = Convert.ToInt32(this.txtNumLEDs.Text.ToString().Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            ScreenArraySelections ledc = new ScreenArraySelections("COLOR", leds, null, ledCArray, null);
            ledc.Owner = this;
            ledc.Show();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void btnSave2Project_Click(object sender, EventArgs e)
        {
            //Declare variables
            String projectName = "", mcuName = "", ledPArray = "", ledCArray = "";
            String[] pinSetup = { "" };
            int numLEDs = 0, lightingEffect, effectNum = 0, iterations = 0, bounces = 0, truePinSetupVal = 0, brightness = 0, incrBrightness = 0;
            float effectStart = 0, effectDuration = 0, delayTime = 0, brightnessDelayTime = 0;

            //Verify Project Name has been entered (if yes, verify if Project Name exists in database and if not, create in database)
            if (!veriProjectName()) { return; }
            else
            {
                //Verify if project exists and create if it doesn't
                veriProjectName();

            }

            //Verify MCU Name has been entered (if yes, verify if MCU Name exists in database and if not, create in database)
            if (!veriMCUName()) { return; }

            //Verify Pin Setup has been entered (if yes, verify if Pin Setup exists in database and if not, create in database)
            if (!veriPinSetup()) { return; }

            //Verify # of leds entered
            if(veriIntTextbox(this.txtNumLEDs) < 1) {
                MessageBox.Show("Please enter number of LEDs > 0 before pressing the save button...");
                return;
            }

            //Verify LED Postion Array has a count equal to # of leds
            if (!veriLEDPositionArray()) { return; }

            //Verify LED Color Array has a count equal to # of leds
            if (!veriLEDColorArray()) { return; }

            //Verify Lighting effect has been entered and in correct format (0 - ????) (if yes, verify if Lighting Effect exists in database and if not, create in database)
            if (!veriLightingEffect()) { return; }

            //Verify Effect_Start has been entered
            if (veriFloatTextbox(this.txtEffectStartTime) < 0) {
                MessageBox.Show("Your entered effect start time must be at least 0...");
                return; }

            //Verify Effect_Duration has been entered
            if (veriFloatTextbox(this.txtEffectDuration) < 0) {
                MessageBox.Show("Your entered effect duration must be greater than 0...");
                return;
            }

            /*Validation done so Create LED_Effect*/

            //Set values from screen
            projectName = getProjectName();
            mcuName = getMCUName();
            pinSetup = getPinSetup();
            numLEDs = convertTextBoxToInt(this.txtNumLEDs);
            lightingEffect = getLightingEffect();
            effectStart = convertTextBoxToFloat(this.txtEffectStartTime);
            effectDuration = convertTextBoxToFloat(this.txtEffectDuration);
            ledPArray = getLEDPArray();
            ledCArray = getLEDCArray();
            int.TryParse(dbmanager.getValueFromDB(DatabaseManager.DBTABLES.MCU_Pins, DatabaseManager.OPTIONS.PinSetup, mcuName, pinSetup[0], pinSetup[1]), out truePinSetupVal);
            if(truePinSetupVal < 1)
            {
                //Notify user
                MessageBox.Show("Error retrieving true pin value.  Please retry...");
                return;
            }

            /*Set values and default to special if invalid*/

            //Set delay time
            try { delayTime = float.Parse(this.txtDelayTime.Text.ToString().Trim()); }
            catch (Exception ex) { delayTime = effectDuration; }

            //Set iterations
            try { iterations = int.Parse(this.txtIterations.Text.ToString().Trim()); }
            catch (Exception ex) { iterations = 1; }

            //Set bounces
            try { bounces = int.Parse(this.txtBounces.Text.ToString().Trim()); }
            catch (Exception ex) { bounces = 2; }

            //Set brightness
            try { brightness = int.Parse(this.txtBrightness.Text.ToString().Trim()); }
            catch (Exception ex) { brightness = 255; }

            //Set incrBrightness
            try { incrBrightness = int.Parse(this.txtIncrBrightness.Text.ToString().Trim()); }
            catch (Exception ex) { incrBrightness = 0; }

            //Set brightnessDelayTime
            try { brightnessDelayTime = float.Parse(this.txtBrightnessDelayTime.Text.ToString().Trim()); }
            catch (Exception ex) { brightnessDelayTime = effectDuration; }

            //Insert or update record according to add value
            if (add)
            {
                //Verify no overlapping effect times for Project Name - Pin Setup pair
                if (dbmanager.verifyOverlappingLightingEffects(projectName, truePinSetupVal, effectStart, effectDuration))
                {
                    //Notify user there is an overlapping lighting effect record
                    MessageBox.Show("There is an overlapping lighting effect.  Please review and modify if necessary.  NOTHING CHANGED...");
                    return;
                }

                if (truePinSetupVal < 0)
                {
                    //Notify user that pinsetup was not created successfully
                    MessageBox.Show("Your Pin Setup was not created successfully. Please try to save again...");
                    return;
                }

                //Insert records into LED_Effect table
                if ((effectNum = dbmanager.insertRecordIntoDBReturnIncr(DatabaseManager.DBTABLES.LED_Effect, projectName, mcuName, 
                    truePinSetupVal.ToString(), numLEDs.ToString(), lightingEffect.ToString(), effectStart.ToString(), 
                    effectDuration.ToString(), ledPArray, ledCArray, delayTime.ToString(), iterations.ToString(), 
                    bounces.ToString(), brightness.ToString(), incrBrightness.ToString(), brightnessDelayTime.ToString())) < 0)
                {
                    //Notify user effect was not created successfully
                    MessageBox.Show("The effect was not created successfully.  Please try again...");
                    return;
                }

                //Notify user that record was added to project successfully
                MessageBox.Show("The LED Effect record has been added to the database successfully...");
            }
            else
            {
                //Update record in LED_Effect table
                if (dbmanager.updateRecordInDB(DatabaseManager.DBTABLES.LED_Effect, this.effectNum.ToString(), ledPArray, ledCArray, 
                    lightingEffect.ToString(), effectStart.ToString(), effectDuration.ToString(), delayTime.ToString(), 
                    iterations.ToString(), bounces.ToString(), brightness.ToString(), incrBrightness.ToString(), 
                    brightnessDelayTime.ToString()) > 0)
                {

                    //Notify user that record was not edited successfully
                    MessageBox.Show("The LED Effect record has been edited in the database successfully...");

                    //Close project screen
                    this.Close();
                }
                else
                {
                    //Notify user that record was not edited successfully
                    MessageBox.Show("The LED Effect record was not edited successfully...");
                }
            }

            //Update song path for entire project

            

            //Update datagridview and drawing manager
            dbmanager.updateProjectsInProjectGrid(projectName);

            //Reset screen values
            //setupScreenValues();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void btnDelete_Click(object sender, EventArgs e)
        {
            //Delete record by effect_num
            dbmanager.deleteRecordFromDB(DatabaseManager.DBTABLES.LED_Effect, effectNum.ToString());

            //Update datagridview and drawing manager
            dbmanager.updateProjectsInProjectGrid(getProjectName());

            //close project screen
            this.Close();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void btnExit_Click(object sender, EventArgs e)
        {
            //Close program
            this.Close();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void cBoxProjectName_DropDownClosed(object sender, EventArgs e)
        {
            
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void cBoxProjectName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Clear MCU Name combobox
            this.cBoxMCUName.Items.Clear();

            //Load distinct MCU Names from LED_Effect table
            dbmanager.loadCBoxByType(DatabaseManager.TYPE.MCUNames, this.cBoxMCUName, getProjectName());
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void cBoxMCUName_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Clear Pin Setup combobox
            this.cBoxPinSetup.Items.Clear();

            //Load Pin Setups from MCU_Pins table
            dbmanager.loadCBoxByType(DatabaseManager.TYPE.MCUPins, this.cBoxPinSetup, getMCUName());
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void cBoxPinSetup_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Declare variables
            List<int> lPArray = new List<int>();
            List<String> lCArray = new List<string>();
            String[] pinSetup = getPinSetup(), temp = { "" };
            String projectName = (this.cBoxProjectName.Text.ToString().Trim().ToUpper().Split('-'))[0].Trim(),
                mcuName = (this.cBoxMCUName.Text.ToString().Trim().ToUpper().Split('-'))[0].Trim();
            int pinRec = -1, numLEDs = -1;

            //Exit if Pin Setup cBox is empty, ubound is less than 1, or pins are not numbers
            if (pinSetup[0].Equals(""))
            {
                //No PinSetup set so exit
                return;
            }
            else if (pinSetup.Count() < 1)
            {
                return;
            }
            else
            {
                //Verify pins are numbers
                try
                {
                    int.Parse(pinSetup[0].Trim());
                    int.Parse(pinSetup[1].Trim());
                }
                catch (Exception ex)
                {
                    return;
                }
            }

            //Verify Project Name and MCU Name are valid MCU_Pins table
            //If so, # of LEDs and lock Project Name, MCU Name, Pin Setup, # of LEDs
            int.TryParse(dbmanager.getValueFromDB(DatabaseManager.DBTABLES.MCU_Pins, DatabaseManager.OPTIONS.MCUPins_MCUName, mcuName, pinSetup[0], pinSetup[1]), out pinRec);
            if (pinRec > 0)
            {
                //Get number of LEDs from LED_Effect table

                //Set Number of LEDs
                if((numLEDs = int.Parse(dbmanager.getNumLEDs(projectName, mcuName, pinRec).ToString())) < 0)
                {
                    //Exit
                    return;
                }
                this.txtNumLEDs.Text = numLEDs.ToString();
                this.txtNumLEDs.Update();

                //Load LED Position Array values from last
                ledPArray.Clear();
                temp = dbmanager.getValueFromDB(DatabaseManager.DBTABLES.LED_Effect, DatabaseManager.OPTIONS.LEDPositionArray, projectName, mcuName, pinRec.ToString()).Split(';');
                try
                {
                    foreach (String s in temp)
                    {
                        ledPArray.Add(int.Parse(s));
                    }
                }catch(Exception ex)
                {

                }

                //Load LED Color Array values from last
                ledCArray.Clear();
                temp = dbmanager.getValueFromDB(DatabaseManager.DBTABLES.LED_Effect, DatabaseManager.OPTIONS.LEDColorArray, projectName, mcuName, pinRec.ToString()).Split(';');
                try
                {
                    foreach (String s in temp)
                    {
                        ledCArray.Add(s);
                    }
                }catch(Exception ex)
                {

                }

                //Lock fields
                lockFields();
            }
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void btnResetScreen_Click(object sender, EventArgs e)
        {
            //Reset screen values
            setupScreenValues();
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void cBoxLEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Declare variables
            String effect = "";
            int iEffect = -1;

            //Show or Hide Iterations and Bounces depending on Lighting Effects
            if (!(effect = this.cBoxLEffect.Text.ToString().Trim().ToUpper().Split('-')[0].Trim()).Equals(""))
            {
                iEffect = int.Parse(effect);
                if(iEffect == 1 || iEffect == 3 || iEffect == 4)
                {
                    //Show iterations and bounces labels and textboxes
                    this.lblIterations.Visible = true;
                    this.lblBounces.Visible = true;
                    this.txtIterations.Visible = true;
                    this.txtBounces.Visible = true;
                }else
                {
                    //Hide iterations and bounces labels and textboxes
                    this.lblIterations.Visible = false;
                    this.lblBounces.Visible = false;
                    this.txtIterations.Visible = false;
                    this.txtBounces.Visible = false;
                }
            }
        }

        /*
            Function: 

            Parameters:

            Returns: 
        */
        private void txtNumLEDs_Leave(object sender, EventArgs e)
        {
            //Declare variables
            String curNumLEDs = "";
            int iCurNumLEDs = 0, i = 0;

            if (!(curNumLEDs = this.txtNumLEDs.Text.ToString().Trim()).Equals(""))
            {
                iCurNumLEDs = int.Parse(curNumLEDs);
            }else
            {
                //Clear arrays
                ledCArray.Clear();
                ledPArray.Clear();

                //Exit method without proceeding since no # of LEDs present
                return;
            }

            if(iCurNumLEDs != lastNumLEDs)
            {
                //Clear arrays
                ledCArray.Clear();
                ledPArray.Clear();

                if (iCurNumLEDs > 0)
                {
                    //Create LEDPArray & LEDCArray (default all to 0 for clear) from number of LEDs
                    for (i = 0; i < iCurNumLEDs; i++)
                    {
                        ledPArray.Add(i);
                        ledCArray.Add("0 - Clear");
                    }
                }
            }
        }

        #endregion Screen events
    }
}
