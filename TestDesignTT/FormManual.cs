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
using System.Timers;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using TrainTTLibrary;
using static TrainTTLibrary.Packet;

namespace TestDesignTT
{
    public partial class FormManual : Form
    {
        private bool IsConnect = false;

        private static TCPClient klient = null;

        private static System.Timers.Timer timeToInitSoftwareStops;

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

            SearchLogic.InitSearch();

            panelSettings.Visible = false;

            ml.InfoMessageEvent += new EventHandler<InfoMessageSend>(EventHandlerNewMsgData);
            ml.LocomotiveDataEvent += new EventHandler<LocomotiveDataSend>(EventHandlerNewLocoData);
            ml.TurnoutsDataEvent += new EventHandler<TurnoutsDataSend>(EventHandlerNewTurnoutData);

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

            timeToInitSoftwareStops = new System.Timers.Timer(1000);

            // Set the event handler for the Elapsed event
            timeToInitSoftwareStops.Elapsed += (sender, e) => Timer_Elapsed_SW_Stops(sender, e);

            // Start the timer
            timeToInitSoftwareStops.Start();

            //vyhybky zbyle 6, 7 a 8 maji problemy s mechanikou
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

            Thread.Sleep(350);

            MainLogic.StopTimers();

            Thread.Sleep(250);

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
                MessageBox.Show("Section unit or switch unit error has occurred! All trains have been stopped!", "IMPORTANT!!!",
                MessageBoxButtons.OK);
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

        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                subMenu.Visible = true;
            }
            else
                subMenu.Visible = false;
        }


        private void btnAddLoco_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCAddManual);

            //ControlLogic.MainLogic.controlLogic();

            labelTitle.Text = (sender as Button).Text;

            panelSettings.Visible = false;

        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCHome);

            labelTitle.Text = (sender as Button).Text;

            panelSettings.Visible = false;
        }

        private void btnSections_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCMap);

            labelTitle.Text = (sender as Button).Text;

            panelSettings.Visible = false;
        }

        private void btnJSON_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCdisplayJson);

            labelTitle.Text = (sender as Button).Text;

            uCdisplayJson.displayJson();

            panelSettings.Visible = false;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            showSubMenu(panelSettings);
        }

        private void btnUnitSettings_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCUnitSet);

            labelTitle.Text = (sender as Button).Text;

        }

        private void btnTurnoutSettings_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCTurnoutSet);

            labelTitle.Text = (sender as Button).Text;
        }

        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            StopAll();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //TODO - wait, until all locomotives are finished
            DialogResult result = MessageBox.Show("Opravdu chcete ukoncit klienta pro rizeni vlaku?", "DŮLEŽITÉ!!!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                FormMainMenu formmm = new FormMainMenu();
                formmm.StartPosition = FormStartPosition.Manual;
                formmm.Location = this.Location;
                formmm.Size = this.Size;
                
                this.Close();
                formmm.Show();

            }
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

            Thread.Sleep(250);
            lock (MainLogic.lockingLogic)
            {
                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();
                foreach (Trains train in trainsList)
                {
                    train.move = 0;
                }

                td.SaveJson(trainsList);

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

        /// <summary>
        /// Event handler pro vyzvednuti dat z logiky rizeni (Main logic)
        /// Vyvolana zmena vyhybek a vyhybky zmeneny podle dat z logiky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Pozadavek na vyhybky</param>
        protected void EventHandlerNewTurnoutData(object sender, TurnoutsDataSend e)
        {
            uint numberOfUnit = e.NumberOfUnit;

            byte turnout = e.Turnouts;

            byte value = e.Value;

            TurnoutInstructionPacket turnoutInstructionPacket = new TurnoutInstructionPacket(Packet.turnoutInstruction.nastaveni_vyhybky, numberOfUnit, turnout, value);
            
            SendTCPData(turnoutInstructionPacket.TCPPacket);
        }

        /// <summary>
        /// Event handler pro vyzvednuti dat z logiky rizeni (Main logic)
        /// Vyvolano logikou rizeni
        /// Dle pozadavku logiky dojde k zastaveni/rozjeti vlaku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EventHandlerNewLocoData(object sender, LocomotiveDataSend e)
        {
            Locomotive loco = e.Loco;

            bool reverse = e.Reverze;

            byte speed = e.Speed;


            if (speed > 3)
            {
                Thread.Sleep(250);

                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(loco, reverse, speed);

                SendTCPData(trainMotionPacket.TCPPacket);

                TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(loco, true);

                SendTCPData(trainFunctionPacket.TCPPacket);
            }
            else if (speed == 3)
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(loco, false, 3);

                SendTCPData(trainMotionPacket.TCPPacket);

                TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(loco, false);

                SendTCPData(trainFunctionPacket.TCPPacket);
            }

            else
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(loco, false, 0);

                SendTCPData(trainMotionPacket.TCPPacket);

                TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(loco, false);

                SendTCPData(trainFunctionPacket.TCPPacket);
            }
        }

        protected void EventHandlerNewMsgData(object sender, InfoMessageSend e)
        {
            string msg = e.InfoMessage.ToString();

            StopAll();

            MessageBox.Show(msg, "IMPORTANT!!!",
            MessageBoxButtons.OK);
        }

        /// <summary>
        /// Nastaveni softwarovych dorazu a rychlosti odesilani odberu proudu
        /// </summary>
        private void UnitSettingForLogic()
        {
            turnoutInstruction ti = turnoutInstruction.nastaveni_dorazu;

            List<XElement> turnoutStopDefinition = SearchLogic.GetTurnoutStopDefinitions();

            foreach (XElement turnout in turnoutStopDefinition)
            {
                uint unit = uint.Parse(turnout.Element("unit").Value);
                byte pos = byte.Parse(turnout.Element("pos").Value);
                byte leftStop = byte.Parse(turnout.Element("leftStop").Value);
                byte rightStop = byte.Parse(turnout.Element("rightStop").Value);

                TurnoutInstructionPacket turnoutInst = new TurnoutInstructionPacket(ti, unit, pos, leftStop, rightStop);
                SendTCPData(turnoutInst.TCPPacket);
            }


            unitInstruction ui = unitInstruction.prodleva_odesilani_zmerenych_proudu;
            IEnumerable<int> unitNumbers = SearchLogic.GetModulesId();
            foreach(int number in unitNumbers)
            {
                UnitInstructionPacket unitInst = new UnitInstructionPacket(ui, (byte)number, (byte)30);

                SendTCPData(unitInst.TCPPacket);
            }

            ti = turnoutInstruction.nastaveni_prodlevy_pred_natocenim;
            IEnumerable<int> turnoutNumbers = SearchLogic.GetTurnoutIDs();
            foreach(int number in turnoutNumbers)
            {
                TurnoutInstructionPacket turnoutInst = new TurnoutInstructionPacket(ti, (byte)number, (byte)10);
                SendTCPData(turnoutInst.TCPPacket);
            }
        }

        /// <summary>
        /// Nastaveni softwarovych dorazu a odesilani odberu proudu po spusteni TCP serveru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed_SW_Stops(object sender, ElapsedEventArgs e)
        {
            //Zastav timer
            timeToInitSoftwareStops.Stop();

            //Inicializace softwarovych dorazu
            UnitSettingForLogic();

            ((System.Timers.Timer)sender).Dispose();
        }
    }
}
