namespace TestDesignTT
{
    partial class FormTimetable
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelSideMenu = new System.Windows.Forms.Panel();
            this.btnCentralStop = new FontAwesome.Sharp.IconButton();
            this.btnPlay = new FontAwesome.Sharp.IconButton();
            this.panelSettings = new System.Windows.Forms.Panel();
            this.btnTurnoutSettings = new FontAwesome.Sharp.IconButton();
            this.btnUnitSettings = new FontAwesome.Sharp.IconButton();
            this.btnSettings = new FontAwesome.Sharp.IconButton();
            this.btnDisplayTimetable = new FontAwesome.Sharp.IconButton();
            this.btnLoadTimetable = new FontAwesome.Sharp.IconButton();
            this.btnJSON = new FontAwesome.Sharp.IconButton();
            this.btnSections = new FontAwesome.Sharp.IconButton();
            this.btnExit = new FontAwesome.Sharp.IconButton();
            this.btnHome = new FontAwesome.Sharp.IconButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelDesktopPanel = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelSideMenu.SuspendLayout();
            this.panelSettings.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSideMenu
            // 
            this.panelSideMenu.AutoScroll = true;
            this.panelSideMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(87)))), ((int)(((byte)(207)))));
            this.panelSideMenu.Controls.Add(this.btnCentralStop);
            this.panelSideMenu.Controls.Add(this.btnPlay);
            this.panelSideMenu.Controls.Add(this.panelSettings);
            this.panelSideMenu.Controls.Add(this.btnSettings);
            this.panelSideMenu.Controls.Add(this.btnDisplayTimetable);
            this.panelSideMenu.Controls.Add(this.btnLoadTimetable);
            this.panelSideMenu.Controls.Add(this.btnJSON);
            this.panelSideMenu.Controls.Add(this.btnSections);
            this.panelSideMenu.Controls.Add(this.btnExit);
            this.panelSideMenu.Controls.Add(this.btnHome);
            this.panelSideMenu.Controls.Add(this.panel1);
            this.panelSideMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSideMenu.Location = new System.Drawing.Point(0, 0);
            this.panelSideMenu.Name = "panelSideMenu";
            this.panelSideMenu.Size = new System.Drawing.Size(250, 861);
            this.panelSideMenu.TabIndex = 0;
            // 
            // btnCentralStop
            // 
            this.btnCentralStop.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCentralStop.FlatAppearance.BorderSize = 0;
            this.btnCentralStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCentralStop.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnCentralStop.ForeColor = System.Drawing.Color.White;
            this.btnCentralStop.IconChar = FontAwesome.Sharp.IconChar.Stop;
            this.btnCentralStop.IconColor = System.Drawing.Color.Red;
            this.btnCentralStop.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCentralStop.IconSize = 32;
            this.btnCentralStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCentralStop.Location = new System.Drawing.Point(0, 504);
            this.btnCentralStop.Name = "btnCentralStop";
            this.btnCentralStop.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnCentralStop.Size = new System.Drawing.Size(250, 50);
            this.btnCentralStop.TabIndex = 27;
            this.btnCentralStop.Text = "Central stop";
            this.btnCentralStop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCentralStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCentralStop.UseVisualStyleBackColor = true;
            this.btnCentralStop.Click += new System.EventHandler(this.btnCentralStop_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnPlay.ForeColor = System.Drawing.Color.White;
            this.btnPlay.IconChar = FontAwesome.Sharp.IconChar.Play;
            this.btnPlay.IconColor = System.Drawing.Color.Red;
            this.btnPlay.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnPlay.IconSize = 32;
            this.btnPlay.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlay.Location = new System.Drawing.Point(0, 454);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnPlay.Size = new System.Drawing.Size(250, 50);
            this.btnPlay.TabIndex = 26;
            this.btnPlay.Text = "Play";
            this.btnPlay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // panelSettings
            // 
            this.panelSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(27)))), ((int)(((byte)(207)))));
            this.panelSettings.Controls.Add(this.btnTurnoutSettings);
            this.panelSettings.Controls.Add(this.btnUnitSettings);
            this.panelSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSettings.Location = new System.Drawing.Point(0, 370);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(250, 84);
            this.panelSettings.TabIndex = 25;
            // 
            // btnTurnoutSettings
            // 
            this.btnTurnoutSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTurnoutSettings.FlatAppearance.BorderSize = 0;
            this.btnTurnoutSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTurnoutSettings.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnTurnoutSettings.ForeColor = System.Drawing.Color.White;
            this.btnTurnoutSettings.IconChar = FontAwesome.Sharp.IconChar.ArrowsSplitUpAndLeft;
            this.btnTurnoutSettings.IconColor = System.Drawing.Color.Red;
            this.btnTurnoutSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTurnoutSettings.IconSize = 25;
            this.btnTurnoutSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTurnoutSettings.Location = new System.Drawing.Point(0, 40);
            this.btnTurnoutSettings.Name = "btnTurnoutSettings";
            this.btnTurnoutSettings.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnTurnoutSettings.Size = new System.Drawing.Size(250, 40);
            this.btnTurnoutSettings.TabIndex = 6;
            this.btnTurnoutSettings.Text = "Turnout Unit";
            this.btnTurnoutSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTurnoutSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTurnoutSettings.UseVisualStyleBackColor = true;
            this.btnTurnoutSettings.Click += new System.EventHandler(this.btnTurnoutSettings_Click);
            // 
            // btnUnitSettings
            // 
            this.btnUnitSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUnitSettings.FlatAppearance.BorderSize = 0;
            this.btnUnitSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnitSettings.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnUnitSettings.ForeColor = System.Drawing.Color.White;
            this.btnUnitSettings.IconChar = FontAwesome.Sharp.IconChar.Info;
            this.btnUnitSettings.IconColor = System.Drawing.Color.Red;
            this.btnUnitSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnUnitSettings.IconSize = 25;
            this.btnUnitSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUnitSettings.Location = new System.Drawing.Point(0, 0);
            this.btnUnitSettings.Name = "btnUnitSettings";
            this.btnUnitSettings.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnUnitSettings.Size = new System.Drawing.Size(250, 40);
            this.btnUnitSettings.TabIndex = 5;
            this.btnUnitSettings.Text = "Instruction Unit";
            this.btnUnitSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUnitSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUnitSettings.UseVisualStyleBackColor = true;
            this.btnUnitSettings.Click += new System.EventHandler(this.btnUnitSettings_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSettings.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSettings.ForeColor = System.Drawing.Color.White;
            this.btnSettings.IconChar = FontAwesome.Sharp.IconChar.Sliders;
            this.btnSettings.IconColor = System.Drawing.Color.Red;
            this.btnSettings.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSettings.IconSize = 32;
            this.btnSettings.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSettings.Location = new System.Drawing.Point(0, 320);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnSettings.Size = new System.Drawing.Size(250, 50);
            this.btnSettings.TabIndex = 24;
            this.btnSettings.Text = "Settings";
            this.btnSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSettings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnDisplayTimetable
            // 
            this.btnDisplayTimetable.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDisplayTimetable.FlatAppearance.BorderSize = 0;
            this.btnDisplayTimetable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisplayTimetable.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnDisplayTimetable.ForeColor = System.Drawing.Color.White;
            this.btnDisplayTimetable.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.btnDisplayTimetable.IconColor = System.Drawing.Color.Red;
            this.btnDisplayTimetable.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDisplayTimetable.IconSize = 32;
            this.btnDisplayTimetable.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDisplayTimetable.Location = new System.Drawing.Point(0, 270);
            this.btnDisplayTimetable.Name = "btnDisplayTimetable";
            this.btnDisplayTimetable.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnDisplayTimetable.Size = new System.Drawing.Size(250, 50);
            this.btnDisplayTimetable.TabIndex = 23;
            this.btnDisplayTimetable.Text = "Display timetable";
            this.btnDisplayTimetable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDisplayTimetable.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDisplayTimetable.UseVisualStyleBackColor = true;
            this.btnDisplayTimetable.Click += new System.EventHandler(this.btnDisplayTimetable_Click);
            // 
            // btnLoadTimetable
            // 
            this.btnLoadTimetable.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLoadTimetable.FlatAppearance.BorderSize = 0;
            this.btnLoadTimetable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadTimetable.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLoadTimetable.ForeColor = System.Drawing.Color.White;
            this.btnLoadTimetable.IconChar = FontAwesome.Sharp.IconChar.Readme;
            this.btnLoadTimetable.IconColor = System.Drawing.Color.Red;
            this.btnLoadTimetable.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLoadTimetable.IconSize = 32;
            this.btnLoadTimetable.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadTimetable.Location = new System.Drawing.Point(0, 220);
            this.btnLoadTimetable.Name = "btnLoadTimetable";
            this.btnLoadTimetable.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnLoadTimetable.Size = new System.Drawing.Size(250, 50);
            this.btnLoadTimetable.TabIndex = 22;
            this.btnLoadTimetable.Text = "Load timetable";
            this.btnLoadTimetable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadTimetable.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoadTimetable.UseVisualStyleBackColor = true;
            this.btnLoadTimetable.Click += new System.EventHandler(this.btnLoadTimetable_Click);
            // 
            // btnJSON
            // 
            this.btnJSON.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnJSON.FlatAppearance.BorderSize = 0;
            this.btnJSON.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJSON.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnJSON.ForeColor = System.Drawing.Color.White;
            this.btnJSON.IconChar = FontAwesome.Sharp.IconChar.Code;
            this.btnJSON.IconColor = System.Drawing.Color.Red;
            this.btnJSON.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnJSON.IconSize = 32;
            this.btnJSON.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnJSON.Location = new System.Drawing.Point(0, 170);
            this.btnJSON.Name = "btnJSON";
            this.btnJSON.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnJSON.Size = new System.Drawing.Size(250, 50);
            this.btnJSON.TabIndex = 20;
            this.btnJSON.Text = "JSON values";
            this.btnJSON.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnJSON.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnJSON.UseVisualStyleBackColor = true;
            this.btnJSON.Click += new System.EventHandler(this.btnJSON_Click);
            // 
            // btnSections
            // 
            this.btnSections.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSections.FlatAppearance.BorderSize = 0;
            this.btnSections.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSections.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSections.ForeColor = System.Drawing.Color.White;
            this.btnSections.IconChar = FontAwesome.Sharp.IconChar.Map;
            this.btnSections.IconColor = System.Drawing.Color.Red;
            this.btnSections.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSections.IconSize = 32;
            this.btnSections.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSections.Location = new System.Drawing.Point(0, 120);
            this.btnSections.Name = "btnSections";
            this.btnSections.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnSections.Size = new System.Drawing.Size(250, 50);
            this.btnSections.TabIndex = 19;
            this.btnSections.Text = "Sections";
            this.btnSections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSections.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSections.UseVisualStyleBackColor = true;
            this.btnSections.Click += new System.EventHandler(this.btnSections_Click);
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.IconChar = FontAwesome.Sharp.IconChar.ArrowRotateBack;
            this.btnExit.IconColor = System.Drawing.Color.Red;
            this.btnExit.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnExit.IconSize = 32;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(0, 811);
            this.btnExit.Name = "btnExit";
            this.btnExit.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnExit.Size = new System.Drawing.Size(250, 50);
            this.btnExit.TabIndex = 18;
            this.btnExit.Text = "Exit";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnHome
            // 
            this.btnHome.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHome.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnHome.ForeColor = System.Drawing.Color.White;
            this.btnHome.IconChar = FontAwesome.Sharp.IconChar.Home;
            this.btnHome.IconColor = System.Drawing.Color.Red;
            this.btnHome.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnHome.IconSize = 32;
            this.btnHome.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHome.Location = new System.Drawing.Point(0, 70);
            this.btnHome.Name = "btnHome";
            this.btnHome.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnHome.Size = new System.Drawing.Size(250, 50);
            this.btnHome.TabIndex = 1;
            this.btnHome.Text = "Home";
            this.btnHome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(250, 70);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Arial Black", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.Info;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 70);
            this.label1.TabIndex = 0;
            this.label1.Text = "TT Trains";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelTitle
            // 
            this.panelTitle.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelTitle.Controls.Add(this.labelTitle);
            this.panelTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitle.Location = new System.Drawing.Point(250, 0);
            this.panelTitle.Name = "panelTitle";
            this.panelTitle.Size = new System.Drawing.Size(934, 70);
            this.panelTitle.TabIndex = 1;
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(87)))), ((int)(((byte)(207)))));
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("Bodoni MT Black", 35F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(934, 70);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Home";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelDesktopPanel
            // 
            this.panelDesktopPanel.AutoSize = true;
            this.panelDesktopPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDesktopPanel.Location = new System.Drawing.Point(250, 70);
            this.panelDesktopPanel.Name = "panelDesktopPanel";
            this.panelDesktopPanel.Size = new System.Drawing.Size(934, 791);
            this.panelDesktopPanel.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FormTimetable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.panelDesktopPanel);
            this.Controls.Add(this.panelTitle);
            this.Controls.Add(this.panelSideMenu);
            this.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.MinimumSize = new System.Drawing.Size(1200, 900);
            this.Name = "FormTimetable";
            this.Text = "Timetable Control Train";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTimetable_FormClosing);
            this.Load += new System.EventHandler(this.FormMainMenu_Load);
            this.SizeChanged += new System.EventHandler(this.FormMainMenu_SizeChanged);
            this.panelSideMenu.ResumeLayout(false);
            this.panelSettings.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panelTitle.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panelSideMenu;
        private Panel panel1;
        private FontAwesome.Sharp.IconButton btnHome;
        private Label label1;
        private Panel panelTitle;
        private Label labelTitle;
        private Panel panelDesktopPanel;
        private System.Windows.Forms.Timer timer1;
        private FontAwesome.Sharp.IconButton btnExit;
        private FontAwesome.Sharp.IconButton btnSections;
        private FontAwesome.Sharp.IconButton btnJSON;
        private FontAwesome.Sharp.IconButton btnDisplayTimetable;
        private FontAwesome.Sharp.IconButton btnLoadTimetable;
        private FontAwesome.Sharp.IconButton btnSettings;
        private FontAwesome.Sharp.IconButton btnCentralStop;
        private FontAwesome.Sharp.IconButton btnPlay;
        private Panel panelSettings;
        private FontAwesome.Sharp.IconButton btnTurnoutSettings;
        private FontAwesome.Sharp.IconButton btnUnitSettings;
    }
}