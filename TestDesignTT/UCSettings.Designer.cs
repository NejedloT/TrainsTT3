namespace TestDesignTT
{
    partial class UCSettings
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnTurnoutSetting = new FontAwesome.Sharp.IconButton();
            this.btnUnitSetting = new FontAwesome.Sharp.IconButton();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(825, 100);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(825, 100);
            this.label1.TabIndex = 0;
            this.label1.Text = "Which settings do you want to change?";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(825, 30);
            this.panel2.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnTurnoutSetting, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnUnitSetting, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 130);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(825, 452);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // btnTurnoutSetting
            // 
            this.btnTurnoutSetting.AutoSize = true;
            this.btnTurnoutSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnTurnoutSetting.FlatAppearance.BorderSize = 0;
            this.btnTurnoutSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTurnoutSetting.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnTurnoutSetting.ForeColor = System.Drawing.Color.White;
            this.btnTurnoutSetting.IconChar = FontAwesome.Sharp.IconChar.ArrowsSplitUpAndLeft;
            this.btnTurnoutSetting.IconColor = System.Drawing.Color.Red;
            this.btnTurnoutSetting.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnTurnoutSetting.IconSize = 25;
            this.btnTurnoutSetting.Location = new System.Drawing.Point(240, 63);
            this.btnTurnoutSetting.Name = "btnTurnoutSetting";
            this.btnTurnoutSetting.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnTurnoutSetting.Size = new System.Drawing.Size(344, 54);
            this.btnTurnoutSetting.TabIndex = 16;
            this.btnTurnoutSetting.Text = "Turnout Instruction Setting";
            this.btnTurnoutSetting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnTurnoutSetting.UseVisualStyleBackColor = true;
            this.btnTurnoutSetting.Click += new System.EventHandler(this.btnTurnoutSetting_Click);
            // 
            // btnUnitSetting
            // 
            this.btnUnitSetting.AutoSize = true;
            this.btnUnitSetting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUnitSetting.FlatAppearance.BorderSize = 0;
            this.btnUnitSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnitSetting.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnUnitSetting.ForeColor = System.Drawing.Color.White;
            this.btnUnitSetting.IconChar = FontAwesome.Sharp.IconChar.Info;
            this.btnUnitSetting.IconColor = System.Drawing.Color.Red;
            this.btnUnitSetting.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnUnitSetting.IconSize = 25;
            this.btnUnitSetting.Location = new System.Drawing.Point(240, 3);
            this.btnUnitSetting.Name = "btnUnitSetting";
            this.btnUnitSetting.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnUnitSetting.Size = new System.Drawing.Size(344, 54);
            this.btnUnitSetting.TabIndex = 15;
            this.btnUnitSetting.Text = "Unit Instruction Setting";
            this.btnUnitSetting.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUnitSetting.UseVisualStyleBackColor = true;
            this.btnUnitSetting.Click += new System.EventHandler(this.btnUnitSetting_Click);
            // 
            // UCSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "UCSettings";
            this.Size = new System.Drawing.Size(825, 582);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Panel panel2;
        private TableLayoutPanel tableLayoutPanel1;
        private FontAwesome.Sharp.IconButton btnTurnoutSetting;
        private FontAwesome.Sharp.IconButton btnUnitSetting;
    }
}
