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
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.btnLoadData = new FontAwesome.Sharp.IconButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panelLoadedData1 = new System.Windows.Forms.Panel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.labelFileName = new System.Windows.Forms.Label();
            this.panelLDTitle.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelLoadedData1.SuspendLayout();
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
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxPath);
            this.panel1.Controls.Add(this.btnLoadData);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 100);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(779, 40);
            this.panel1.TabIndex = 2;
            // 
            // textBoxPath
            // 
            this.textBoxPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxPath.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.textBoxPath.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.textBoxPath.Location = new System.Drawing.Point(103, 0);
            this.textBoxPath.Multiline = true;
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(562, 40);
            this.textBoxPath.TabIndex = 2;
            this.textBoxPath.Text = "Please enter fath to you CSV file...";
            this.textBoxPath.Enter += new System.EventHandler(this.textBoxPath_Enter);
            this.textBoxPath.Leave += new System.EventHandler(this.textBoxPath_Leave);
            // 
            // btnLoadData
            // 
            this.btnLoadData.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnLoadData.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnLoadData.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLoadData.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoadData.IconChar = FontAwesome.Sharp.IconChar.Share;
            this.btnLoadData.IconColor = System.Drawing.Color.Black;
            this.btnLoadData.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnLoadData.IconSize = 35;
            this.btnLoadData.Location = new System.Drawing.Point(665, 0);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(114, 40);
            this.btnLoadData.TabIndex = 1;
            this.btnLoadData.Text = "Find";
            this.btnLoadData.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLoadData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnLoadData.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 40);
            this.label1.TabIndex = 0;
            this.label1.Text = "File Path:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelLoadedData1
            // 
            this.panelLoadedData1.Controls.Add(this.btnLoad);
            this.panelLoadedData1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLoadedData1.Location = new System.Drawing.Point(0, 140);
            this.panelLoadedData1.Name = "panelLoadedData1";
            this.panelLoadedData1.Size = new System.Drawing.Size(779, 60);
            this.panelLoadedData1.TabIndex = 3;
            // 
            // btnLoad
            // 
            this.btnLoad.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLoad.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnLoad.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnLoad.Location = new System.Drawing.Point(0, 0);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(779, 60);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load data";
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // labelFileName
            // 
            this.labelFileName.BackColor = System.Drawing.SystemColors.HotTrack;
            this.labelFileName.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelFileName.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelFileName.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelFileName.Location = new System.Drawing.Point(0, 200);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(779, 155);
            this.labelFileName.TabIndex = 4;
            // 
            // UCDataLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Controls.Add(this.labelFileName);
            this.Controls.Add(this.panelLoadedData1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelLDTitle);
            this.Name = "UCDataLoad";
            this.Size = new System.Drawing.Size(779, 468);
            this.panelLDTitle.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelLoadedData1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panelLDTitle;
        private Label labelLDTitle;
        private Panel panel1;
        private FontAwesome.Sharp.IconButton btnLoadData;
        private Label label1;
        private TextBox textBoxPath;
        private Panel panelLoadedData1;
        private Button btnLoad;
        private OpenFileDialog openFileDialog1;
        private Label labelFileName;
    }
}
