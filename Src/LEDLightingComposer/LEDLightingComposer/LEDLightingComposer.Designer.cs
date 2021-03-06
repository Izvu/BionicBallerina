﻿namespace LEDLightingComposer
{
    partial class LEDLightingComposerCS
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LEDLightingComposerCS));
            this.dgvProjectData = new System.Windows.Forms.DataGridView();
            this.lblTimer = new System.Windows.Forms.Label();
            this.lblAudioControls = new System.Windows.Forms.Label();
            this.lblProjectDatabase = new System.Windows.Forms.Label();
            this.btnAdd2Project = new System.Windows.Forms.Button();
            this.btnSendViaHTTP = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblSongName = new System.Windows.Forms.Label();
            this.btnSend2SDCard = new System.Windows.Forms.Button();
            this.btnEditRecord = new System.Windows.Forms.Button();
            this.btnLoadSong = new System.Windows.Forms.Button();
            this.WMPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.txtTimer = new System.Windows.Forms.TextBox();
            this.btnJump2Secs = new System.Windows.Forms.Button();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.btnClearGrid = new System.Windows.Forms.Button();
            this.btnOpenProject = new System.Windows.Forms.Button();
            this.btnSend2LocalFile = new System.Windows.Forms.Button();
            this.tBarTimer = new System.Windows.Forms.TrackBar();
            this.pnlTBar = new System.Windows.Forms.Panel();
            this.pnlDraw = new System.Windows.Forms.Panel();
            this.chkSynchronizeMCUs = new System.Windows.Forms.CheckBox();
            this.chkSkipIPSetup = new System.Windows.Forms.CheckBox();
            this.btnMCUIPSetup = new System.Windows.Forms.Button();
            this.chkPlayerDelayTime = new System.Windows.Forms.CheckBox();
            this.txtPlayerDelayTime = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjectData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.WMPlayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarTimer)).BeginInit();
            this.pnlTBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvProjectData
            // 
            this.dgvProjectData.AllowUserToAddRows = false;
            this.dgvProjectData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProjectData.Location = new System.Drawing.Point(12, 317);
            this.dgvProjectData.Name = "dgvProjectData";
            this.dgvProjectData.ReadOnly = true;
            this.dgvProjectData.Size = new System.Drawing.Size(760, 189);
            this.dgvProjectData.TabIndex = 0;
            this.dgvProjectData.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvProjectData_CellDoubleClick);
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.Location = new System.Drawing.Point(401, 109);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(66, 13);
            this.lblTimer.TabIndex = 7;
            this.lblTimer.Text = "Timer - Secs";
            // 
            // lblAudioControls
            // 
            this.lblAudioControls.AutoSize = true;
            this.lblAudioControls.Location = new System.Drawing.Point(590, 9);
            this.lblAudioControls.Name = "lblAudioControls";
            this.lblAudioControls.Size = new System.Drawing.Size(75, 13);
            this.lblAudioControls.TabIndex = 8;
            this.lblAudioControls.Text = "Audio Controls";
            // 
            // lblProjectDatabase
            // 
            this.lblProjectDatabase.AutoSize = true;
            this.lblProjectDatabase.Location = new System.Drawing.Point(12, 294);
            this.lblProjectDatabase.Name = "lblProjectDatabase";
            this.lblProjectDatabase.Size = new System.Drawing.Size(89, 13);
            this.lblProjectDatabase.TabIndex = 9;
            this.lblProjectDatabase.Text = "Project Database";
            // 
            // btnAdd2Project
            // 
            this.btnAdd2Project.Location = new System.Drawing.Point(316, 271);
            this.btnAdd2Project.Name = "btnAdd2Project";
            this.btnAdd2Project.Size = new System.Drawing.Size(89, 23);
            this.btnAdd2Project.TabIndex = 10;
            this.btnAdd2Project.Text = "Add 2 Project";
            this.btnAdd2Project.UseVisualStyleBackColor = true;
            this.btnAdd2Project.Click += new System.EventHandler(this.btnAdd2Project_Click);
            // 
            // btnSendViaHTTP
            // 
            this.btnSendViaHTTP.Location = new System.Drawing.Point(221, 526);
            this.btnSendViaHTTP.Name = "btnSendViaHTTP";
            this.btnSendViaHTTP.Size = new System.Drawing.Size(135, 23);
            this.btnSendViaHTTP.TabIndex = 11;
            this.btnSendViaHTTP.Text = "Send 2 MCU Via HTTP";
            this.btnSendViaHTTP.UseVisualStyleBackColor = true;
            this.btnSendViaHTTP.Click += new System.EventHandler(this.btnSendViaHTTP_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(637, 526);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(135, 23);
            this.btnExit.TabIndex = 12;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblSongName
            // 
            this.lblSongName.AutoSize = true;
            this.lblSongName.Location = new System.Drawing.Point(590, 30);
            this.lblSongName.Name = "lblSongName";
            this.lblSongName.Size = new System.Drawing.Size(75, 13);
            this.lblSongName.TabIndex = 13;
            this.lblSongName.Text = "Name of Song";
            // 
            // btnSend2SDCard
            // 
            this.btnSend2SDCard.Location = new System.Drawing.Point(434, 526);
            this.btnSend2SDCard.Name = "btnSend2SDCard";
            this.btnSend2SDCard.Size = new System.Drawing.Size(135, 23);
            this.btnSend2SDCard.TabIndex = 14;
            this.btnSend2SDCard.Text = "Send 2 SD Card";
            this.btnSend2SDCard.UseVisualStyleBackColor = true;
            this.btnSend2SDCard.Click += new System.EventHandler(this.btnSend2SDCard_Click);
            // 
            // btnEditRecord
            // 
            this.btnEditRecord.Location = new System.Drawing.Point(412, 271);
            this.btnEditRecord.Name = "btnEditRecord";
            this.btnEditRecord.Size = new System.Drawing.Size(89, 23);
            this.btnEditRecord.TabIndex = 15;
            this.btnEditRecord.Text = "Edit Record";
            this.btnEditRecord.UseVisualStyleBackColor = true;
            this.btnEditRecord.Click += new System.EventHandler(this.btnEditRecord_Click);
            // 
            // btnLoadSong
            // 
            this.btnLoadSong.Location = new System.Drawing.Point(484, 221);
            this.btnLoadSong.Name = "btnLoadSong";
            this.btnLoadSong.Size = new System.Drawing.Size(276, 23);
            this.btnLoadSong.TabIndex = 16;
            this.btnLoadSong.Text = "Load Song";
            this.btnLoadSong.UseVisualStyleBackColor = true;
            this.btnLoadSong.Click += new System.EventHandler(this.btnLoadSong_Click);
            // 
            // WMPlayer
            // 
            this.WMPlayer.Enabled = true;
            this.WMPlayer.Location = new System.Drawing.Point(484, 52);
            this.WMPlayer.Name = "WMPlayer";
            this.WMPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("WMPlayer.OcxState")));
            this.WMPlayer.Size = new System.Drawing.Size(276, 163);
            this.WMPlayer.TabIndex = 17;
            this.WMPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.WMPlayer_PlayStateChange);
            this.WMPlayer.PositionChange += new AxWMPLib._WMPOCXEvents_PositionChangeEventHandler(this.WMPlayer_PositionChange);
            // 
            // txtTimer
            // 
            this.txtTimer.Location = new System.Drawing.Point(401, 127);
            this.txtTimer.Name = "txtTimer";
            this.txtTimer.Size = new System.Drawing.Size(67, 20);
            this.txtTimer.TabIndex = 18;
            // 
            // btnJump2Secs
            // 
            this.btnJump2Secs.Location = new System.Drawing.Point(401, 153);
            this.btnJump2Secs.Name = "btnJump2Secs";
            this.btnJump2Secs.Size = new System.Drawing.Size(67, 42);
            this.btnJump2Secs.TabIndex = 19;
            this.btnJump2Secs.Text = "Jump to Secs";
            this.btnJump2Secs.UseVisualStyleBackColor = true;
            this.btnJump2Secs.Click += new System.EventHandler(this.btnJump2Secs_Click);
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Location = new System.Drawing.Point(107, 294);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(108, 13);
            this.lblProjectName.TabIndex = 20;
            this.lblProjectName.Text = "Current Project Name";
            // 
            // btnClearGrid
            // 
            this.btnClearGrid.Location = new System.Drawing.Point(507, 271);
            this.btnClearGrid.Name = "btnClearGrid";
            this.btnClearGrid.Size = new System.Drawing.Size(89, 23);
            this.btnClearGrid.TabIndex = 21;
            this.btnClearGrid.Text = "Clear Grid";
            this.btnClearGrid.UseVisualStyleBackColor = true;
            this.btnClearGrid.Click += new System.EventHandler(this.btnClearGrid_Click);
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.Location = new System.Drawing.Point(221, 271);
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(89, 23);
            this.btnOpenProject.TabIndex = 22;
            this.btnOpenProject.Text = "Open Project";
            this.btnOpenProject.UseVisualStyleBackColor = true;
            this.btnOpenProject.Click += new System.EventHandler(this.btnOpenProject_Click);
            // 
            // btnSend2LocalFile
            // 
            this.btnSend2LocalFile.Location = new System.Drawing.Point(15, 526);
            this.btnSend2LocalFile.Name = "btnSend2LocalFile";
            this.btnSend2LocalFile.Size = new System.Drawing.Size(135, 23);
            this.btnSend2LocalFile.TabIndex = 23;
            this.btnSend2LocalFile.Text = "Send To Local File";
            this.btnSend2LocalFile.UseVisualStyleBackColor = true;
            this.btnSend2LocalFile.Click += new System.EventHandler(this.btnSend2LocalFile_Click);
            // 
            // tBarTimer
            // 
            this.tBarTimer.LargeChange = 1;
            this.tBarTimer.Location = new System.Drawing.Point(3, 3);
            this.tBarTimer.Name = "tBarTimer";
            this.tBarTimer.Size = new System.Drawing.Size(757, 45);
            this.tBarTimer.TabIndex = 24;
            this.tBarTimer.Visible = false;
            // 
            // pnlTBar
            // 
            this.pnlTBar.AutoScroll = true;
            this.pnlTBar.Controls.Add(this.tBarTimer);
            this.pnlTBar.Location = new System.Drawing.Point(15, 314);
            this.pnlTBar.Name = "pnlTBar";
            this.pnlTBar.Size = new System.Drawing.Size(757, 100);
            this.pnlTBar.TabIndex = 25;
            this.pnlTBar.Visible = false;
            // 
            // pnlDraw
            // 
            this.pnlDraw.AutoScroll = true;
            this.pnlDraw.BackColor = System.Drawing.Color.Transparent;
            this.pnlDraw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlDraw.Location = new System.Drawing.Point(15, 13);
            this.pnlDraw.Name = "pnlDraw";
            this.pnlDraw.Size = new System.Drawing.Size(374, 252);
            this.pnlDraw.TabIndex = 26;
            // 
            // chkSynchronizeMCUs
            // 
            this.chkSynchronizeMCUs.AutoSize = true;
            this.chkSynchronizeMCUs.Location = new System.Drawing.Point(394, 9);
            this.chkSynchronizeMCUs.Name = "chkSynchronizeMCUs";
            this.chkSynchronizeMCUs.Size = new System.Drawing.Size(116, 17);
            this.chkSynchronizeMCUs.TabIndex = 27;
            this.chkSynchronizeMCUs.Text = "Synchronize MCUs";
            this.chkSynchronizeMCUs.UseVisualStyleBackColor = true;
            // 
            // chkSkipIPSetup
            // 
            this.chkSkipIPSetup.AutoSize = true;
            this.chkSkipIPSetup.Location = new System.Drawing.Point(394, 32);
            this.chkSkipIPSetup.Name = "chkSkipIPSetup";
            this.chkSkipIPSetup.Size = new System.Drawing.Size(91, 17);
            this.chkSkipIPSetup.TabIndex = 28;
            this.chkSkipIPSetup.Text = "Skip IP Setup";
            this.chkSkipIPSetup.UseVisualStyleBackColor = true;
            // 
            // btnMCUIPSetup
            // 
            this.btnMCUIPSetup.Location = new System.Drawing.Point(398, 55);
            this.btnMCUIPSetup.Name = "btnMCUIPSetup";
            this.btnMCUIPSetup.Size = new System.Drawing.Size(75, 34);
            this.btnMCUIPSetup.TabIndex = 29;
            this.btnMCUIPSetup.Text = "MCU IP Setup";
            this.btnMCUIPSetup.UseVisualStyleBackColor = true;
            this.btnMCUIPSetup.Click += new System.EventHandler(this.btnMCUIPSetup_Click);
            // 
            // chkPlayerDelayTime
            // 
            this.chkPlayerDelayTime.AutoSize = true;
            this.chkPlayerDelayTime.Location = new System.Drawing.Point(395, 202);
            this.chkPlayerDelayTime.Name = "chkPlayerDelayTime";
            this.chkPlayerDelayTime.Size = new System.Drawing.Size(85, 17);
            this.chkPlayerDelayTime.TabIndex = 30;
            this.chkPlayerDelayTime.Text = "Delay Player";
            this.chkPlayerDelayTime.UseVisualStyleBackColor = true;
            this.chkPlayerDelayTime.CheckedChanged += new System.EventHandler(this.chkPlayerDelayTime_CheckedChanged);
            // 
            // txtPlayerDelayTime
            // 
            this.txtPlayerDelayTime.Enabled = false;
            this.txtPlayerDelayTime.Location = new System.Drawing.Point(400, 222);
            this.txtPlayerDelayTime.Name = "txtPlayerDelayTime";
            this.txtPlayerDelayTime.Size = new System.Drawing.Size(67, 20);
            this.txtPlayerDelayTime.TabIndex = 31;
            // 
            // LEDLightingComposerCS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.txtPlayerDelayTime);
            this.Controls.Add(this.chkPlayerDelayTime);
            this.Controls.Add(this.btnMCUIPSetup);
            this.Controls.Add(this.chkSkipIPSetup);
            this.Controls.Add(this.chkSynchronizeMCUs);
            this.Controls.Add(this.pnlDraw);
            this.Controls.Add(this.pnlTBar);
            this.Controls.Add(this.btnSend2LocalFile);
            this.Controls.Add(this.btnOpenProject);
            this.Controls.Add(this.btnClearGrid);
            this.Controls.Add(this.lblProjectName);
            this.Controls.Add(this.btnJump2Secs);
            this.Controls.Add(this.txtTimer);
            this.Controls.Add(this.WMPlayer);
            this.Controls.Add(this.btnLoadSong);
            this.Controls.Add(this.btnEditRecord);
            this.Controls.Add(this.btnSend2SDCard);
            this.Controls.Add(this.lblSongName);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSendViaHTTP);
            this.Controls.Add(this.btnAdd2Project);
            this.Controls.Add(this.lblProjectDatabase);
            this.Controls.Add(this.lblAudioControls);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.dgvProjectData);
            this.Name = "LEDLightingComposerCS";
            this.Text = "DotStar Effects Composer GUI";
            this.Resize += new System.EventHandler(this.LEDLightingComposerCS_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProjectData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.WMPlayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tBarTimer)).EndInit();
            this.pnlTBar.ResumeLayout(false);
            this.pnlTBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvProjectData;
        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Label lblAudioControls;
        private System.Windows.Forms.Label lblProjectDatabase;
        private System.Windows.Forms.Button btnAdd2Project;
        private System.Windows.Forms.Button btnSendViaHTTP;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblSongName;
        private System.Windows.Forms.Button btnSend2SDCard;
        private System.Windows.Forms.Button btnEditRecord;
        private System.Windows.Forms.Button btnLoadSong;
        private AxWMPLib.AxWindowsMediaPlayer WMPlayer;
        private System.Windows.Forms.TextBox txtTimer;
        private System.Windows.Forms.Button btnJump2Secs;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.Button btnClearGrid;
        private System.Windows.Forms.Button btnOpenProject;
        private System.Windows.Forms.Button btnSend2LocalFile;
        private System.Windows.Forms.TrackBar tBarTimer;
        private System.Windows.Forms.Panel pnlTBar;
        private System.Windows.Forms.Panel pnlDraw;
        private System.Windows.Forms.CheckBox chkSynchronizeMCUs;
        private System.Windows.Forms.CheckBox chkSkipIPSetup;
        private System.Windows.Forms.Button btnMCUIPSetup;
        private System.Windows.Forms.CheckBox chkPlayerDelayTime;
        private System.Windows.Forms.TextBox txtPlayerDelayTime;
    }
}

