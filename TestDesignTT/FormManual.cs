using ControlLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
    public partial class FormManual : Form
    {
        private bool IsConnect = false;

        private static TCPClient klient = null;

        UCHome uCHome = new UCHome();
        UCMap uCMap = new UCMap();
        UCAddManualTrain uCAddManual = new UCAddManualTrain();
        UCJsonDisplay uCdisplayJson = new UCJsonDisplay();
        UCSettings uCSettings = new UCSettings();
        UCUnitSet uCUnitSet = new UCUnitSet();
        UCTurnoutsSettings uCTurnoutSet = new UCTurnoutsSettings();

        //MainLogic ml = new MainLogic();
        MainLogic ml = new MainLogic();

        private static List<Trains> trainsList = new List<Trains>();

        public FormManual()
        {
            InitializeComponent();
            DisplayInstance(uCHome);

            //ml = new MainLogic(); // initialize the MainLogic object
            //MainLogic.Initialization(); // pass the MainLogic object as a parameter

            //MainLogic.Initialization();

            ml.LocomotiveDataEvent += new EventHandler<LocomotiveDataSend>(EventHandlerNewLocoData);

            MainLogic.Initialization(ml);
            //MainLogic.TestEventHandler(ml);
        }

        private void FormManual_Load(object sender, EventArgs e)
        {
            uCAddManual.ButtonAddLocoClick += new EventHandler(UserControl_ButtonAddLocoClick);
            uCUnitSet.UnitInstructionEventClick += new EventHandler(UserControl_UnitInstructionClick); //user control pro zmenu nastaveni ridici jednotky
            uCTurnoutSet.TurnoutDefinitionStopsClick += new EventHandler(UserControl_TurnoutInstructionStops);
            uCTurnoutSet.TurnoutInstructionSetClick += new EventHandler(UserControl_TurnoutInstructionSet);

            //ml.LocomotiveDataEvent += new EventHandler<LocomotiveDataSend>(EventHandlerNewLocoData);

            StartTCPClient();
        }


        private void FormManual_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                uCMap.setLabels();
                // form is in full screen mode
            }
        }

        private void FormManual_FormClosing(object sender, FormClosingEventArgs e)
        {
            //vypni timery - logika nebude bezet dale
            if (IsConnect)
            {
                StopAll();
            }

            //zastav timery, jak dlouho neposlali ridici jednotky zadna data
            /*
            foreach (var timer in timersByNumberOfUnit.Values)
            {
                timer.Stop();
            }
            */

            Thread.Sleep(350);

            MainLogic.StopTimers();

            Thread.Sleep(350);

            //vycisti klienta
            KlientCleanUp();
            if (klient != null)
            {
                klient.Dispose();
                klient = null;
            }
        }

        private void FormManual_SizeChanged(object sender, EventArgs e)
        {
            //Potentially TODO
        }

        #region TCP server (Start, Connect, Disconnect, Send Data)
        private void StartTCPClient()
        {
            //vytvoreni TCP klienta
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            klient = new TCPClient(ipAddress, 8080);

            //nastaveni eventu pri spusteni ci odpojeni klienta a pro prijem dat
            klient.DataType = eRecvDataType.dataStringNL;
            klient.OnClientConnected += KlientConnected;
            klient.OnClientDisconnected += TCPDisconnectClient;
            klient.DataReceived += TCP_DataRecv;

            //pokud se nepripojim, vycisti klienta
            if (!klient.Connect())
            {
                KlientCleanUp();
            }
        }

        /// <summary>
        /// akce v pripade odpojeni klienta
        /// </summary>
        /// <param name="sender">Event handler pri odpojeni</param>
        /// <param name="e">Event handler pri odpojeni</param>
        private void TCPDisconnectClient(object sender, TCPClientConnectedEventArgs e)
        {
            IsConnect = false;
        }

        /// <summary>
        /// akce v pripade pripojeni klienta
        /// </summary>
        /// <param name="sender">Event handler pri pripojeni klienta</param>
        /// <param name="e">Event handler pri pripojeni klienta</param>
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
                IsConnect = true;
            }
        }

        /// <summary>
        /// vycisti klienta, pokud byl napr. odpojen
        /// </summary>
        private void KlientCleanUp()
        {
            if (klient != null)
            {
                klient.Disconnect();

                klient.OnClientConnected -= KlientConnected;
                klient.OnClientDisconnected -= TCPDisconnectClient;
                klient.DataReceived -= TCP_DataRecv;

                klient.Dispose();
                klient = null;
            }
        }

        /// <summary>
        /// Event na to, ze prislo neco ze serioveho portu
        /// </summary>
        /// <param name="sender">Event handler na prichodi data ze serioveho portu</param>
        /// <param name="e">Event handler na prichodi data ze serioveho portu</param>
        private void TCP_DataRecv(object sender, TCPReceivedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<TCPReceivedEventArgs>(DataProcessing), new object[] { e });
                return;
            }

        }

        /// <summary>
        /// zpracovani dat prijatych
        /// </summary>
        /// <param name="e">Event na prichozi data z portu</param>
        private void DataProcessing(TCPReceivedEventArgs e)
        {
            ProcessDataFromTCP.ProcessData(e);

            bool testError = ProcessDataFromTCP.getErrors();

            if (testError)
            {
                StopAll();
                ProcessDataFromTCP.setErrors(false);
            }
        }


        /// <summary>
        /// Metoda pro posilani dat
        /// </summary>
        /// <param name="str">TCP packet ve forme stringu</param>
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

        #region Click events on buttons
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

        private void btnSettings_Click(object sender, EventArgs e)
        {
            UCSettings uCSettings = new UCSettings();

            DisplayInstance(uCSettings);

            labelTitle.Text = (sender as Button).Text;
        }

        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            StopAll();
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

        #endregion




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


        private void StopAll()
        {
            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(locomotive, false, 0);

                SendTCPData(trainMotionPacket.TCPPacket);
            }
        }

        /// <summary>
        /// Metoda vyvolana Event Handlerem na zmenu nastaveni ridici jednotky
        /// Prisel pozadavek z user controlu. Data budou vlozeno do packetu a zaslana
        /// </summary>
        /// <param name="sender">Event Handler z user controlu</param>
        /// <param name="e">Event Handler z user controlu</param>
        protected void UserControl_UnitInstructionClick(object sender, EventArgs e)
        {
            List<SetNewUnitData> newUnit = uCUnitSet.newUnit;

            //vlozeni prijatych dat do promennych, vytvoreni packetu a zaslani
            for (int i = 0; i < newUnit.Count; i++)
            {
                unitInstruction ui = newUnit[i].Type;

                byte data0 = newUnit[i].NumberOfUnit;

                byte data1 = newUnit[i].Data;

                UnitInstructionPacket unitInst = new UnitInstructionPacket(ui, data0, data1);

                SendTCPData(unitInst.TCPPacket);

            }

            //smazani prijatych dat pro nastaveni ridici jednotky
            newUnit.Clear();
        }

        /// <summary>
        /// Metoda vyvolana Event Handlerem na zmenu nastaveni softwarovych dorazu
        /// Prisel pozadavek na nove nastaveni dorazu nejake vyhybky
        /// Data budou vlozena do packetu a zaslana prislusne jednotce
        /// </summary>
        /// <param name="sender">Event Handler z user controlu</param>
        /// <param name="e">Event Handler z user controlu</param>
        protected void UserControl_TurnoutInstructionStops(object sender, EventArgs e)
        {
            List<SetNewTurnoutStops> newTurnoutStops = uCTurnoutSet.newTurnoutStops;

            for (int i = 0; i < newTurnoutStops.Count; i++)
            {
                turnoutInstruction ti = newTurnoutStops[i].Type;

                byte numberOfUnit = newTurnoutStops[i].NumberOfUnit;

                byte numberOfTurnout = newTurnoutStops[i].NumberOfTurnout;

                byte left = newTurnoutStops[i].LeftStop;

                byte right = newTurnoutStops[i].RightStop;

                TurnoutInstructionPacket turnoutInst = new TurnoutInstructionPacket(ti, numberOfUnit, numberOfTurnout, left, right);

                SendTCPData(turnoutInst.TCPPacket);

            }

            newTurnoutStops.Clear();
        }

        /// <summary>
        /// Metoda vyvolana Event Handlerem na zmenu nastaveni jednotky vyhybek (mimo dorazy a zmeny polohy vyhybek)
        /// Prisel pozadavek na nastaveni nejake turnout instruction
        /// Data budou vlozena do packetu a zaslana prislusne jednotce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UserControl_TurnoutInstructionSet(object sender, EventArgs e)
        {
            List<SetNewTurnoutUnitData> newTurnoutData = uCTurnoutSet.newTurnoutData;

            for (int i = 0; i < newTurnoutData.Count; i++)
            {
                turnoutInstruction ti = newTurnoutData[i].Type;

                byte numberOfUnit = newTurnoutData[i].NumberOfUnit;

                byte data = newTurnoutData[i].Data;

                TurnoutInstructionPacket turnoutInst = new TurnoutInstructionPacket(ti, numberOfUnit, data);

                SendTCPData(turnoutInst.TCPPacket);
            }
            newTurnoutData.Clear();
        }

        protected void EventHandlerNewLocoData(object sender, LocomotiveDataSend e)
        {
            Locomotive loco = e.Loco;
            bool reverse = e.Reverze;
            byte speed = e.Speed;

            for (int i = 0; i < MainLogic.switchesChange.Count(); i++)
            {
                if (MainLogic.switchesChange[i].TrainId == loco.ID)
                {
                    uint numberOfUnit = MainLogic.switchesChange[i].NumberOfUnit;

                    byte data1 = MainLogic.switchesChange[i].Turnouts;

                    byte data2 = MainLogic.switchesChange[i].Value;

                    TurnoutInstructionPacket turnoutInstructionPacket = new TurnoutInstructionPacket(Packet.turnoutInstruction.nastaveni_vyhybky, numberOfUnit, data1, data2);

                    SendTCPData(turnoutInstructionPacket.TCPPacket);

                    MainLogic.switchesChange.RemoveAt(i);
                    i--;
                }
            }

            if (speed > 3)
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(loco, reverse, speed);

                SendTCPData(trainMotionPacket.TCPPacket);

                TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(loco, true);

                SendTCPData(trainFunctionPacket.TCPPacket);
            }

            else
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(loco, false, 3);

                SendTCPData(trainMotionPacket.TCPPacket);

                TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(loco, false);

                SendTCPData(trainFunctionPacket.TCPPacket);
            }
        }
    }
}
