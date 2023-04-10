namespace TestDesignTT
{
    partial class UCEditJson
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
            this.btnClear = new FontAwesome.Sharp.IconButton();
            this.btnSaveData = new FontAwesome.Sharp.IconButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cbDirection = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbPreviousPosition = new System.Windows.Forms.ComboBox();
            this.cbCurrentPosition = new System.Windows.Forms.ComboBox();
            this.cbPickTrain = new System.Windows.Forms.ComboBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(996, 100);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(996, 100);
            this.label1.TabIndex = 0;
            this.label1.Text = "Update train current and previous position";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 475);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(996, 100);
            this.panel2.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSaveData, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(996, 100);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.IconChar = FontAwesome.Sharp.IconChar.XmarkSquare;
            this.btnClear.IconColor = System.Drawing.Color.Red;
            this.btnClear.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnClear.IconSize = 25;
            this.btnClear.Location = new System.Drawing.Point(501, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnClear.Size = new System.Drawing.Size(243, 94);
            this.btnClear.TabIndex = 15;
            this.btnClear.Text = "Clear Data";
            this.btnClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSaveData
            // 
            this.btnSaveData.AutoSize = true;
            this.btnSaveData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSaveData.FlatAppearance.BorderSize = 0;
            this.btnSaveData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveData.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnSaveData.ForeColor = System.Drawing.Color.White;
            this.btnSaveData.IconChar = FontAwesome.Sharp.IconChar.ArrowDown;
            this.btnSaveData.IconColor = System.Drawing.Color.Red;
            this.btnSaveData.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnSaveData.IconSize = 25;
            this.btnSaveData.Location = new System.Drawing.Point(252, 3);
            this.btnSaveData.Name = "btnSaveData";
            this.btnSaveData.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnSaveData.Size = new System.Drawing.Size(243, 94);
            this.btnSaveData.TabIndex = 14;
            this.btnSaveData.Text = "Save changes";
            this.btnSaveData.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSaveData.UseVisualStyleBackColor = true;
            this.btnSaveData.Click += new System.EventHandler(this.btnSaveData_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tableLayoutPanel2);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 100);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(996, 375);
            this.panel3.TabIndex = 3;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.Controls.Add(this.cbDirection, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.label5, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label4, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.cbPreviousPosition, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.cbCurrentPosition, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.cbPickTrain, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 50);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(996, 325);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // cbDirection
            // 
            this.cbDirection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDirection.FormattingEnabled = true;
            this.cbDirection.Items.AddRange(new object[] {
            "Direct",
            "Reverse"});
            this.cbDirection.Location = new System.Drawing.Point(471, 108);
            this.cbDirection.Name = "cbDirection";
            this.cbDirection.Size = new System.Drawing.Size(344, 23);
            this.cbDirection.TabIndex = 14;
            this.cbDirection.SelectedIndexChanged += new System.EventHandler(this.cbDirection_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label5.Location = new System.Drawing.Point(118, 105);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(350, 35);
            this.label5.TabIndex = 13;
            this.label5.Text = "Pick last locomotive direct:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(118, 70);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(350, 35);
            this.label4.TabIndex = 12;
            this.label4.Text = "Pick previous position:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(118, 35);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(350, 35);
            this.label2.TabIndex = 11;
            this.label2.Text = "Pick current position:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(118, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(350, 35);
            this.label3.TabIndex = 10;
            this.label3.Text = "Pick train:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbPreviousPosition
            // 
            this.cbPreviousPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPreviousPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPreviousPosition.FormattingEnabled = true;
            this.cbPreviousPosition.Location = new System.Drawing.Point(471, 73);
            this.cbPreviousPosition.Name = "cbPreviousPosition";
            this.cbPreviousPosition.Size = new System.Drawing.Size(344, 23);
            this.cbPreviousPosition.TabIndex = 9;
            this.cbPreviousPosition.SelectedIndexChanged += new System.EventHandler(this.cbPreviousPosition_SelectedIndexChanged);
            // 
            // cbCurrentPosition
            // 
            this.cbCurrentPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbCurrentPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCurrentPosition.FormattingEnabled = true;
            this.cbCurrentPosition.Location = new System.Drawing.Point(471, 38);
            this.cbCurrentPosition.Name = "cbCurrentPosition";
            this.cbCurrentPosition.Size = new System.Drawing.Size(344, 23);
            this.cbCurrentPosition.TabIndex = 8;
            this.cbCurrentPosition.SelectedIndexChanged += new System.EventHandler(this.cbCurrentPosition_SelectedIndexChanged);
            this.cbCurrentPosition.Click += new System.EventHandler(this.cbCurrentPosition_Click);
            // 
            // cbPickTrain
            // 
            this.cbPickTrain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPickTrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPickTrain.FormattingEnabled = true;
            this.cbPickTrain.Location = new System.Drawing.Point(471, 3);
            this.cbPickTrain.Name = "cbPickTrain";
            this.cbPickTrain.Size = new System.Drawing.Size(344, 23);
            this.cbPickTrain.TabIndex = 7;
            this.cbPickTrain.SelectedIndexChanged += new System.EventHandler(this.cbPickTrain_SelectedIndexChanged);
            this.cbPickTrain.Click += new System.EventHandler(this.cbPickTrain_Click);
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(996, 50);
            this.panel4.TabIndex = 0;
            // 
            // UCEditJson
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "UCEditJson";
            this.Size = new System.Drawing.Size(996, 575);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Panel panel2;
        private TableLayoutPanel tableLayoutPanel1;
        private FontAwesome.Sharp.IconButton btnSaveData;
        private FontAwesome.Sharp.IconButton btnClear;
        private Panel panel3;
        private TableLayoutPanel tableLayoutPanel2;
        private Panel panel4;
        private ComboBox cbPreviousPosition;
        private ComboBox cbCurrentPosition;
        private ComboBox cbPickTrain;
        private Label label3;
        private Label label4;
        private Label label2;
        private ComboBox cbDirection;
        private Label label5;
    }
}
