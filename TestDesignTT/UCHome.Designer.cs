namespace TestDesignTT
{
    partial class UCHome
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelTestCount = new System.Windows.Forms.Label();
            this.labelMyTest = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::TestDesignTT.Properties.Resources.zcu_logo2;
            this.pictureBox2.Location = new System.Drawing.Point(261, 56);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(417, 154);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelTime.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelTime.Location = new System.Drawing.Point(130, 346);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(90, 37);
            this.labelTime.TabIndex = 2;
            this.labelTime.Text = "label1";
            // 
            // labelDate
            // 
            this.labelDate.AutoSize = true;
            this.labelDate.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelDate.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelDate.Location = new System.Drawing.Point(130, 402);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(90, 37);
            this.labelDate.TabIndex = 3;
            this.labelDate.Text = "label2";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labelTestCount
            // 
            this.labelTestCount.AutoSize = true;
            this.labelTestCount.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelTestCount.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelTestCount.Location = new System.Drawing.Point(135, 292);
            this.labelTestCount.Name = "labelTestCount";
            this.labelTestCount.Size = new System.Drawing.Size(65, 28);
            this.labelTestCount.TabIndex = 4;
            this.labelTestCount.Text = "label1";
            // 
            // labelMyTest
            // 
            this.labelMyTest.AutoSize = true;
            this.labelMyTest.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelMyTest.Location = new System.Drawing.Point(135, 253);
            this.labelMyTest.Name = "labelMyTest";
            this.labelMyTest.Size = new System.Drawing.Size(38, 15);
            this.labelMyTest.TabIndex = 5;
            this.labelMyTest.Text = "label1";
            // 
            // UCHome
            // 
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Controls.Add(this.labelMyTest);
            this.Controls.Add(this.labelTestCount);
            this.Controls.Add(this.labelDate);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.pictureBox2);
            this.Name = "UCHome";
            this.Size = new System.Drawing.Size(934, 570);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label labelDate;
        private Label labelTime;
        //private PictureBox pictureZCU;
        private PictureBox pictureBox2;
        private System.Windows.Forms.Timer timer1;
        private Label labelTestCount;
        private Label labelMyTest;
    }
}
