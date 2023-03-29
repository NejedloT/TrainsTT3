using ControlLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using TrainTTLibrary;

namespace TestDesignTT
{
    public partial class UCLokomotives : UserControl
    {
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ChangeOfTrainData;

        private static List<Trains> trainsList = new List<Trains>();

        private static HashSet<string> addedLocomotives = new HashSet<string>();

        public List<ChangeTrainData> trainDataChange = new List<ChangeTrainData>();
        public UCLokomotives()
        {
            InitializeComponent();
            hideAll();
        }

        public void hideAll()
        {
            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                tableLayoutPanel1.RowStyles[i] = new RowStyle(SizeType.Absolute, 0);
                checkStartStopButton();
            }
        }

        private void updateHashLoco(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            int rowIndex = tableLayoutPanel1.GetRow(combo);
            addedLocomotives.Clear();

            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                if (i == rowIndex)
                    continue;

                ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                if (cbTrain.SelectedIndex > -1)
                    addedLocomotives.Add(cbTrain.SelectedItem.ToString());
            }

            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                if (i == rowIndex)
                {
                    ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                    cbTrain.Items.Clear();
                    for (int j = 0; j < LocomotiveInfo.listOfLocomotives.Count; j++)
                    {
                        string loco = Packet.UnderLineToGap(LocomotiveInfo.listOfLocomotives[j].Name);

                        if (!addedLocomotives.Contains(loco))
                        {
                            cbTrain.Items.Add(loco);
                        }
                    }
                }
            }
        }

        private void btnAddTrain_Click(object sender, EventArgs e)
        {
            trainsList = ControlLogic.MainLogic.GetData();
            checkStartStopButton();
            int[] rowHeights = tableLayoutPanel1.GetRowHeights();

            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                if (cbTrain.SelectedIndex > -1)
                    addedLocomotives.Add(cbTrain.SelectedItem.ToString());
            }

            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                if (rowHeights[i] < 0.5)
                {
                    tableLayoutPanel1.RowStyles[i] = new RowStyle(SizeType.Absolute, 35);
                    /*
                    ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                    cbTrain.Items.Clear();
                    for (int j = 0; j < LocomotiveInfo.listOfLocomotives.Count; j++)
                    {
                        string loco = Packet.UnderLineToGap(LocomotiveInfo.listOfLocomotives[j].Name);

                        if (!addedLocomotives.Contains(loco))
                        {
                            cbTrain.Items.Add(loco);
                        }
                    }
                    */

                    break;
                }
                /*
                else
                {
                    ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                    addedLocomotives.Add(cbTrain.SelectedItem.ToString());
                }
                */

            }
            //tableLayoutPanel1.RowStyles[1] = new RowStyle(SizeType.Absolute, 35);
        }

        /*
        private void btnAddTrain_Click(object sender, EventArgs e)
        {
            trainsList = ControlLogic.MainLogic.GetData();
            checkStartStopButton();
            int[] rowHeights = tableLayoutPanel1.GetRowHeights();
            for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            {
                if (rowHeights[i] < 0.5) {
                    tableLayoutPanel1.RowStyles[i] = new RowStyle(SizeType.Absolute, 35);
                    ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                    cbTrain.Items.Clear();
                    foreach (Trains train in trainsList)
                    {
                        if (train.move == "0")
                        {
                            cbTrain.Items.Add(train.name);
                        }
                    }

                    break;
                }

            }
            //tableLayoutPanel1.RowStyles[1] = new RowStyle(SizeType.Absolute, 35);
        }
        */

        #region Buttons Start/Stop click events
        private void btnStartStop1_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop2_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop3_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop4_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop5_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop6_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop7_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop8_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }

        private void btnStartStop9_Click(object sender, EventArgs e)
        {
            startStopAction(sender, e);
        }
        #endregion

        #region Comboboxes for train and speed, click events and logic

        private void cbTrain1_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender,e);
        }

        private void cbTrain2_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain3_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain4_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain5_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain6_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain7_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain8_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }

        private void cbTrain9_Click(object sender, EventArgs e)
        {
            updateHashLoco(sender, e);
        }
        private void cbTrain1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain2_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed2_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain3_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed3_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain4_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed4_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain5_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed5_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain6_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed6_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain7_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed7_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain8_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed8_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbTrain9_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }

        private void cbSpeed9_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkStartStopButton();
        }
        #endregion

        private void checkStartStopButton()
        {
            for (int i = 0; i < 8; i++)
            {
                ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, i) as ComboBox;
                ComboBox cbSpeed = tableLayoutPanel1.GetControlFromPosition(2, i) as ComboBox;

                Button btnStartStop = tableLayoutPanel1.GetControlFromPosition(4, i) as Button;

                if ((cbSpeed.SelectedIndex > -1) && (cbTrain.SelectedIndex > -1))
                {
                    if (btnStartStop.Text != "Stop")
                    {
                        btnStartStop.Enabled = true;
                        btnStartStop.ForeColor = Color.DarkOliveGreen;
                        btnStartStop.Font = new Font(btnStartStop.Font, FontStyle.Bold);
                    }
                }
                else
                {
                    btnStartStop.Enabled = false;
                    //btnStartStop.ForeColor = SystemColors.ControlDark;
                }
            }

            int[] rowHeights = tableLayoutPanel1.GetRowHeights();
            if (rowHeights[8] > 20)
            {
                btnAddLocomotive.Enabled = false;
            }
            else
                btnAddLocomotive.Enabled = true;
        }


        private void startStopAction(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return; // Exit if sender is not a button

            // Find the row index of the button's parent control in the table layout
            //int rowIndex = tableLayoutPanel1.GetRow(button.Parent);
            int rowIndex = tableLayoutPanel1.GetRow(button);

            // Find the comboboxes in the same row as the button
            ComboBox cbTrain = tableLayoutPanel1.GetControlFromPosition(1, rowIndex) as ComboBox;
            ComboBox cbSpeed = tableLayoutPanel1.GetControlFromPosition(2, rowIndex) as ComboBox;
            CheckBox cbReverse = tableLayoutPanel1.GetControlFromPosition(3, rowIndex) as CheckBox;

            // Do something with the comboboxes
            if (button.Text == "Start")
            {
                button.Text = "Stop";
                cbTrain.Enabled = false;
                cbSpeed.Enabled = false;
                cbReverse.Enabled = false;
                button.ForeColor = Color.Red;
                button.Font = new Font(button.Font, FontStyle.Bold);
                //Send data to start locomotive

            }
            else
            {
                button.Text = "Start";
                cbTrain.Enabled = true;
                cbReverse.Enabled = true;
                cbSpeed.Enabled = true;
                button.ForeColor = Color.DarkOliveGreen;
                button.Font = new Font(button.Font, FontStyle.Bold);
                //Stop locomotive
            }

            byte speedToByte;

            byte.TryParse(cbSpeed.SelectedItem.ToString(), out speedToByte);

            ChangeTrainData matchingTrain = trainDataChange.FirstOrDefault(t => t.Lokomotive == cbTrain.SelectedItem.ToString());
            if (matchingTrain != null)
            {

                matchingTrain.Speed = speedToByte;
                matchingTrain.Reverze = cbReverse.Checked ? true : false;
                matchingTrain.StartStop = button.Text == "Stop" ? true : false;
            }
            else
            {

                addTrainData(cbTrain.SelectedItem.ToString(), speedToByte, cbReverse.Checked ? true : false, button.Text == "Stop" ? true : false);
            }

            ChangeOfTrainData?.Invoke(this, e);
        }

        public void addTrainData(string locomotive, byte speed, bool reverze, bool startStop)
        {
            trainDataChange.Add(new ChangeTrainData { Lokomotive = locomotive, Speed = speed, Reverze = reverze, StartStop = startStop });
        }
    }

    public class ChangeTrainData
    {
        public string Lokomotive;
        public byte Speed;
        public bool Reverze;
        public bool StartStop;
    }
}
