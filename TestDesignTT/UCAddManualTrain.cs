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
        private LoadJson loadJson;

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
            /*
            LoadJson loadJson = new LoadJson();
            List<Trains> trainsList = loadJson.data;
            this.loadJson = loadJson;
            */

            //dodelano
            trainsList = ControlLogic.MainLogic.GetData();

            // filter the list based on the conditions
            //List<string> names = new List<string>();
            cbPickTrain.Items.Clear();
            foreach (Trains train in trainsList)
            {
                if (train.move == "0")
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

            //List<Trains> trainsList = loadJson.data;

            //dodelano
            trainsList = ControlLogic.MainLogic.GetData();

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
            fromCircuit = ControlLogic.MainLogic.getFromCircuit(train);
            IEnumerable<XElement> toCircuits = fromCircuit.Descendants("toCircuit")
                            .Where(e => e.Elements().Any(c => (string)c == train.currentPosition));

            IEnumerable<string> toNames = toCircuits.Select(e => (string)e.Attribute("name"));

            cbFinalStation.Items.Clear();
            if (!toNames.Any())
            {
                if (cbDirect.SelectedItem.ToString() == train.direction)
                {
                    string[] validPositions = { "Beroun", "Karlstejn", "Lhota" };
                    if (validPositions.Contains(train.finalPosition))
                    {
                        
                        cbFinalStation.Items.Add(train.finalPosition);
                    }
                    else
                    {
                        IEnumerable<XElement> idToFind = ControlLogic.MainLogic.GetFinalStation(train.finalPosition);
                        foreach (XElement toNameElement in toCircuits.Descendants("item").Where(e => (string)e == idToFind.ToString()))
                        {
                            string toName = (string)toNameElement.Parent.Attribute("name");
                            cbFinalStation.Items.Add(toName);
                        }
                    }
                }
                else
                {
                    string[] validPositions = { "Beroun", "Karlstejn", "Lhota" };
                    if (validPositions.Contains(train.startPosition))
                    {
                        string final = train.finalPosition;
                        cbFinalStation.Items.Add(train.startPosition);
                    }
                    else
                    {
                        IEnumerable<XElement> idToFind = ControlLogic.MainLogic.GetFinalStation(train.finalPosition);
                        foreach (XElement toNameElement in toCircuits.Descendants("item").Where(e => (string)e == idToFind.ToString()))
                        {
                            string toName = (string)toNameElement.Parent.Attribute("name");
                            cbFinalStation.Items.Add(toName);
                        }
                    }
                }
            }
            else
            {
                int i = 0;
                foreach (string toName in toNames)
                {
                    if ((toName == "Lhota" && (((train.mapOrientation == "prevConnection") && (cbDirect.SelectedItem.ToString() == train.direction))
                        || ((train.mapOrientation == "nextConnection") && (cbDirect.SelectedItem.ToString()) != train.direction))))
                        i++;
                    else
                        cbFinalStation.Items.Add(toName);
                }
            }
            /*
            routes = ControlLogic.MainLogic.GetFromElements(train);
            if (cbDirect.SelectedIndex < 0)
            {
                cbExitPoint.Items.Clear();

                foreach (XElement element in routes)
                {
                    foreach (XElement toElement in element.Elements("to"))
                    {
                        //vraci postupne jednotlive id definovanych finalnich koleji
                        string getToId = (string)toElement.Attribute("id");

                        //najdi, jestli ma nejaky vlak stejnou finalni pozici (ta bude zmenena pri vytvoreni prikazu pro jizdu),
                        //takze by se cekalo, nez vlak odjede
                        var reserveSections = toElement.Element("parts")
                                   .Elements()
                                   .Select(e => e.Value)
                                   .ToList();

                        //pokud to obsahuje pozici a orientace bude opacna
                        if (reserveSections.Any(t => t.Contains(train.lastPosition) && cbDirect.SelectedItem.ToString() != train.direction))
                        {
                            cbExitPoint.Items.Add(getToId);
                        }
                        else if ((!(reserveSections.Any(t => t.Contains(train.lastPosition))) && cbDirect.SelectedItem.ToString() == train.direction))
                        {
                            cbExitPoint.Items.Add(getToId);
                        }
                    }
                }
            }
            else
            {
                IEnumerable<XElement> matchingRoutes = routes
                    .Where(r => (string)r.Element("from").Attribute("id") == tbStartPosition.Text
                    && (string)r.Element("to").Attribute("id") == cbExitPoint.SelectedItem.ToString());

                foreach (XElement route in matchingRoutes)
                {
                    // Access the values in the XML elements here
                    string toFinal = (string)route.Element("to").Element("toFinal");
                    int circuit = (int)route.Element("to").Element("circuit");

                    List<string> parts = route.Element("to").Element("parts")
                        .Elements()
                        .Select(e => (string)e)
                        .ToList();
                }
            }
            */

        }
        private void getFinalTrack(Trains train)
        {
            cbFinalTrack.Items.Clear();
            IEnumerable<XElement> finTrack = ControlLogic.MainLogic.GetFinalTrack(train, cbFinalStation.SelectedItem.ToString());
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

            addTrainInfoData(cbPickTrain.SelectedItem.ToString(),tbStartPosition.Text,cbSpeed.SelectedItem.ToString(),cbDirect.SelectedItem.ToString(), final);
            ButtonAddLocoClick?.Invoke(this, e);
        }

        public void addTrainInfoData(string id, string currentPosition, string speed, string direction, string final)
        {
            addNewLocoData.Add(new AddDataToSend { Id = id, CurrentPosition = currentPosition, Speed = speed, Direction = direction, FinalPosition = final });
        }
    }

    public class AddDataToSend
    {
        public string Id { get; set; }
        public string CurrentPosition { get; set; }
        public string Speed { get; set; }
        public string Direction { get; set; }
        public string FinalPosition { get; set; }
    }
}
