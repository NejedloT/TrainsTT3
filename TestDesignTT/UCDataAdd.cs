using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDesignTT
{
    public partial class UCDataAdd : UserControl
    {
        /*
        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user changed combobox")]
        public event EventHandler OnSelectedIndexChanged;
        */

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonYesAddTrainClick;

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonNoAddTrainClick;
        //public BindingList<MovingInTimetamble> movingTimetable = 

        public DataTimetable addData = new DataTimetable();
        //public newDataToAdd addTrain = new newDataToAdd();

        private string myType;
        private string myStartSTation;
        private string myFinalSTation;
        private DateTime myDeparture; // čas odjezdu
        private double mySpeed;
        private bool myReverse;
        private uint WaitTime;

        public string Type { set { myType = value; } get { return myType; }}
        public string StartStation { set { myStartSTation = value; } get { return myStartSTation; } }
        public string FinalStation { set { myFinalSTation = value; } get { return myFinalSTation; } }
        public DateTime Departure { set { myDeparture = value; } get { return myDeparture; } }
        public double Speed { set { mySpeed = value; } get { return mySpeed; } }
        public bool Reverse { set { myReverse = value; } get { return myReverse; } }



        public UCDataAdd()
        {
            InitializeComponent();
            disableAllButtons(true);
            //clearData();
            //cbFinalPlace.SelectedIndex = -1;
        }

        public void clearData()
        {
            cbTrain.SelectedIndex = -1;
            cbFinalPlace.SelectedIndex = -1;
            cbDirection.SelectedIndex = -1;
            cbSpeed.SelectedIndex = -1;
            cbStart.SelectedIndex = -1;
            disableAllButtons(true);
            //addButtonReady();
        }

        private void disableAllButtons(bool bb)
        {
            if (bb)
            {
                cbFinalPlace.Enabled = false;
                cbStart.Enabled = false;
                cbSpeed.Enabled = false;
                cbDirection.Enabled = false;
                cbTime.Enabled = false;
                btnAddTrain.Enabled = false;
            }
            else
            {
                cbFinalPlace.Enabled = true;
                cbStart.Enabled = true;
                cbSpeed.Enabled = true;
                cbDirection.Enabled = true;
                //cbTime.Enabled = false;
            }
        }

        private void disableTime(bool bb)
        {
            if (bb)
                cbTime.Enabled = false;
            else
                cbTime.Enabled = true;
        }
        private void addButtonReady()
        {
            //bool ready = false;
            if (!(cbStart.SelectedIndex > -1))
                return;
            if (!(cbSpeed.SelectedIndex > -1))
                return;
            if (!(cbDirection.SelectedIndex > -1))
                return;
            if (!(cbFinalPlace.SelectedIndex > -1))
                return;
            if (!(cbTrain.SelectedIndex > -1))
                return;
            btnAddTrain.Enabled = true;
        }



        private void cbTrain_SelectedIndexChanged(object sender, EventArgs e)
        {
            disableAllButtons(false);
            //addData.Type = (string)cbTrain.SelectedValue;
            //string aaa = cbTrain.FindString(cbTrain.SelectedValue); SelectedItem

            //this.Type = (string)cbTrain.SelectedValue;
            this.Type = (string)cbTrain.SelectedItem;

            addButtonReady();

            //TODO: momentalne natvrdo, ale predelat na hledani soucasne polohy
            //TODO: Dodelat Collections pro cilove stanice a pro vlaky umoznujici vyber (pouze nejedouci)
        }

        private void cbFinalPlace_SelectedIndexChanged(object sender, EventArgs e)
        {
            //addData.FinalStation = (string)cbFinalPlace.SelectedValue;
            this.StartStation = "Kolin";
            this.FinalStation = (string)cbFinalPlace.SelectedItem;
            addButtonReady();
        }

        private void cbStart_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cbStart.FindString("Later");
            if (cbStart.SelectedIndex == index)
            {
                disableTime(false);
            }
            else
            {
                disableTime(true);
                //addData.Departure = default(DateTime);
                this.Departure = default(DateTime);

            }
            addButtonReady();
        }

        private void cbSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            double number;
            string aaa = (string)cbSpeed.SelectedItem;
            Double.TryParse(aaa, out number);
            //addData.Speed = number;
            this.Speed = number;
            */
            //this.Speed = (double)cbSpeed.SelectedItem;

            //string itm = "a";
            if (cbSpeed.SelectedIndex >= 0 && cbSpeed.SelectedItem != null) {
                string itm = cbSpeed.SelectedItem.ToString();
                double number = Convert.ToDouble(itm);
                this.Speed = number;
            }
            

            addButtonReady();
            //addData.Speed = (double)cbSpeed.SelectedValue;
        }

        private void cbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cbDirection.FindString("Ahead");

            if (cbStart.SelectedIndex == index)
                //if ((string)cbDirection.SelectedValue == "Ahead")
                //addData.Reverse = false;
                this.Reverse = false;
            else
                //addData.Reverse = true;
                this.Reverse = true;
            addButtonReady();
        }

        private void cbTime_ValueChanged(object sender, EventArgs e)
        {
            //addData.Departure = (DateTime)cbTime.Value;
            this.Departure = (DateTime)cbTime.Value;
        }

        private void btnAddTrain_Click(object sender, EventArgs e)
        {
            //cbSpeed.SelectedItem = null;
            ButtonYesAddTrainClick?.Invoke(this, e);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ButtonNoAddTrainClick?.Invoke(this, e);
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
