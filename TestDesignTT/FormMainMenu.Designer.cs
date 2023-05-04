namespace TestDesignTT
{
    partial class FormMainMenu
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelHeader = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnTimetable = new FontAwesome.Sharp.IconButton();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnManual = new FontAwesome.Sharp.IconButton();
            this.panel7 = new System.Windows.Forms.Panel();
            this.btnDebug = new FontAwesome.Sharp.IconButton();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelHeader);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1184, 100);
            this.panel1.TabIndex = 0;
            // 
            // labelHeader
            // 
            this.labelHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelHeader.Font = new System.Drawing.Font("Bodoni MT Black", 35F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.labelHeader.Location = new System.Drawing.Point(0, 0);
            this.labelHeader.Name = "labelHeader";
            this.labelHeader.Size = new System.Drawing.Size(1184, 100);
            this.labelHeader.TabIndex = 0;
            this.labelHeader.Text = "Choose your prefered mode!";
            this.labelHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1184, 50);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnTimetable);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 150);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1184, 100);
            this.panel3.TabIndex = 2;
            // 
            // btnTimetable
            // 
            this.btnTimetable.BackColor = System.Drawing.Color.Gainsboro;
            this.btnTimetable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnTimetable.FlatAppearance.BorderSize = 0;
            this.btnTimetable.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTimetable.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnTimetable.ForeColor = System.Drawing.Color.White;
            this.btnTimetable.IconChar = FontAwesome.Sharp.IconChar.Calendar;
            this.btnTimetable.IconColor = System.Drawing.Color.Red;
            this.btnTimetable.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTimetable.IconSize = 32;
            this.btnTimetable.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnTimetable.Location = new System.Drawing.Point(0, 0);
            this.btnTimetable.Name = "btnTimetable";
            this.btnTimetable.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnTimetable.Size = new System.Drawing.Size(1184, 100);
            this.btnTimetable.TabIndex = 15;
            this.btnTimetable.Text = "Timetable mode";
            this.btnTimetable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnTimetable.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTimetable.UseVisualStyleBackColor = false;
            this.btnTimetable.Click += new System.EventHandler(this.btnTimetable_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnManual);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 250);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1184, 100);
            this.panel5.TabIndex = 4;
            // 
            // btnManual
            // 
            this.btnManual.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnManual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnManual.FlatAppearance.BorderSize = 0;
            this.btnManual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManual.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnManual.ForeColor = System.Drawing.Color.White;
            this.btnManual.IconChar = FontAwesome.Sharp.IconChar.HandPointer;
            this.btnManual.IconColor = System.Drawing.Color.Red;
            this.btnManual.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnManual.IconSize = 32;
            this.btnManual.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnManual.Location = new System.Drawing.Point(0, 0);
            this.btnManual.Name = "btnManual";
            this.btnManual.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnManual.Size = new System.Drawing.Size(1184, 100);
            this.btnManual.TabIndex = 16;
            this.btnManual.Text = "Manual mode";
            this.btnManual.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnManual.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnManual.UseVisualStyleBackColor = false;
            this.btnManual.Click += new System.EventHandler(this.btnManual_Click);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.btnDebug);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(0, 350);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1184, 100);
            this.panel7.TabIndex = 6;
            // 
            // btnDebug
            // 
            this.btnDebug.BackColor = System.Drawing.Color.Gainsboro;
            this.btnDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDebug.FlatAppearance.BorderSize = 0;
            this.btnDebug.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDebug.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnDebug.ForeColor = System.Drawing.Color.White;
            this.btnDebug.IconChar = FontAwesome.Sharp.IconChar.Train;
            this.btnDebug.IconColor = System.Drawing.Color.Red;
            this.btnDebug.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDebug.IconSize = 32;
            this.btnDebug.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDebug.Location = new System.Drawing.Point(0, 0);
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnDebug.Size = new System.Drawing.Size(1184, 100);
            this.btnDebug.TabIndex = 16;
            this.btnDebug.Text = "Debug Mode";
            this.btnDebug.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDebug.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDebug.UseVisualStyleBackColor = false;
            this.btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
            // 
            // FormMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 861);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(1200, 900);
            this.Name = "FormMainMenu";
            this.Text = "MainMenu";
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label labelHeader;
        private Panel panel2;
        private Panel panel3;
        private Panel panel5;
        private Panel panel7;
        private FontAwesome.Sharp.IconButton btnTimetable;
        private FontAwesome.Sharp.IconButton btnManual;
        private FontAwesome.Sharp.IconButton btnDebug;
    }
}