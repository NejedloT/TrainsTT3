namespace TestDesignTT
{
    partial class UCDataLoad
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
            this.panelLDTitle = new System.Windows.Forms.Panel();
            this.labelLDTitle = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.btnLoadDataPath = new FontAwesome.Sharp.IconButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panelLoadedData1 = new System.Windows.Forms.Panel();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelFileName = new System.Windows.Forms.Label();
            this.btnLoadDataPickFile = new FontAwesome.Sharp.IconButton();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.rbInfinity = new System.Windows.Forms.RadioButton();
            this.rbPauses = new System.Windows.Forms.RadioButton();
            this.panelLDTitle.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLDTitle
            // 
            this.panelLDTitle.Controls.Add(this.labelLDTitle);
            this.panelLDTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLDTitle.Location = new System.Drawing.Point(0, 0);
            this.panelLDTitle.Name = "panelLDTitle";
            this.panelLDTitle.Size = new System.Drawing.Size(779, 100);
            this.panelLDTitle.TabIndex = 0;
            // 
            // labelLDTitle
            // 
            this.labelLDTitle.BackColor = System.Drawing.SystemColors.HotTrack;
            this.labelLDTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLDTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelLDTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelLDTitle.Location = new System.Drawing.Point(0, 0);
            this.labelLDTitle.Name = "labelLDTitle";
            this.labelLDTitle.Size = new System.Drawing.Size(779, 100);
            this.labelLDTitle.TabIndex = 0;
            this.labelLDTitle.Text = "Load your CSV Data!";
            this.labelLDTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxPath
            // 
            this.textBoxPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPath.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.textBoxPath.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.textBoxPath.Location = new System.Drawing.Point(143, 3);
            this.textBoxPath.Multiline = true;
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(493, 44);
            this.textBoxPath.TabIndex = 2;
            this.textBoxPath.Text = "Please enter path to your CSV file...";
            this.textBoxPath.TextChanged += new System.EventHandler(this.textBoxPath_TextChanged);
            this.textBoxPath.Enter += new System.EventHandler(this.textBoxPath_Enter);
            this.textBoxPath.Leave += new System.EventHandler(this.textBoxPath_Leave);
            // 
            // btnLoadDataPath
            // 
            this.btnLoadDataPath.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnLoadDataPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoadDataPath.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLoadDataPath.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoadDataPath.IconChar = FontAwesome.Sharp.IconChar.Share;
            this.btnLoadDataPath.IconColor = System.Drawing.Color.Black;
            this.btnLoadDataPath.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLoadDataPath.IconSize = 35;
            this.btnLoadDataPath.Location = new System.Drawing.Point(642, 3);
            this.btnLoadDataPath.Name = "btnLoadDataPath";
            this.btnLoadDataPath.Size = new System.Drawing.Size(134, 44);
            this.btnLoadDataPath.TabIndex = 1;
            this.btnLoadDataPath.Text = "Find";
            this.btnLoadDataPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadDataPath.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoadDataPath.UseVisualStyleBackColor = false;
            this.btnLoadDataPath.Click += new System.EventHandler(this.btnLoadDataPath_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(1, 1);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 48);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Path:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelLoadedData1
            // 
            this.panelLoadedData1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLoadedData1.Location = new System.Drawing.Point(0, 100);
            this.panelLoadedData1.Name = "panelLoadedData1";
            this.panelLoadedData1.Size = new System.Drawing.Size(779, 60);
            this.panelLoadedData1.TabIndex = 3;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel1.Controls.Add(this.labelFileName, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.btnLoadDataPickFile, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnLoadDataPath, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBoxPath, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 160);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(779, 308);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // labelFileName
            // 
            this.labelFileName.BackColor = System.Drawing.SystemColors.HotTrack;
            this.labelFileName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFileName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelFileName.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelFileName.Location = new System.Drawing.Point(143, 210);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(493, 50);
            this.labelFileName.TabIndex = 5;
            // 
            // btnLoadDataPickFile
            // 
            this.btnLoadDataPickFile.BackColor = System.Drawing.SystemColors.HotTrack;
            this.tableLayoutPanel1.SetColumnSpan(this.btnLoadDataPickFile, 3);
            this.btnLoadDataPickFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoadDataPickFile.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLoadDataPickFile.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoadDataPickFile.IconChar = FontAwesome.Sharp.IconChar.Database;
            this.btnLoadDataPickFile.IconColor = System.Drawing.Color.Black;
            this.btnLoadDataPickFile.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLoadDataPickFile.IconSize = 35;
            this.btnLoadDataPickFile.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnLoadDataPickFile.Location = new System.Drawing.Point(3, 73);
            this.btnLoadDataPickFile.Name = "btnLoadDataPickFile";
            this.btnLoadDataPickFile.Size = new System.Drawing.Size(773, 44);
            this.btnLoadDataPickFile.TabIndex = 3;
            this.btnLoadDataPickFile.Text = "Load Data";
            this.btnLoadDataPickFile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoadDataPickFile.UseVisualStyleBackColor = false;
            this.btnLoadDataPickFile.Click += new System.EventHandler(this.btn_LoadDataPickFile_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.rbInfinity, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.rbPauses, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(143, 143);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(493, 44);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // rbInfinity
            // 
            this.rbInfinity.AutoSize = true;
            this.rbInfinity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbInfinity.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.rbInfinity.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbInfinity.Location = new System.Drawing.Point(249, 3);
            this.rbInfinity.Name = "rbInfinity";
            this.rbInfinity.Size = new System.Drawing.Size(241, 38);
            this.rbInfinity.TabIndex = 1;
            this.rbInfinity.TabStop = true;
            this.rbInfinity.Text = "Timetable for 24h ride";
            this.rbInfinity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbInfinity.UseVisualStyleBackColor = true;
            this.rbInfinity.CheckedChanged += new System.EventHandler(this.rbInfinity_CheckedChanged);
            // 
            // rbPauses
            // 
            this.rbPauses.AutoSize = true;
            this.rbPauses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rbPauses.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.rbPauses.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbPauses.Location = new System.Drawing.Point(3, 3);
            this.rbPauses.Name = "rbPauses";
            this.rbPauses.Size = new System.Drawing.Size(240, 38);
            this.rbPauses.TabIndex = 0;
            this.rbPauses.TabStop = true;
            this.rbPauses.Text = "Timetable for pauses";
            this.rbPauses.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPauses.UseVisualStyleBackColor = true;
            this.rbPauses.CheckedChanged += new System.EventHandler(this.rbPauses_CheckedChanged);
            // 
            // UCDataLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panelLoadedData1);
            this.Controls.Add(this.panelLDTitle);
            this.Name = "UCDataLoad";
            this.Size = new System.Drawing.Size(779, 468);
            this.panelLDTitle.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panelLDTitle;
        private Label labelLDTitle;
        private FontAwesome.Sharp.IconButton btnLoadDataPath;
        private Label label1;
        private TextBox textBoxPath;
        private Panel panelLoadedData1;
        private OpenFileDialog openFileDialog1;
        private TableLayoutPanel tableLayoutPanel1;
        private FontAwesome.Sharp.IconButton btnLoadDataPickFile;
        private TableLayoutPanel tableLayoutPanel2;
        private Label labelFileName;
        private RadioButton rbInfinity;
        private RadioButton rbPauses;
    }
}
