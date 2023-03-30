using ControlLogic;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using TrainTTLibrary;
using static TrainTTLibrary.Packet;

namespace TestDesignTT
{
    public partial class FormDebug : Form
    {
        private bool IsConnect = false;

        private static TCPClient klient = null;


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
        UCOccupancy uCOccupancy = new UCOccupancy();
        UCUnitSet uCUnitSet = new UCUnitSet();


        private static List<Trains> trainsList = new List<Trains>();


        public FormDebug()
        {
            InitializeComponent();
            DisplayInstance(uCHome);
            ControlLogic.MainLogic.Initialization();

            //List<Section> si = SectionInfo.listOfSection;
        }

        #region Form actions (Load, Init, Closed, Resized)
        private void FormDebug_Load(object sender, EventArgs e)
        {
            uCmulti.MultiTurnoutButtonAddClick += new EventHandler(UserControl_MultiTurnoutClick);
            uCTurnouts.TurnoutButtonSendClick += new EventHandler(UserControl_TurnoutClick);
            uCLocomotives.ChangeOfTrainData += new EventHandler(UserControl_TrainDataChange);
            uCEditJson.ButtonChangeJsonClick += new EventHandler(UserControl_EditJsonClick);
            uCUnitSet.UnitInstructionEventClick += new EventHandler(UserControl_UnitInstructionClick);
            StartTCPClient();
        }

        private void FormDebug_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsConnect)
            {
                StopAll();
            }

            Thread.Sleep(30);


            if (klient != null)
            {
                klient.Dispose();
                klient = null;
            }
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

        #endregion

        #region TCP server (Start, Connect, Disconnect, Send Data)
        private void StartTCPClient()
        {

            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];

            klient = new TCPClient(ipAddress, 8080);

            klient.DataType = eRecvDataType.dataStringNL;
            klient.OnClientConnected += KlientConnected;
            klient.OnClientDisconnected += TCPDisconnectClient;
            if (!klient.Connect())
            {
                KlientCleanUp();
            }
        }

        private void TCPDisconnectClient(object sender, TCPClientConnectedEventArgs e)
        {
            IsConnect = false;
        }

        private void KlientConnected(object sender, TCPClientConnectedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, TCPClientConnectedEventArgs>(KlientConnected),
                  new object[] { sender, e });
                return;
            }


            if (e == null)
            {
                IsConnect = false;

                KlientCleanUp();
            }
            else
            {
                //log.Add("Client connected to " + e.clientIPE);
                IsConnect = true;
            }
        }

        private void KlientCleanUp()
        {
            if (klient != null)
            {
                klient.Disconnect();

                klient.OnClientConnected -= KlientConnected;
                klient.OnClientDisconnected -= TCPDisconnectClient;

                klient.Dispose();
                klient = null;
            }
        }

        private void SendTCPData(string str)
        {
            while ((!IsConnect) || !klient.Send(str))
            {
                if (DialogResult.Cancel == MessageBox.Show("Server not found\nDo you want to try to reconnect?", "Error: Server not found", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning))
                {

                    Close();

                }

                StartTCPClient();
                Thread.Sleep(200);

            }

        }

        #endregion

        #region Definition of buttons in the left menu and user control

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

        private void btnUpdateJson_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCEditJson);
            labelTitle.Text = (sender as Button).Text;
            uCEditJson.ClearData();
        }

        private void btnOccupancy_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCOccupancy);
            labelTitle.Text = (sender as Button).Text;

            //List<Section> occupancySections = TCPServerTrainTT.Program.GetOccupancySections();
            List<Section> occupancySections = TrainTTLibrary.SectionInfo.listOfSection;

        }

        private void btnUnitInstruction_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCUnitSet);
            labelTitle.Text = (sender as Button).Text;
        }


        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            //Stop all trains
            StopAll();
        }


        #endregion

        private void StopAll()
        {
            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {

                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(locomotive, false, 3);

                SendTCPData(trainMotionPacket.TCPPacket);
            }

        }

        protected void UserControl_TrainDataChange(object sender, EventArgs e)
        {
            List<ChangeTrainData> trainDataChange = uCLocomotives.trainDataChange;

            bool foundMatch = false;

            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {
                string loco = locomotive.Name.ToString();

                foreach (ChangeTrainData data in trainDataChange)
                {
                    string loc = Packet.GapToUnderLine(data.Lokomotive);

                    if (loco == loc)
                    {
                        //train.direction = data.Reverze;
                        if (data.StartStop && data.Speed > 3)
                        {
                            bool reverse = data.Reverze;

                            byte speed = data.Speed;

                            TrainMotionPacket trainMotionPacket = new TrainMotionPacket(locomotive, reverse, speed);

                            SendTCPData(trainMotionPacket.TCPPacket);

                            //int pr = 220;

                            //byte prodleva = (byte)pr;


                            //UnitInstructionPacket unitInst = new UnitInstructionPacket(Packet.unitInstruction.precteni_stavu_jednotky, 3, 0xFA);
                            //SendTCPData(unitInst.TCPPacket);

                            //funkcni zmena rychlosti zasilani dat
                            //UnitInstructionPacket unitInst = new UnitInstructionPacket(Packet.unitInstruction.prodleva_odesilani_zmerenych_proudu, 3, 0xFA);
                            //SendTCPData(unitInst.TCPPacket);



                            //H mustek restart v poradku!!
                            //UnitInstructionPacket unitInst = new UnitInstructionPacket(Packet.unitInstruction.restart_H_mustku, 3, 1);
                            //SendTCPData(unitInst.TCPPacket);


                        }
                        else
                        {
                            TrainMotionPacket trainMotionPacket = new TrainMotionPacket(locomotive, false, 0);

                            SendTCPData(trainMotionPacket.TCPPacket);
                        }
                        foundMatch = true;
                        break;
                    }
                }
                if (foundMatch)
                    break;
            }
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

        protected void UserControl_MultiTurnoutClick(object sender, EventArgs e)
        {
            //handle the event
            //TODO - send packets (or maybe firtstly just check to values? But probably not needed)
            List<Turnouts> turnouts = uCmulti.turnouts;
            for (int i = 0; i < turnouts.Count; i++)
            {
                uint numberOfUnit = turnouts[i].UnitID;

                byte data1 = turnouts[i].Change;

                byte data2 = turnouts[i].Position;

                TurnoutInstructionPacket turnoutInstructionPacket = new TurnoutInstructionPacket(Packet.turnoutInstruction.nastaveni_vyhybky, numberOfUnit, data1, data2);

                SendTCPData(turnoutInstructionPacket.TCPPacket);
            }
            turnouts.Clear();
        }

        protected void UserControl_TurnoutClick(object sender, EventArgs e)
        {

            List<Turnouts> turnouts = uCTurnouts.turnouts;
            for (int i = 0; i < turnouts.Count; i++)
            {
                uint numberOfUnit = turnouts[i].UnitID;

                byte data1 = turnouts[i].Change;

                byte data2 = turnouts[i].Position;

                
                TurnoutInstructionPacket turnoutInstructionPacket = new TurnoutInstructionPacket(Packet.turnoutInstruction.nastaveni_vyhybky, numberOfUnit, data1, data2);

                SendTCPData(turnoutInstructionPacket.TCPPacket);
                
                
                
                //UnitInstructionPacket unitInst = new UnitInstructionPacket(Packet.unitInstruction.nastaveni_zdroje, 3, 0);
                
                //SendTCPData(unitInst.TCPPacket);
                
            }
            turnouts.Clear();
        }

        protected void UserControl_UnitInstructionClick(object sender, EventArgs e)
        {
            List<SetNewUnitData> newUnit = uCUnitSet.newUnit;

            for (int i = 0; i < newUnit.Count; i++)
            {
                unitInstruction ui = newUnit[i].Unit;

                byte data0 = newUnit[i].Data0;

                byte data1 = newUnit[i].Data1;

                UnitInstructionPacket unitInst = new UnitInstructionPacket(ui, data0, data1);

                SendTCPData(unitInst.TCPPacket);

            }
            newUnit.Clear();

        }
    }
}
