namespace TestDesignTT
{
    partial class UCEditMoving
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.dgwMoving = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnEditMoving = new FontAwesome.Sharp.IconButton();
            this.panel8 = new System.Windows.Forms.Panel();
            this.labelInfoAboutDelete = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgwMoving)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(684, 100);
            this.panel1.TabIndex = 0;
            // 
            // labelTitle
            // 
            this.labelTitle.BackColor = System.Drawing.SystemColors.HotTrack;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelTitle.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.labelTitle.Location = new System.Drawing.Point(0, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(684, 100);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Žádné vlaky momentálně nejedou";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(684, 391);
            this.panel2.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.dgwMoving);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(484, 200);
            this.panel5.TabIndex = 2;
            // 
            // dgwMoving
            // 
            this.dgwMoving.AllowUserToAddRows = false;
            this.dgwMoving.AllowUserToDeleteRows = false;
            this.dgwMoving.AllowUserToResizeColumns = false;
            this.dgwMoving.AllowUserToResizeRows = false;
            this.dgwMoving.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgwMoving.BackgroundColor = System.Drawing.SystemColors.HotTrack;
            this.dgwMoving.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Black", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgwMoving.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgwMoving.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgwMoving.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgwMoving.GridColor = System.Drawing.Color.DodgerBlue;
            this.dgwMoving.Location = new System.Drawing.Point(0, 0);
            this.dgwMoving.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.dgwMoving.Name = "dgwMoving";
            this.dgwMoving.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgwMoving.RowHeadersVisible = false;
            this.dgwMoving.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dgwMoving.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.dgwMoving.RowTemplate.Height = 25;
            this.dgwMoving.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgwMoving.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgwMoving.Size = new System.Drawing.Size(484, 200);
            this.dgwMoving.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnEditMoving);
            this.panel3.Controls.Add(this.panel8);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(484, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(200, 391);
            this.panel3.TabIndex = 0;
            // 
            // btnEditMoving
            // 
            this.btnEditMoving.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnEditMoving.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEditMoving.FlatAppearance.BorderSize = 0;
            this.btnEditMoving.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditMoving.Font = new System.Drawing.Font("Arial Rounded MT Bold", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btnEditMoving.ForeColor = System.Drawing.Color.White;
            this.btnEditMoving.IconChar = FontAwesome.Sharp.IconChar.Pen;
            this.btnEditMoving.IconColor = System.Drawing.Color.Red;
            this.btnEditMoving.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnEditMoving.IconSize = 32;
            this.btnEditMoving.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditMoving.Location = new System.Drawing.Point(0, 100);
            this.btnEditMoving.Name = "btnEditMoving";
            this.btnEditMoving.Padding = new System.Windows.Forms.Padding(10, 0, 20, 0);
            this.btnEditMoving.Size = new System.Drawing.Size(200, 50);
            this.btnEditMoving.TabIndex = 6;
            this.btnEditMoving.Text = "Edit";
            this.btnEditMoving.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditMoving.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEditMoving.UseVisualStyleBackColor = false;
            this.btnEditMoving.Click += new System.EventHandler(this.btnEditMoving_Click);
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel8.Controls.Add(this.labelInfoAboutDelete);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(200, 100);
            this.panel8.TabIndex = 0;
            // 
            // labelInfoAboutDelete
            // 
            this.labelInfoAboutDelete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelInfoAboutDelete.Font = new System.Drawing.Font("Segoe UI Emoji", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelInfoAboutDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelInfoAboutDelete.Location = new System.Drawing.Point(0, 0);
            this.labelInfoAboutDelete.Name = "labelInfoAboutDelete";
            this.labelInfoAboutDelete.Size = new System.Drawing.Size(200, 100);
            this.labelInfoAboutDelete.TabIndex = 1;
            this.labelInfoAboutDelete.Text = "Info";
            this.labelInfoAboutDelete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UCEditMoving
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "UCEditMoving";
            this.Size = new System.Drawing.Size(684, 491);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgwMoving)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label testLabelText;
        private Panel panel2;
        private Panel panel5;
        private Panel panel3;
        private Panel panel8;
        private DataGridView dgwMoving;
        private FontAwesome.Sharp.IconButton btnEditMoving;
        private Label labelInfoAboutDelete;
        private Label labelTitle;
    }
}
