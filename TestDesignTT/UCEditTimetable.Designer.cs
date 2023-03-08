namespace TestDesignTT
{
    partial class UCEditTimetable
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.titleDisplayData = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnDeleteTT = new FontAwesome.Sharp.IconButton();
            this.btnEditTT = new FontAwesome.Sharp.IconButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelInfoAboutDelete = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.titleDisplayData);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(919, 100);
            this.panel1.TabIndex = 0;
            // 
            // titleDisplayData
            // 
            this.titleDisplayData.BackColor = System.Drawing.SystemColors.HotTrack;
            this.titleDisplayData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleDisplayData.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.titleDisplayData.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.titleDisplayData.Location = new System.Drawing.Point(0, 0);
            this.titleDisplayData.Name = "titleDisplayData";
            this.titleDisplayData.Size = new System.Drawing.Size(919, 100);
            this.titleDisplayData.TabIndex = 3;
            this.titleDisplayData.Text = "Žádné vlaky nebyly načteny";
            this.titleDisplayData.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel2.Controls.Add(this.btnDeleteTT);
            this.panel2.Controls.Add(this.btnEditTT);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(719, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(200, 400);
            this.panel2.TabIndex = 1;
            // 
            // btnDeleteTT
            // 
            this.btnDeleteTT.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnDeleteTT.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDeleteTT.FlatAppearance.BorderSize = 0;
            this.btnDeleteTT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteTT.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnDeleteTT.ForeColor = System.Drawing.Color.White;
            this.btnDeleteTT.IconChar = FontAwesome.Sharp.IconChar.DeleteLeft;
            this.btnDeleteTT.IconColor = System.Drawing.Color.Red;
            this.btnDeleteTT.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDeleteTT.IconSize = 32;
            this.btnDeleteTT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteTT.Location = new System.Drawing.Point(0, 150);
            this.btnDeleteTT.Name = "btnDeleteTT";
            this.btnDeleteTT.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnDeleteTT.Size = new System.Drawing.Size(200, 50);
            this.btnDeleteTT.TabIndex = 6;
            this.btnDeleteTT.Text = "Delete";
            this.btnDeleteTT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDeleteTT.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDeleteTT.UseVisualStyleBackColor = false;
            this.btnDeleteTT.Click += new System.EventHandler(this.btnDeleteTT_Click);
            // 
            // btnEditTT
            // 
            this.btnEditTT.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnEditTT.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEditTT.FlatAppearance.BorderSize = 0;
            this.btnEditTT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditTT.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnEditTT.ForeColor = System.Drawing.Color.White;
            this.btnEditTT.IconChar = FontAwesome.Sharp.IconChar.Pen;
            this.btnEditTT.IconColor = System.Drawing.Color.Red;
            this.btnEditTT.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnEditTT.IconSize = 32;
            this.btnEditTT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditTT.Location = new System.Drawing.Point(0, 100);
            this.btnEditTT.Name = "btnEditTT";
            this.btnEditTT.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnEditTT.Size = new System.Drawing.Size(200, 50);
            this.btnEditTT.TabIndex = 5;
            this.btnEditTT.Text = "Edit";
            this.btnEditTT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditTT.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEditTT.UseVisualStyleBackColor = false;
            this.btnEditTT.Click += new System.EventHandler(this.btnEditTT_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labelInfoAboutDelete);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 100);
            this.panel3.TabIndex = 1;
            // 
            // labelInfoAboutDelete
            // 
            this.labelInfoAboutDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelInfoAboutDelete.Font = new System.Drawing.Font("Segoe UI Emoji", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelInfoAboutDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelInfoAboutDelete.Location = new System.Drawing.Point(0, 0);
            this.labelInfoAboutDelete.Name = "labelInfoAboutDelete";
            this.labelInfoAboutDelete.Size = new System.Drawing.Size(200, 100);
            this.labelInfoAboutDelete.TabIndex = 0;
            this.labelInfoAboutDelete.Text = "Info";
            this.labelInfoAboutDelete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 100);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(719, 334);
            this.panel4.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.HotTrack;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Black", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.GridColor = System.Drawing.Color.DodgerBlue;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dataGridView1.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.dataGridView1.RowTemplate.Height = 25;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(719, 334);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // UCEditTimetable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "UCEditTimetable";
            this.Size = new System.Drawing.Size(919, 500);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Label labelInfoAboutDelete;
        private Panel panel4;
        private Label titleDisplayData;
        private DataGridView dataGridView1;
        private FontAwesome.Sharp.IconButton btnDeleteTT;
        private FontAwesome.Sharp.IconButton btnEditTT;
    }
}
