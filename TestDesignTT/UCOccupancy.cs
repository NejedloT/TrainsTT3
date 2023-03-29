using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrainTTLibrary;
using System.Timers;
using System.Xml;

namespace TestDesignTT
{
    public partial class UCOccupancy : UserControl
    {
        private System.Timers.Timer occupancyTimer;

        public UCOccupancy()
        {
            InitializeComponent();

            /*
            occupancyTimer = new System.Timers.Timer(1000);

            occupancyTimer.Elapsed += UCOccupancy_Tick;

            occupancyTimer.AutoReset = true;

            occupancyTimer.Enabled = true;
            */
        }

        private void UCOccupancy_Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            List<Section> occupancySections = TrainTTLibrary.SectionInfo.listOfSection;

            //List<Section> occupancySections = TCPServerTrainTT.Program.GetOccupancySections();

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.RowStyles.Clear();

            if (occupancySections.Count > 0)
            {
                foreach (Section section in occupancySections)
                {
                    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                    Label label = new Label();
                    label.AutoSize = true;
                    label.Text = section.ToString();

                    tableLayoutPanel1.Controls.Add(label);
                }
            }
        }

        public void displayOccupacySections(List<Section> aaa)
        {
            List<Section> occupancySections = TrainTTLibrary.SectionInfo.listOfSection;
        }

        private void UCOccupancy_Load(object sender, EventArgs e)
        {
            occupancyTimer.Start();
        }

        private void UCOccupancy_Leave(object sender, EventArgs e)
        {
            occupancyTimer.Stop();
        }
    }
}
