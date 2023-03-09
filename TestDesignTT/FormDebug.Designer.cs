﻿namespace TestDesignTT
{
    partial class FormDebug
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
            this.panelSideMenu = new System.Windows.Forms.Panel();
            this.btnMultiTurnouts = new FontAwesome.Sharp.IconButton();
            this.btnTurnouts = new FontAwesome.Sharp.IconButton();
            this.btnExit = new FontAwesome.Sharp.IconButton();
            this.btnSections = new FontAwesome.Sharp.IconButton();
            this.btnAddLoco = new FontAwesome.Sharp.IconButton();
            this.btnHome = new FontAwesome.Sharp.IconButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelTitle = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelDesktopPanel = new System.Windows.Forms.Panel();
            this.btnUpdateJson = new FontAwesome.Sharp.IconButton();
            this.btnCentralStop = new FontAwesome.Sharp.IconButton();
            this.panelSideMenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelTitle.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSideMenu
            // 
            this.panelSideMenu.AutoScroll = true;
            this.panelSideMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(87)))), ((int)(((byte)(207)))));
            this.panelSideMenu.Controls.Add(this.btnCentralStop);
            this.panelSideMenu.Controls.Add(this.btnUpdateJson);
            this.panelSideMenu.Controls.Add(this.btnMultiTurnouts);
            this.panelSideMenu.Controls.Add(this.btnTurnouts);
            this.panelSideMenu.Controls.Add(this.btnExit);
            this.panelSideMenu.Controls.Add(this.btnSections);
            this.panelSideMenu.Controls.Add(this.btnAddLoco);
            this.panelSideMenu.Controls.Add(this.btnHome);
            this.panelSideMenu.Controls.Add(this.panel1);
            this.panelSideMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSideMenu.Location = new System.Drawing.Point(0, 0);
            this.panelSideMenu.Name = "panelSideMenu";
            this.panelSideMenu.Size = new System.Drawing.Size(250, 861);
            this.panelSideMenu.TabIndex = 1;
            // 
            // btnMultiTurnouts
            // 
            this.btnMultiTurnouts.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMultiTurnouts.FlatAppearance.BorderSize = 0;
            this.btnMultiTurnouts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMultiTurnouts.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnMultiTurnouts.ForeColor = System.Drawing.Color.White;
            this.btnMultiTurnouts.IconChar = FontAwesome.Sharp.IconChar.ArrowsUpToLine;
            this.btnMultiTurnouts.IconColor = System.Drawing.Color.Red;
            this.btnMultiTurnouts.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnMultiTurnouts.IconSize = 32;
            this.btnMultiTurnouts.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMultiTurnouts.Location = new System.Drawing.Point(0, 270);
            this.btnMultiTurnouts.Name = "btnMultiTurnouts";
            this.btnMultiTurnouts.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnMultiTurnouts.Size = new System.Drawing.Size(250, 50);
            this.btnMultiTurnouts.TabIndex = 20;
            this.btnMultiTurnouts.Text = "Multi turnouts";
            this.btnMultiTurnouts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMultiTurnouts.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnMultiTurnouts.UseVisualStyleBackColor = true;
            this.btnMultiTurnouts.Click += new System.EventHandler(this.btnMultiTurnouts_Click);
            // 
            // btnTurnouts
            // 
            this.btnTurnouts.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTurnouts.FlatAppearance.BorderSize = 0;
            this.btnTurnouts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTurnouts.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnTurnouts.ForeColor = System.Drawing.Color.White;
            this.btnTurnouts.IconChar = FontAwesome.Sharp.IconChar.ArrowsSplitUpAndLeft;
            this.btnTurnouts.IconColor = System.Drawing.Color.Red;
            this.btnTurnouts.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTurnouts.IconSize = 32;
            this.btnTurnouts.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTurnouts.Location = new System.Drawing.Point(0, 220);
            this.btnTurnouts.Name = "btnTurnouts";
            this.btnTurnouts.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnTurnouts.Size = new System.Drawing.Size(250, 50);
            this.btnTurnouts.TabIndex = 19;
            this.btnTurnouts.Text = "Turnouts";
            this.btnTurnouts.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTurnouts.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTurnouts.UseVisualStyleBackColor = true;
            this.btnTurnouts.Click += new System.EventHandler(this.btnTurnouts_Click);
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
            this.btnSections.Location = new System.Drawing.Point(0, 170);
            this.btnSections.Name = "btnSections";
            this.btnSections.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnSections.Size = new System.Drawing.Size(250, 50);
            this.btnSections.TabIndex = 17;
            this.btnSections.Text = "Sections";
            this.btnSections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSections.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSections.UseVisualStyleBackColor = true;
            this.btnSections.Click += new System.EventHandler(this.btnSections_Click);
            // 
            // btnAddLoco
            // 
            this.btnAddLoco.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAddLoco.FlatAppearance.BorderSize = 0;
            this.btnAddLoco.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLoco.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAddLoco.ForeColor = System.Drawing.Color.White;
            this.btnAddLoco.IconChar = FontAwesome.Sharp.IconChar.Train;
            this.btnAddLoco.IconColor = System.Drawing.Color.Red;
            this.btnAddLoco.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAddLoco.IconSize = 32;
            this.btnAddLoco.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddLoco.Location = new System.Drawing.Point(0, 120);
            this.btnAddLoco.Name = "btnAddLoco";
            this.btnAddLoco.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnAddLoco.Size = new System.Drawing.Size(250, 50);
            this.btnAddLoco.TabIndex = 16;
            this.btnAddLoco.Text = "Locomotives";
            this.btnAddLoco.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddLoco.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddLoco.UseVisualStyleBackColor = true;
            this.btnAddLoco.Click += new System.EventHandler(this.btnAddLoco_Click);
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
            this.panelTitle.TabIndex = 2;
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
            this.panelDesktopPanel.TabIndex = 3;
            // 
            // btnUpdateJson
            // 
            this.btnUpdateJson.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnUpdateJson.FlatAppearance.BorderSize = 0;
            this.btnUpdateJson.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateJson.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnUpdateJson.ForeColor = System.Drawing.Color.White;
            this.btnUpdateJson.IconChar = FontAwesome.Sharp.IconChar.Java;
            this.btnUpdateJson.IconColor = System.Drawing.Color.Red;
            this.btnUpdateJson.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnUpdateJson.IconSize = 32;
            this.btnUpdateJson.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdateJson.Location = new System.Drawing.Point(0, 320);
            this.btnUpdateJson.Name = "btnUpdateJson";
            this.btnUpdateJson.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnUpdateJson.Size = new System.Drawing.Size(250, 50);
            this.btnUpdateJson.TabIndex = 21;
            this.btnUpdateJson.Text = "Update JSON";
            this.btnUpdateJson.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUpdateJson.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpdateJson.UseVisualStyleBackColor = true;
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
            this.btnCentralStop.Location = new System.Drawing.Point(0, 370);
            this.btnCentralStop.Name = "btnCentralStop";
            this.btnCentralStop.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnCentralStop.Size = new System.Drawing.Size(250, 50);
            this.btnCentralStop.TabIndex = 22;
            this.btnCentralStop.Text = "Central stop";
            this.btnCentralStop.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCentralStop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCentralStop.UseVisualStyleBackColor = true;
            this.btnCentralStop.Click += new System.EventHandler(this.btnCentralStop_Click);
            // 
            // FormDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.panelDesktopPanel);
            this.Controls.Add(this.panelTitle);
            this.Controls.Add(this.panelSideMenu);
            this.MinimumSize = new System.Drawing.Size(1200, 900);
            this.Name = "FormDebug";
            this.Text = "FormDebug";
            this.Load += new System.EventHandler(this.FormDebug_Load);
            this.SizeChanged += new System.EventHandler(this.FormDebug_SizeChanged);
            this.Resize += new System.EventHandler(this.FormDebug_Resize);
            this.panelSideMenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelTitle.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel panelSideMenu;
        private FontAwesome.Sharp.IconButton btnExit;
        private FontAwesome.Sharp.IconButton btnSections;
        private FontAwesome.Sharp.IconButton btnAddLoco;
        private FontAwesome.Sharp.IconButton btnHome;
        private Panel panel1;
        private Label label1;
        private Panel panelTitle;
        private Label labelTitle;
        private Panel panelDesktopPanel;
        private FontAwesome.Sharp.IconButton btnTurnouts;
        private FontAwesome.Sharp.IconButton btnMultiTurnouts;
        private FontAwesome.Sharp.IconButton btnCentralStop;
        private FontAwesome.Sharp.IconButton btnUpdateJson;
    }
}