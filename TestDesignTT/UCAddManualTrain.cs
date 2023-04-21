using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ControlLogic;

namespace TestDesignTT
{
    public partial class UCAddManualTrain : UserControl
    {
        //private LoadJson loadJson;
        private TrainDataJSON loadJson;

        bool testing = false;

        private static List<Trains> trainsList = new List<Trains>();

        private static IEnumerable<XElement> fromCircuit = new List<XElement>();

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonAddLocoClick;

        public List<AddDataToSend> addNewLocoData = new List<AddDataToSend>();


        public UCAddManualTrain()
        {
            InitializeComponent();
            ClearData();
            CheckValues();
        }


        public void ClearData()
        {
            cbPickTrain.SelectedIndex = -1;
            ClearAllExceptTrain();
            CheckValues();
        }

        public void ClearAllExceptTrain()
        {
            cbDirect.SelectedIndex = -1;
            cbSpeed.SelectedIndex = -1;
            //cbExitPoint.SelectedIndex = -1;
            cbFinalStation.SelectedIndex = -1;
            cbFinalTrack.SelectedIndex = -1;
            cbFinalTrack.Visible = false;
            labelFinalTrack.Visible = false;
            tbStartPosition.Text= string.Empty;

        }

        public void EnableDependingOnTrain(bool bb, bool direct)
        {
            cbDirect.Enabled = direct;
            cbSpeed.Enabled = bb;
            //cbExitPoint.Enabled = bb;
            cbFinalStation.Enabled = bb;
            cbFinalTrack.Enabled = bb;
            radioButtonNo.Enabled = bb;
            radioButtonYes.Enabled = bb;
        }

        public void CheckValues()
        {
            if (cbPickTrain.SelectedIndex == -1)
            {
                EnableDependingOnTrain(false, false);
                ClearAllExceptTrain();
            }
            else
            {
                cbDirect.Enabled = true;
                EnableDependingOnTrain(true, true);
                /*
                if (cbDirect.SelectedIndex == -1)
                {
                    EnableDependingOnTrain(false, true);
                }
                else
                {
                    EnableDependingOnTrain(true, true);
                }
                */
            }

            if (cbDirect.SelectedIndex > -1)
                cbFinalStation.Enabled = true;
            else
                cbFinalTrack.Enabled = false;


            if (radioButtonYes.Checked)
            {
                cbFinalTrack.Visible = true;
                labelFinalTrack.Visible = true;
                radioButtonNo.Checked = false;
            }
            if (radioButtonNo.Checked)
            {
                cbFinalTrack.Visible = false;
                labelFinalTrack.Visible = false;
                radioButtonYes.Checked = false;
            }

            if (testing)
            {
                radioButtonNo.Visible = false;
                radioButtonYes.Visible = false;
                labelSpecificTrack.Visible = false;
                labelFinalStation.Visible = false;
                cbFinalTrack.Visible = false;
                cbFinalStation.Visible = false;
                cbExitPoint.Visible = false;
                labelExitPoint.Visible = false;
            }

            checkAddButton();
        }

        private void checkAddButton()
        {
            if ((cbPickTrain.SelectedIndex > -1) && (cbDirect.SelectedIndex > -1) && (cbSpeed.SelectedIndex > -1))
            {
                if (!testing)
                {
                    //if ((cbFinalStation.SelectedIndex > -1) && (cbExitPoint.SelectedIndex > -1))
                    if ((cbFinalStation.SelectedIndex > -1))
                    {
                        if ((radioButtonNo.Checked) || ((radioButtonYes.Checked) && cbFinalTrack.SelectedIndex > -1))
                        {
                            btnAddTrain.Enabled = true;
                            return;
                        }

                    }
                    btnAddTrain.Enabled = false;
                }
                else
                {
                    btnAddTrain.Enabled = true;
                    return;

                }

            }
            btnAddTrain.Enabled = false;
        }

        private void cbPickTrain_Click(object sender, EventArgs e)
        {
            //data z JSONu
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();

            // filter the list based on the conditions
            cbPickTrain.Items.Clear();
            foreach (Trains train in trainsList)
            {
                if (train.move == 0)
                {
                    cbPickTrain.Items.Add(train.name);
                }
            }
            CheckValues();
        }

        private void cbPickTrain_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearAllExceptTrain();
            ComboBox cb = (ComboBox)sender;
            string selectedTrainName = (string)cb.SelectedItem;

            //data z JSONu
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();

            foreach (Trains train in trainsList)
            {
                if (train.name == selectedTrainName)
                {
                    //labelStartPosition.Text = train.currentPosition;
                    tbStartPosition.Text = train.currentPosition;
                }
            }
            CheckValues();
        }

        private void getFinalStation(Trains train)
        {
            //string fromStart = null;
            IEnumerable<string> fromStart = null;
            IEnumerable<string> final = null;
            bool crit = false;

            cbFinalStation.Items.Clear();

            if (train.circuit == 0)
            {
                crit = true;
                fromStart = MainLogic.GetStartStationInCritical(train.currentPosition, train.lastPosition);
                //string fromStart = toElement.FirstOrDefault();

            }
            else
            {
                crit = false;
                fromStart = MainLogic.GetStartStationOutside(train.currentPosition, train.lastPosition);
                //string fromStart = toElement.FirstOrDefault();
            }

            bool reverse;
            if (cbDirect.SelectedItem.ToString() == "Direct")
                reverse = false;
            else
                reverse = true;

            if (crit)
            {
                final = MainLogic.GetFinalStationInCritical(train.currentPosition, train.lastPosition);
                //cbFinalStation.Items.Add(final);
            }
            else
            {
                final = MainLogic.GetFinalStationOutside(train.currentPosition, train.lastPosition);
            }


            if (reverse != train.reverse)
            {
                IEnumerable<string> HelpVar = fromStart;
                fromStart = final;
                final = HelpVar;
            }


            foreach (string s in final)
            {
                if (!cbFinalStation.Items.Contains(s))
                    cbFinalStation.Items.Add(s);
            }
        }


        private void getFinalTrack(Trains train)
        {
            cbFinalTrack.Items.Clear();
            IEnumerable<XElement> finTrack = MainLogic.GetFinalTrack(train, cbFinalStation.SelectedItem.ToString());
            foreach (XElement x in finTrack)
                cbFinalTrack.Items.Add(x.Value);
        }

        #region Actions when index of combobox or value of radiobuttons are changed
        private void radioButtonYes_CheckedChanged(object sender, EventArgs e)
        {
            //radioButtonNo.Checked = true;
            CheckValues();
        }

        private void radioButtonNo_CheckedChanged(object sender, EventArgs e)
        {
            //radioButtonNo.Checked = true;
            CheckValues();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void cbSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckValues();
        }

        private void cbDirect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPickTrain.SelectedItem != null)
            {

                foreach (Trains train in trainsList)
                {
                    if (train.name == cbPickTrain.SelectedItem.ToString())
                    {
                        getFinalStation(train);
                    }
                }
            }
            radioButtonNo.Checked = true;
            CheckValues();

        }

        private void cbExitPoint_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Trains train in trainsList)
            {
                if (train.name == cbPickTrain.SelectedItem.ToString())
                {
                    getFinalStation(train);
                }
            }

            CheckValues();
        }

        private void cbFinalStation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPickTrain.SelectedItem != null)
            {

                foreach (Trains train in trainsList)
                {
                    if (train.name == cbPickTrain.SelectedItem.ToString())
                    {
                        getFinalTrack(train);
                    }
                }
            }
            CheckValues();
        }

        private void cbFinalTrack_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckValues();
        }
        #endregion

        private void btnAddTrain_Click(object sender, EventArgs e)
        {
            string final = "";

            if (radioButtonYes.Checked)
                final = cbFinalTrack.SelectedItem.ToString();
            else
                final = cbFinalStation.SelectedItem.ToString();


            string name = cbPickTrain.SelectedItem.ToString();

            string currentPosition = tbStartPosition.Text;

            string spd = cbSpeed.SelectedItem.ToString();
            byte speed = byte.Parse(spd);

            bool reverse;
            if (cbDirect.SelectedItem.ToString() == "Direct")
                reverse = false;
            else
                reverse = true;

            addTrainInfoData(name, currentPosition, speed, reverse, final);
            //addTrainInfoData(cbPickTrain.SelectedItem.ToString(),tbStartPosition.Text,cbSpeed.SelectedItem.ToString(),cbDirect.SelectedItem.ToString(), final);
            ButtonAddLocoClick?.Invoke(this, e);

            ClearData();
        }

        public void addTrainInfoData(string id, string currentPosition, byte speed, bool direction, string final)
        {
            addNewLocoData.Add(new AddDataToSend { Id = id, CurrentPosition = currentPosition, Speed = speed, Reverse = direction, FinalPosition = final });
        }
    }

    public class AddDataToSend
    {
        public string Id { get; set; }
        public string CurrentPosition { get; set; }
        public byte Speed { get; set; }
        public bool Reverse { get; set; }
        public string FinalPosition { get; set; }
    }
}
