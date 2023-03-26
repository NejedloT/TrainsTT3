using ControlLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TestDesignTT
{
    public partial class FormDebug : Form
    {
        UCEditTimetable ucEditTimetable = new UCEditTimetable();
        UCTrainTimetable ucTrainTimetable = new UCTrainTimetable();
        UCEditMoving ucEditMoving = new UCEditMoving();
        UCTrainMoving ucTrainMoving = new UCTrainMoving();
        UCHome uCHome = new UCHome();
        UCTurnouts uCTurnouts = new UCTurnouts();
        UCMap uCMap = new UCMap();
        UCLokomotives uCLocomotives = new UCLokomotives();
        UCMultiTurnout uCmulti = new UCMultiTurnout();
        UCEditJson uCEditJson = new UCEditJson();


        private static List<Trains> trainsList = new List<Trains>();


        public FormDebug()
        {
            InitializeComponent();
            DisplayInstance(uCHome);
            ControlLogic.MainLogic.Initialization();
        }

        private void FormDebug_Load(object sender, EventArgs e)
        {
            uCmulti.ButtonAddClick += new EventHandler(UserControl_ButtonAddClick);
            uCTurnouts.ButtonSendClick += new EventHandler(UserControl_ButtonSaveClick);
            uCLocomotives.ChangeOfTrainData += new EventHandler(UserControl_TrainDataChange);
            uCEditJson.ButtonChangeJsonClick += new EventHandler(UserControl_EditJsonClick);
        }

        private void FormDebug_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                uCMap.setLabels();
                // form is in full screen mode
                // do something here
            }
        }


        private void DisplayInstance(UserControl uc)
        {
            if (!(panelDesktopPanel.Controls.Contains(uc)))
            {
                panelDesktopPanel.Controls.Add(uc);
                uc.Dock = DockStyle.Fill;
                uc.BringToFront();

            }
            else
            {
                uc.BringToFront();
            }
        }



        private void FormDebug_SizeChanged(object sender, EventArgs e)
        {
            if (panelDesktopPanel.Controls.Contains(ucEditMoving))
                ucEditMoving.changeSize();
            if (panelDesktopPanel.Controls.Contains(ucTrainTimetable))
                ucTrainTimetable.changeSize();
            if (panelDesktopPanel.Controls.Contains(ucEditTimetable))
                ucEditTimetable.changeSize();
            if (panelDesktopPanel.Controls.Contains(ucTrainMoving))
                ucTrainMoving.changeSize();
        }

        /*
        private void btn_prev(object sender, EventArgs e)
        {
            //myFileName = MyControls.UCTrainTimetable.fileName;


            //DisplayInstance(UCTrainTimetable.Instance);
            //UCTrainTimetable ucTrainTimetable = new UCTrainTimetable();
            DisplayInstance(uCTurnouts);

            labelTitle.Text = (sender as Button).Text;
            uCTurnouts.clearData();
        }
        */

        private void btnExit_Click(object sender, EventArgs e)
        {
            //TODO - wait, until all locomotives are finished
            FormMainMenu formmm = new FormMainMenu();
            formmm.StartPosition = FormStartPosition.Manual;
            formmm.Location = this.Location;
            formmm.Size = this.Size;
            this.Hide();
            formmm.Show();
            //FormMainMenu.;
        }

        private void btnSections_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCMap);

            labelTitle.Text = (sender as Button).Text;
            uCMap.setLabels();
        }


        private void btnAddLoco_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCLocomotives);

            labelTitle.Text = (sender as Button).Text;
            //uCLocomotives.setLabels();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCHome);

            labelTitle.Text = (sender as Button).Text;
        }

        private void btnTurnouts_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCTurnouts);

            labelTitle.Text = (sender as Button).Text;
            uCTurnouts.clearData();
        }

        private void btnMultiTurnouts_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCmulti);

            labelTitle.Text = (sender as Button).Text;
            uCmulti.ClearData();
        }

        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            //Stop all trains

        }

        private void btnUpdateJson_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCEditJson);
            labelTitle.Text = (sender as Button).Text;
            uCEditJson.ClearData();
        }

        protected void UserControl_TrainDataChange(object sender, EventArgs e)
        {
            List<ChangeTrainData> trainDataChange = uCLocomotives.trainDataChange;

            bool foundMatch = false;

            foreach (Trains train in trainsList)
            {
                foreach (ChangeTrainData data in trainDataChange)
                {
                    if (train.name == data.Lokomotive)
                    {
                        //train.direction = data.Reverze;
                        foundMatch = true;
                        break;
                    }
                }
                if (foundMatch)
                    break;
            }
            StoreJson sj = new StoreJson();
            sj.SaveJson(trainsList);
            //TODO - send packet
        }

        protected void UserControl_EditJsonClick(object sender, EventArgs e)
        {
            List<ChangeJsonData> changeData = uCEditJson.changeJsonData;
            //TODO - save into JSON
            trainsList = ControlLogic.MainLogic.GetData();

            bool foundMatch = false;

            foreach (Trains train in trainsList)
            {
                foreach (ChangeJsonData data in changeData)
                {
                    if (train.name == data.Id)
                    {
                        train.currentPosition = data.CurrentPosition;
                        train.lastPosition = data.PreviousPosition;
                        train.direction = data.Direction;
                        foundMatch = true;
                        break;
                    }
                }
                if (foundMatch) 
                    break;
            }
            StoreJson sj = new StoreJson();
            sj.SaveJson(trainsList);
            changeData.Clear();
        }

        protected void UserControl_ButtonAddClick(object sender, EventArgs e)
        {
            //handle the event
            //TODO - send packets (or maybe firtstly just check to values? But probably not needed)
            List<Turnouts> turnouts = uCmulti.turnouts;
            for (int i = 0; i < turnouts.Count; i++)
            {

            }
            turnouts.Clear();
        }

        protected void UserControl_ButtonSaveClick(object sender, EventArgs e)
        {
            //TODO
            //Get values from list and send packets
            //Update

            List<Turnouts> turnouts = uCTurnouts.turnouts;
            for (int i = 0; i < turnouts.Count; i++)
            {

            }
            turnouts.Clear();
        }
    }
}
