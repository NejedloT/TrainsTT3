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
    public partial class FormManual : Form
    {

        UCHome uCHome = new UCHome();
        UCMap uCMap = new UCMap();
        UCAddManualTrain uCAddManual = new UCAddManualTrain();
        UCJsonDisplay uCdisplayJson = new UCJsonDisplay();

        MainLogic ml = new MainLogic();


        public FormManual()
        {
            InitializeComponent();
            DisplayInstance(uCHome);
            MainLogic.Initialization();
        }

        private void FormManual_Load(object sender, EventArgs e)
        {
            uCAddManual.ButtonAddLocoClick += new EventHandler(UserControl_ButtonAddLocoClick);

            ml.LocomotiveDataEvent += new EventHandler<LocomotiveDataSend>(EventHandlerNewLocoData);

        }

        protected void EventHandlerNewLocoData(object sender, LocomotiveDataSend e)
        {
            //Locomotive loco = e.L
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

        private void btnAddLoco_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCAddManual);

            //ControlLogic.MainLogic.controlLogic();

            labelTitle.Text = (sender as Button).Text;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCHome);

            labelTitle.Text = (sender as Button).Text;
        }

        private void btnSections_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCMap);

            labelTitle.Text = (sender as Button).Text;
        }

        private void btnJSON_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCdisplayJson);

            labelTitle.Text = (sender as Button).Text;

            uCdisplayJson.displayJson();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //TODO - wait, until all locomotives are finished
            FormMainMenu formmm = new FormMainMenu();
            formmm.StartPosition = FormStartPosition.Manual;
            formmm.Location = this.Location;
            formmm.Size = this.Size;
            this.Close();
            formmm.Show();
            //FormMainMenu.;
        }

        protected void UserControl_ButtonAddLocoClick(object sender, EventArgs e)
        {
            List<AddDataToSend> addData = uCAddManual.addNewLocoData;

            foreach (var item in addData)
            {
                //updating values in json
                MainLogic.addNewTrainDataFromClient(item.Id, item.CurrentPosition,item.Speed, item.Reverse,item.FinalPosition);
            }
            addData.Clear();
        }

        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            //Stop everything

        }

        private void FormManual_Resize(object sender, EventArgs e)
        {

        }

        private void FormManual_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void FormManual_SizeChanged(object sender, EventArgs e)
        {
            //vypni timery - logika nebude bezet dale
            MainLogic.StopTimers();
        }
    }
}
