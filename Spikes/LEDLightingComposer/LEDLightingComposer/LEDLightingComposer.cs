﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LEDLightingComposer
{
    public partial class LEDLightingComposer : Form
    {
        //Declare global variables
        private MusicManager musicmanager;
        private LEDManager ledmanager;
        private DatabaseManager databasemanager;
        private DrawingManager composerDrawManager;
        private System.Windows.Forms.Timer screenRefreshTimer;

        public LEDLightingComposer()
        {
            InitializeComponent();

            //Anchor components
            this.lblAudioControls.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.lblSongName.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.WMPlayer.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.lblTimer.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.txtTimer.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnJump2Secs.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnLoadSong.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

            this.lblProjectDatabase.Anchor = (AnchorStyles.Bottom| AnchorStyles.Left);
            this.lblProjectName.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnOpenProject.Anchor = (AnchorStyles.Bottom| AnchorStyles.Left);
            this.btnAdd2Project.Anchor = (AnchorStyles.Bottom| AnchorStyles.Left);
            this.btnEditRecord.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnClearGrid.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.dgvProjectData.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.btnSendViaHTTP.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnSend2SDCard.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnExit.Anchor = (AnchorStyles.Bottom| AnchorStyles.Left);

            //Initialize classes to handle certain form functions
            musicmanager = new MusicManager(this, this.WMPlayer, this.btnLoadSong, this.btnJump2Secs, this.txtTimer, this.lblSongName);
            ledmanager = new LEDManager();
            databasemanager = new DatabaseManager(this.lblProjectName, this.btnAdd2Project, this.btnEditRecord, this.btnSend2SDCard, this.btnSend2SDCard, this.btnOpenProject, this.dgvProjectData);
            composerDrawManager = new DrawingManager();

            //Initialize screen refresh timer
            screenRefreshTimer = new System.Windows.Forms.Timer();
            screenRefreshTimer.Enabled = true;
            screenRefreshTimer.Interval = 1000; /* 100 millisec */
            screenRefreshTimer.Tick += new EventHandler(TimerCallback);
        }

        public int getDrawingBottom()
        {
            return this.btnClearGrid.Top;
        }

        public int getDrawingRight()
        {
            return this.btnJump2Secs.Left;
        }

        /*
        */
        private void TimerCallback(object sender, EventArgs e)
        {
            this.Invalidate();
            return;
        }
        
        /*
        */
        public void startTicker()
        {
            //Start a ticker in new thread which will update timer textbox every second when media player is playing
            Thread t = new Thread(new ThreadStart(updateLabelThreadProc));
            t.Start();
        }

        #region Private Methods

        /*
        */
        private void updateLabelThreadProc()
        {
            while (musicmanager.isPlaying)
            {
                this.BeginInvoke(new MethodInvoker(UpdateLabel));
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void UpdateLabel()
        {
            musicmanager.UpdateLabel();
        }

        #endregion Private Methods

        private void btnLoadSong_Click(object sender, EventArgs e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.btnLoadSong_Click(sender, e);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //Close program
            this.Close();
        }

        private void btnSend2SDCard_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnSend2SDCard_Click(sender, e);
        }

        private void btnSendViaHTTP_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnSendViaHTTP_Click(sender, e);
        }

        private void btnEditRecord_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnEditRecord_Click(sender, e);
        }

        private void btnAdd2Project_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            int i = 0;
            if (!musicmanager.timer.Text.ToString().Trim().Equals(""))
            {
                i = int.Parse(musicmanager.timer.Text.ToString().Trim());
            }
            databasemanager.btnAdd2Project_Click(sender, e, this, i, musicmanager.CurrentSongFilePath);
        }

        private void btnJump2Secs_Click(object sender, EventArgs e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.btnJump2Secs_Click(sender, e);
        }

        private void WMPlayer_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.WMPlayer_PositionChange(sender, e);
        }

        private void WMPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.WMPlayer_PlayStateChange(sender, e);
        }

        private void dgvProjectData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.dgvProjectData_CellDoubleClick(sender, e);
        }

        private void btnClearGrid_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnClearGrid_Click(sender, e, composerDrawManager);
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnOpenProject_Click(sender, e, this, composerDrawManager);
        }

        private void LEDLightingComposer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.composerDrawManager = null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Draw the form's native elements
            base.OnPaint(e);

            //Draw items from DrawingManager
            try {
                composerDrawManager.draw(e.Graphics, getDrawingBottom(), getDrawingRight());
            }catch(Exception ex)
            {

            }
        }
    }
}
