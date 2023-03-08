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
            this.btnExit = new FontAwesome.Sharp.IconButton();
            this.btnCentralStop = new FontAwesome.Sharp.IconButton();
            this.btnPlay = new FontAwesome.Sharp.IconButton();
            this.panelModifyData = new System.Windows.Forms.Panel();
            this.btnEditTrain = new FontAwesome.Sharp.IconButton();
            this.btnDeleteTrain = new FontAwesome.Sharp.IconButton();
            this.btnAddTrain = new FontAwesome.Sharp.IconButton();
            this.btnModifyData = new FontAwesome.Sharp.IconButton();
            this.panelTimetable = new System.Windows.Forms.Panel();
            this.btnLoadData = new FontAwesome.Sharp.IconButton();
            this.btnDisplayData = new FontAwesome.Sharp.IconButton();
            this.btnTimetable = new FontAwesome.Sharp.IconButton();
            this.btnMovingTrains = new FontAwesome.Sharp.IconButton();
            this.btnHome = new FontAwesome.Sharp.IconButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelDesktopPanel = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelSideMenu.SuspendLayout();
            this.panelModifyData.SuspendLayout();
            this.panelTimetable.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSideMenu
            // 
            this.panelSideMenu.AutoScroll = true;
            this.panelSideMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(87)))), ((int)(((byte)(207)))));
            this.panelSideMenu.Controls.Add(this.btnExit);
            this.panelSideMenu.Controls.Add(this.btnCentralStop);
            this.panelSideMenu.Controls.Add(this.btnPlay);
            this.panelSideMenu.Controls.Add(this.panelModifyData);
            this.panelSideMenu.Controls.Add(this.btnModifyData);
            this.panelSideMenu.Controls.Add(this.panelTimetable);
            this.panelSideMenu.Controls.Add(this.btnTimetable);
            this.panelSideMenu.Controls.Add(this.btnMovingTrains);
            this.panelSideMenu.Controls.Add(this.btnHome);
            this.panelSideMenu.Controls.Add(this.panel1);
            this.panelSideMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSideMenu.Location = new System.Drawing.Point(0, 0);
            this.panelSideMenu.Name = "panelSideMenu";
            this.panelSideMenu.Size = new System.Drawing.Size(250, 861);
            this.panelSideMenu.TabIndex = 0;
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
            this.btnCentralStop.Location = new System.Drawing.Point(0, 529);
            this.btnCentralStop.Name = "btnCentralStop";
            this.btnCentralStop.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnCentralStop.Size = new System.Drawing.Size(250, 50);
            this.btnCentralStop.TabIndex = 17;
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
            this.btnPlay.Location = new System.Drawing.Point(0, 479);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnPlay.Size = new System.Drawing.Size(250, 50);
            this.btnPlay.TabIndex = 16;
            this.btnPlay.Text = "Play";
            this.btnPlay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // panelModifyData
            // 
            this.panelModifyData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(27)))), ((int)(((byte)(207)))));
            this.panelModifyData.Controls.Add(this.btnEditTrain);
            this.panelModifyData.Controls.Add(this.btnDeleteTrain);
            this.panelModifyData.Controls.Add(this.btnAddTrain);
            this.panelModifyData.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelModifyData.Location = new System.Drawing.Point(0, 354);
            this.panelModifyData.Name = "panelModifyData";
            this.panelModifyData.Size = new System.Drawing.Size(250, 125);
            this.panelModifyData.TabIndex = 15;
            // 
            // btnEditTrain
            // 
            this.btnEditTrain.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEditTrain.FlatAppearance.BorderSize = 0;
            this.btnEditTrain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditTrain.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnEditTrain.ForeColor = System.Drawing.Color.White;
            this.btnEditTrain.IconChar = FontAwesome.Sharp.IconChar.Database;
            this.btnEditTrain.IconColor = System.Drawing.Color.Red;
            this.btnEditTrain.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnEditTrain.IconSize = 25;
            this.btnEditTrain.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditTrain.Location = new System.Drawing.Point(0, 80);
            this.btnEditTrain.Name = "btnEditTrain";
            this.btnEditTrain.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnEditTrain.Size = new System.Drawing.Size(250, 40);
            this.btnEditTrain.TabIndex = 13;
            this.btnEditTrain.Text = "Timetable edit";
            this.btnEditTrain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditTrain.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEditTrain.UseVisualStyleBackColor = true;
            this.btnEditTrain.Click += new System.EventHandler(this.btnEditTimetable_Click);
            // 
            // btnDeleteTrain
            // 
            this.btnDeleteTrain.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDeleteTrain.FlatAppearance.BorderSize = 0;
            this.btnDeleteTrain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteTrain.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnDeleteTrain.ForeColor = System.Drawing.Color.White;
            this.btnDeleteTrain.IconChar = FontAwesome.Sharp.IconChar.Tram;
            this.btnDeleteTrain.IconColor = System.Drawing.Color.Red;
            this.btnDeleteTrain.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDeleteTrain.IconSize = 25;
            this.btnDeleteTrain.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteTrain.Location = new System.Drawing.Point(0, 40);
            this.btnDeleteTrain.Name = "btnDeleteTrain";
            this.btnDeleteTrain.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnDeleteTrain.Size = new System.Drawing.Size(250, 40);
            this.btnDeleteTrain.TabIndex = 12;
            this.btnDeleteTrain.Text = "Train edit";
            this.btnDeleteTrain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteTrain.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDeleteTrain.UseVisualStyleBackColor = true;
            this.btnDeleteTrain.Click += new System.EventHandler(this.btnEditTrain_Click);
            // 
            // btnAddTrain
            // 
            this.btnAddTrain.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAddTrain.FlatAppearance.BorderSize = 0;
            this.btnAddTrain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddTrain.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAddTrain.ForeColor = System.Drawing.Color.White;
            this.btnAddTrain.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnAddTrain.IconColor = System.Drawing.Color.Red;
            this.btnAddTrain.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAddTrain.IconSize = 25;
            this.btnAddTrain.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddTrain.Location = new System.Drawing.Point(0, 0);
            this.btnAddTrain.Name = "btnAddTrain";
            this.btnAddTrain.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnAddTrain.Size = new System.Drawing.Size(250, 40);
            this.btnAddTrain.TabIndex = 11;
            this.btnAddTrain.Text = "Add train";
            this.btnAddTrain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddTrain.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddTrain.UseVisualStyleBackColor = true;
            this.btnAddTrain.Click += new System.EventHandler(this.btnAddTrain_Click);
            // 
            // btnModifyData
            // 
            this.btnModifyData.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnModifyData.FlatAppearance.BorderSize = 0;
            this.btnModifyData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModifyData.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnModifyData.ForeColor = System.Drawing.Color.White;
            this.btnModifyData.IconChar = FontAwesome.Sharp.IconChar.T;
            this.btnModifyData.IconColor = System.Drawing.Color.Red;
            this.btnModifyData.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnModifyData.IconSize = 32;
            this.btnModifyData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnModifyData.Location = new System.Drawing.Point(0, 304);
            this.btnModifyData.Name = "btnModifyData";
            this.btnModifyData.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnModifyData.Size = new System.Drawing.Size(250, 50);
            this.btnModifyData.TabIndex = 14;
            this.btnModifyData.Text = "Modify Data";
            this.btnModifyData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnModifyData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnModifyData.UseVisualStyleBackColor = true;
            this.btnModifyData.Click += new System.EventHandler(this.btnModifyData_Click);
            // 
            // panelTimetable
            // 
            this.panelTimetable.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(27)))), ((int)(((byte)(207)))));
            this.panelTimetable.Controls.Add(this.btnLoadData);
            this.panelTimetable.Controls.Add(this.btnDisplayData);
            this.panelTimetable.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTimetable.Location = new System.Drawing.Point(0, 220);
            this.panelTimetable.Name = "panelTimetable";
            this.panelTimetable.Size = new System.Drawing.Size(250, 84);
            this.panelTimetable.TabIndex = 8;
            // 
            // btnLoadData
            // 
            this.btnLoadData.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnLoadData.FlatAppearance.BorderSize = 0;
            this.btnLoadData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadData.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLoadData.ForeColor = System.Drawing.Color.White;
            this.btnLoadData.IconChar = FontAwesome.Sharp.IconChar.Readme;
            this.btnLoadData.IconColor = System.Drawing.Color.Red;
            this.btnLoadData.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLoadData.IconSize = 25;
            this.btnLoadData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadData.Location = new System.Drawing.Point(0, 40);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnLoadData.Size = new System.Drawing.Size(250, 40);
            this.btnLoadData.TabIndex = 6;
            this.btnLoadData.Text = "Load data";
            this.btnLoadData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoadData.UseVisualStyleBackColor = true;
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            // 
            // btnDisplayData
            // 
            this.btnDisplayData.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDisplayData.FlatAppearance.BorderSize = 0;
            this.btnDisplayData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDisplayData.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnDisplayData.ForeColor = System.Drawing.Color.White;
            this.btnDisplayData.IconChar = FontAwesome.Sharp.IconChar.Eye;
            this.btnDisplayData.IconColor = System.Drawing.Color.Red;
            this.btnDisplayData.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDisplayData.IconSize = 25;
            this.btnDisplayData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDisplayData.Location = new System.Drawing.Point(0, 0);
            this.btnDisplayData.Name = "btnDisplayData";
            this.btnDisplayData.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnDisplayData.Size = new System.Drawing.Size(250, 40);
            this.btnDisplayData.TabIndex = 5;
            this.btnDisplayData.Text = "Display data";
            this.btnDisplayData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDisplayData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDisplayData.UseVisualStyleBackColor = true;
            this.btnDisplayData.Click += new System.EventHandler(this.btnDisplayTimetable_Click);
            // 
            // btnTimetable
            // 
            this.btnTimetable.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTimetable.FlatAppearance.BorderSize = 0;
            this.btnTimetable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTimetable.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnTimetable.ForeColor = System.Drawing.Color.White;
            this.btnTimetable.IconChar = FontAwesome.Sharp.IconChar.CalendarTimes;
            this.btnTimetable.IconColor = System.Drawing.Color.Red;
            this.btnTimetable.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTimetable.IconSize = 32;
            this.btnTimetable.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTimetable.Location = new System.Drawing.Point(0, 170);
            this.btnTimetable.Name = "btnTimetable";
            this.btnTimetable.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnTimetable.Size = new System.Drawing.Size(250, 50);
            this.btnTimetable.TabIndex = 3;
            this.btnTimetable.Text = "Timetable";
            this.btnTimetable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTimetable.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTimetable.UseVisualStyleBackColor = true;
            this.btnTimetable.Click += new System.EventHandler(this.btnTimetable_Click);
            // 
            // btnMovingTrains
            // 
            this.btnMovingTrains.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMovingTrains.FlatAppearance.BorderSize = 0;
            this.btnMovingTrains.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMovingTrains.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnMovingTrains.ForeColor = System.Drawing.Color.White;
            this.btnMovingTrains.IconChar = FontAwesome.Sharp.IconChar.Train;
            this.btnMovingTrains.IconColor = System.Drawing.Color.Red;
            this.btnMovingTrains.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnMovingTrains.IconSize = 32;
            this.btnMovingTrains.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMovingTrains.Location = new System.Drawing.Point(0, 120);
            this.btnMovingTrains.Name = "btnMovingTrains";
            this.btnMovingTrains.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnMovingTrains.Size = new System.Drawing.Size(250, 50);
            this.btnMovingTrains.TabIndex = 2;
            this.btnMovingTrains.Text = "Moving trains";
            this.btnMovingTrains.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMovingTrains.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMovingTrains.UseVisualStyleBackColor = true;
            this.btnMovingTrains.Click += new System.EventHandler(this.btnMovingTrain_Click);
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
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Arial Black", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.Info;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 42);
            this.label1.TabIndex = 0;
            this.label1.Text = "TT Trains KAE";
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
            this.Load += new System.EventHandler(this.FormMainMenu_Load);
            this.SizeChanged += new System.EventHandler(this.FormMainMenu_SizeChanged);
            this.panelSideMenu.ResumeLayout(false);
            this.panelModifyData.ResumeLayout(false);
            this.panelTimetable.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelTitle.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panelSideMenu;
        private Panel panel1;
        private FontAwesome.Sharp.IconButton btnMovingTrains;
        private FontAwesome.Sharp.IconButton btnHome;
        private FontAwesome.Sharp.IconButton btnTimetable;
        private FontAwesome.Sharp.IconButton btnDisplayData;
        private FontAwesome.Sharp.IconButton btnLoadData;
        private Panel panelTimetable;
        private FontAwesome.Sharp.IconButton btnCentralStop;
        private FontAwesome.Sharp.IconButton btnPlay;
        private Panel panelModifyData;
        private FontAwesome.Sharp.IconButton btnEditTrain;
        private FontAwesome.Sharp.IconButton btnDeleteTrain;
        private FontAwesome.Sharp.IconButton btnAddTrain;
        private FontAwesome.Sharp.IconButton btnModifyData;
        private Label label1;
        private Panel panelTitle;
        private Label labelTitle;
        private Panel panelDesktopPanel;
        private System.Windows.Forms.Timer timer1;
        private FontAwesome.Sharp.IconButton btnExit;
    }
}