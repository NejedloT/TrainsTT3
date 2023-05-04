namespace TestDesignTT
{
    partial class UCAddManualTrain
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
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnClear = new FontAwesome.Sharp.IconButton();
            this.btnAddTrain = new FontAwesome.Sharp.IconButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cbFinalTrack = new System.Windows.Forms.ComboBox();
            this.labelSpecificTrack = new System.Windows.Forms.Label();
            this.labelFinalTrack = new System.Windows.Forms.Label();
            this.cbFinalStation = new System.Windows.Forms.ComboBox();
            this.labelFinalStation = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbDirect = new System.Windows.Forms.ComboBox();
            this.cbSpeed = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbPickTrain = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.radioButtonNo = new System.Windows.Forms.RadioButton();
            this.radioButtonYes = new System.Windows.Forms.RadioButton();
            this.tbStartPosition = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(974, 100);
            this.label1.TabIndex = 1;
            this.label1.Text = "Add train that you want to start moving";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnAddTrain, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 511);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(974, 100);
            this.tableLayoutPanel1.TabIndex = 2;
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
            this.btnClear.Location = new System.Drawing.Point(489, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnClear.Size = new System.Drawing.Size(237, 94);
            this.btnClear.TabIndex = 14;
            this.btnClear.Text = "Clear Data";
            this.btnClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnAddTrain
            // 
            this.btnAddTrain.AutoSize = true;
            this.btnAddTrain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddTrain.FlatAppearance.BorderSize = 0;
            this.btnAddTrain.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddTrain.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnAddTrain.ForeColor = System.Drawing.Color.White;
            this.btnAddTrain.IconChar = FontAwesome.Sharp.IconChar.Plus;
            this.btnAddTrain.IconColor = System.Drawing.Color.Red;
            this.btnAddTrain.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAddTrain.IconSize = 25;
            this.btnAddTrain.Location = new System.Drawing.Point(246, 3);
            this.btnAddTrain.Name = "btnAddTrain";
            this.btnAddTrain.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.btnAddTrain.Size = new System.Drawing.Size(237, 94);
            this.btnAddTrain.TabIndex = 13;
            this.btnAddTrain.Text = "Add train";
            this.btnAddTrain.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddTrain.UseVisualStyleBackColor = true;
            this.btnAddTrain.Click += new System.EventHandler(this.btnAddTrain_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 100);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(974, 100);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.tableLayoutPanel2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 200);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(974, 264);
            this.panel2.TabIndex = 4;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 350F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel2.Controls.Add(this.cbFinalTrack, 2, 7);
            this.tableLayoutPanel2.Controls.Add(this.labelSpecificTrack, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.labelFinalTrack, 1, 7);
            this.tableLayoutPanel2.Controls.Add(this.cbFinalStation, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.labelFinalStation, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.label6, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.cbDirect, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.cbSpeed, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.label4, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.cbPickTrain, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 2, 6);
            this.tableLayoutPanel2.Controls.Add(this.tbStartPosition, 2, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(974, 264);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // cbFinalTrack
            // 
            this.cbFinalTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbFinalTrack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFinalTrack.FormattingEnabled = true;
            this.cbFinalTrack.Items.AddRange(new object[] {
            "1"});
            this.cbFinalTrack.Location = new System.Drawing.Point(462, 234);
            this.cbFinalTrack.Name = "cbFinalTrack";
            this.cbFinalTrack.Size = new System.Drawing.Size(344, 23);
            this.cbFinalTrack.TabIndex = 17;
            this.cbFinalTrack.SelectedIndexChanged += new System.EventHandler(this.cbFinalTrack_SelectedIndexChanged);
            // 
            // labelSpecificTrack
            // 
            this.labelSpecificTrack.AutoSize = true;
            this.labelSpecificTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSpecificTrack.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelSpecificTrack.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelSpecificTrack.Location = new System.Drawing.Point(109, 198);
            this.labelSpecificTrack.Margin = new System.Windows.Forms.Padding(0);
            this.labelSpecificTrack.Name = "labelSpecificTrack";
            this.labelSpecificTrack.Size = new System.Drawing.Size(350, 33);
            this.labelSpecificTrack.TabIndex = 15;
            this.labelSpecificTrack.Text = "Want to pick specific track?";
            this.labelSpecificTrack.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelFinalTrack
            // 
            this.labelFinalTrack.AutoSize = true;
            this.labelFinalTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFinalTrack.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelFinalTrack.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelFinalTrack.Location = new System.Drawing.Point(109, 231);
            this.labelFinalTrack.Margin = new System.Windows.Forms.Padding(0);
            this.labelFinalTrack.Name = "labelFinalTrack";
            this.labelFinalTrack.Size = new System.Drawing.Size(350, 33);
            this.labelFinalTrack.TabIndex = 14;
            this.labelFinalTrack.Text = "Final track:";
            this.labelFinalTrack.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbFinalStation
            // 
            this.cbFinalStation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbFinalStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFinalStation.FormattingEnabled = true;
            this.cbFinalStation.Items.AddRange(new object[] {
            "1"});
            this.cbFinalStation.Location = new System.Drawing.Point(462, 168);
            this.cbFinalStation.Name = "cbFinalStation";
            this.cbFinalStation.Size = new System.Drawing.Size(344, 23);
            this.cbFinalStation.TabIndex = 12;
            this.cbFinalStation.SelectedIndexChanged += new System.EventHandler(this.cbFinalStation_SelectedIndexChanged);
            // 
            // labelFinalStation
            // 
            this.labelFinalStation.AutoSize = true;
            this.labelFinalStation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFinalStation.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelFinalStation.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelFinalStation.Location = new System.Drawing.Point(109, 165);
            this.labelFinalStation.Margin = new System.Windows.Forms.Padding(0);
            this.labelFinalStation.Name = "labelFinalStation";
            this.labelFinalStation.Size = new System.Drawing.Size(350, 33);
            this.labelFinalStation.TabIndex = 11;
            this.labelFinalStation.Text = "Final Station:";
            this.labelFinalStation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label6.Location = new System.Drawing.Point(109, 33);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(350, 33);
            this.label6.TabIndex = 10;
            this.label6.Text = "Start position:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbDirect
            // 
            this.cbDirect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbDirect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDirect.FormattingEnabled = true;
            this.cbDirect.Items.AddRange(new object[] {
            "Direct",
            "Reverse"});
            this.cbDirect.Location = new System.Drawing.Point(462, 69);
            this.cbDirect.Name = "cbDirect";
            this.cbDirect.Size = new System.Drawing.Size(344, 23);
            this.cbDirect.TabIndex = 8;
            this.cbDirect.SelectedIndexChanged += new System.EventHandler(this.cbDirect_SelectedIndexChanged);
            // 
            // cbSpeed
            // 
            this.cbSpeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSpeed.FormattingEnabled = true;
            this.cbSpeed.Items.AddRange(new object[] {
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20"});
            this.cbSpeed.Location = new System.Drawing.Point(462, 102);
            this.cbSpeed.Name = "cbSpeed";
            this.cbSpeed.Size = new System.Drawing.Size(344, 23);
            this.cbSpeed.TabIndex = 7;
            this.cbSpeed.SelectedIndexChanged += new System.EventHandler(this.cbSpeed_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label4.Location = new System.Drawing.Point(109, 66);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(350, 33);
            this.label4.TabIndex = 4;
            this.label4.Text = "Reverse/Direct:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label2.Location = new System.Drawing.Point(109, 99);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(350, 33);
            this.label2.TabIndex = 3;
            this.label2.Text = "Set its speed:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label3.Location = new System.Drawing.Point(109, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(350, 33);
            this.label3.TabIndex = 2;
            this.label3.Text = "Pick train:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbPickTrain
            // 
            this.cbPickTrain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPickTrain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPickTrain.FormattingEnabled = true;
            this.cbPickTrain.Location = new System.Drawing.Point(462, 3);
            this.cbPickTrain.Name = "cbPickTrain";
            this.cbPickTrain.Size = new System.Drawing.Size(344, 23);
            this.cbPickTrain.TabIndex = 6;
            this.cbPickTrain.SelectedIndexChanged += new System.EventHandler(this.cbPickTrain_SelectedIndexChanged);
            this.cbPickTrain.Click += new System.EventHandler(this.cbPickTrain_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.radioButtonNo, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.radioButtonYes, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(462, 201);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(344, 27);
            this.tableLayoutPanel3.TabIndex = 16;
            // 
            // radioButtonNo
            // 
            this.radioButtonNo.AutoSize = true;
            this.radioButtonNo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radioButtonNo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.radioButtonNo.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.radioButtonNo.Location = new System.Drawing.Point(175, 3);
            this.radioButtonNo.Name = "radioButtonNo";
            this.radioButtonNo.Size = new System.Drawing.Size(166, 21);
            this.radioButtonNo.TabIndex = 1;
            this.radioButtonNo.TabStop = true;
            this.radioButtonNo.Text = "No";
            this.radioButtonNo.UseVisualStyleBackColor = true;
            this.radioButtonNo.CheckedChanged += new System.EventHandler(this.radioButtonNo_CheckedChanged);
            // 
            // radioButtonYes
            // 
            this.radioButtonYes.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.radioButtonYes.AutoSize = true;
            this.radioButtonYes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.radioButtonYes.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.radioButtonYes.Location = new System.Drawing.Point(60, 3);
            this.radioButtonYes.Name = "radioButtonYes";
            this.radioButtonYes.Size = new System.Drawing.Size(51, 21);
            this.radioButtonYes.TabIndex = 0;
            this.radioButtonYes.TabStop = true;
            this.radioButtonYes.Text = "Yes";
            this.radioButtonYes.UseVisualStyleBackColor = true;
            this.radioButtonYes.CheckedChanged += new System.EventHandler(this.radioButtonYes_CheckedChanged);
            // 
            // tbStartPosition
            // 
            this.tbStartPosition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStartPosition.Enabled = false;
            this.tbStartPosition.Location = new System.Drawing.Point(462, 36);
            this.tbStartPosition.Name = "tbStartPosition";
            this.tbStartPosition.Size = new System.Drawing.Size(344, 23);
            this.tbStartPosition.TabIndex = 18;
            // 
            // UCAddManualTrain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label1);
            this.Name = "UCAddManualTrain";
            this.Size = new System.Drawing.Size(974, 611);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private TableLayoutPanel tableLayoutPanel1;
        private FontAwesome.Sharp.IconButton btnAddTrain;
        private FontAwesome.Sharp.IconButton btnClear;
        private Panel panel1;
        private Panel panel2;
        private TableLayoutPanel tableLayoutPanel2;
        private ComboBox cbDirect;
        private ComboBox cbSpeed;
        private Label label4;
        private Label label2;
        private Label label3;
        private ComboBox cbPickTrain;
        private Label labelSpecificTrack;
        private Label labelFinalTrack;
        private ComboBox cbFinalStation;
        private Label labelFinalStation;
        private Label label6;
        private TableLayoutPanel tableLayoutPanel3;
        private RadioButton radioButtonNo;
        private RadioButton radioButtonYes;
        private ComboBox cbFinalTrack;
        private TextBox tbStartPosition;
    }
}
