using ControlLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TestDesignTT
{
    public partial class UCEditJson : UserControl
    {

        private LoadJson loadJson;


        private static List<Trains> trainsList = new List<Trains>();

        //private static IEnumerable<string> nextPositions = new List<string>();

        public List<ChangeJsonData> changeJsonData = new List<ChangeJsonData>();

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonChangeJsonClick;

        public UCEditJson()
        {
            InitializeComponent();
            ClearData();
            checkAddButton();
        }

        public void ClearData()
        {
            cbPickTrain.SelectedIndex = -1;
            ClearAllExceptTrain();
            checkAddButton();
        }

        public void ClearAllExceptTrain()
        {
            cbCurrentPosition.SelectedIndex = -1;
            cbDirection.SelectedIndex = -1;
            cbPreviousPosition.SelectedIndex = -1;
            checkAddButton();
        }

        public void checkAddButton()
        {
            if ((cbPickTrain.SelectedIndex > -1) &&
                    (cbCurrentPosition.SelectedIndex > -1) &&
                    (cbPreviousPosition.SelectedIndex > -1) &&
                    (cbDirection.SelectedIndex > -1))
            {
                btnSaveData.Enabled = true;
            }
            else
            {
                btnSaveData.Enabled = false;
            }

            if (cbCurrentPosition.SelectedIndex > -1)
            {
                cbPreviousPosition.Enabled = true;
            }
            else
            {
                cbPreviousPosition.Enabled = false;
            }
        }

        private void cbPickTrain_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkAddButton();
        }

        private void cbCurrentPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            //find the others 2 parts that are next to the current part
            IEnumerable<string> nextPositions = ControlLogic.MainLogic.GetNextPositions(cbCurrentPosition.SelectedItem.ToString());
            cbPreviousPosition.Items.Clear();
            foreach (string position in nextPositions)
            {
                //if (cbPreviousPosition.Items.Contains(position))
                cbPreviousPosition.Items.Add(position);
            }
            checkAddButton();
        }

        private void cbPreviousPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkAddButton();
        }

        private void cbDirection_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkAddButton();
        }

        private void cbPickTrain_Click(object sender, EventArgs e)
        {
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
            checkAddButton();
        }

        private void cbCurrentPosition_Click(object sender, EventArgs e)
        {
            //get all parts with current consumption
            //// In Package A:  
            //PackageB.PackageC.ClassName.FunctionName();
            cbCurrentPosition.Items.Clear();
            cbCurrentPosition.Items.Add("id3");
            cbCurrentPosition.Items.Add("id1");
            cbCurrentPosition.Items.Add("id199");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearData();
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            addJsonChange(cbPickTrain.SelectedItem.ToString(), cbCurrentPosition.SelectedItem.ToString(), cbPreviousPosition.SelectedItem.ToString(), cbDirection.SelectedItem.ToString());
            ButtonChangeJsonClick?.Invoke(this, e);
        }



        public void addJsonChange(string id, string currentPosition, string previousPosition, string direction)
        {
            changeJsonData.Add(new ChangeJsonData { Id = id, CurrentPosition = currentPosition, PreviousPosition = previousPosition, Direction = direction });
        }
    }

    public class ChangeJsonData
    {
        public string Id { get; set; }
        public string CurrentPosition { get; set; }
        public string PreviousPosition { get; set; }
        public string Direction { get; set; }
    }
}
