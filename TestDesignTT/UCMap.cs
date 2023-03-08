using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace TestDesignTT
{
    public partial class UCMap : UserControl
    {

        double[] x_positions = { 0.13, 0.12, 0.018, 0.055, 0.145, 0.15, 0.105, 0.095 };
        double[] y_positions = { 0.06, 0.027, 0.17, 0.15, 0.32, 0.29, 0.067, 0.028 };


        public UCMap()
        {
            InitializeComponent();
        }

        public void setLabels()
        {


            int width = pictureBox1.ClientSize.Width;
            int height = pictureBox1.ClientSize.Height;

            /*
            Form parentForm = this.FindForm();
            if (parentForm.WindowState == FormWindowState.Maximized)
            {
                setLabels();
            }
            */

            //int x = (int)(width * 0.05); // center horizontally
            //int y = (int)(height * 0.1); // center vertically

            for (int i = 0; i < x_positions.Length; i++)
            {
                int x = (int)(width * x_positions[i]); // center horizontally
                int y = (int)(height * y_positions[i]); // center vertically
                Label label = new Label();
                label.Text = i.ToString();
                label.Font = new Font("Arial", 9, FontStyle.Bold);
                label.BackColor = Color.Transparent;
                label.AutoSize = true;
                //label.Location = new Point((int)(width * x_positions[i]), (int)(width * y_positions[i]));
                label.Location = new Point(x, y);



                label.Paint += (s, e) => {
                    e.Graphics.DrawLine(new Pen(Color.Red, 1), new Point(0, 0), new Point(0, label.Height));
                };


                pictureBox1.Controls.Add(label);
                label.BringToFront();
            }
            /*
            Label label = new Label();
            label.Text = "My label";
            label.Font = new Font("Arial", 12);
            label.BackColor = Color.Transparent;
            label.Location = new Point(x, y);
            //label.BringToFront();

            pictureBox1.Controls.Add(label);

            label.BringToFront();
            */
        }

        private void UCMap_SizeChanged(object sender, EventArgs e)
        {
            foreach (var control in pictureBox1.Controls.OfType<Label>().ToList())
            {
                pictureBox1.Controls.Remove(control);
            }
            setLabels();
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            foreach (var control in pictureBox1.Controls.OfType<Label>().ToList())
            {
                pictureBox1.Controls.Remove(control);
            }
            setLabels();
        }
    }
}
