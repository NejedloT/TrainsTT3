using ControlLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
using Tulpep.NotificationWindow;
using static TrainTTLibrary.Packet;

namespace TestDesignTT
{
    public partial class FormManual : Form
    {
        //bool hodnota, zdali je pripojen klient k TCP serveru
        private bool IsConnect = false;

        //TCP klient
        private static TCPClient klient = null;

        //enum pro typy notifikaci
        private enum notificationType
        {
            warning,
            success,
            error,
        }

        //definice timeru pro nastaveni usekovych jednotek a jednotek vyhybek
        private static System.Timers.Timer timeToInitSoftwareStops;

        //definice user controlu
        UCHome uCHome = new UCHome();
        UCMap uCMap = new UCMap();
        UCAddManualTrain uCAddManual = new UCAddManualTrain();
        UCJsonDisplay uCdisplayJson = new UCJsonDisplay();
        UCUnitSet uCUnitSet = new UCUnitSet();
        UCTurnoutsSettings uCTurnoutSet = new UCTurnoutsSettings();

        //vytvoreni instance logiky rizeni
        MainLogic ml = new MainLogic();

        //list pro data z JSONu
        private static List<Trains> trainsList = new List<Trains>();

        public FormManual()
        {
            //inicializace vsech komponent
            InitializeComponent();

            //zobrazeni domovske obrazovky user controlu
            DisplayInstance(uCHome);

            //inicializace logiky vyhledavani
            SearchLogic.InitSearch();

            //skryti postranniho panelu
            panelSettings.Visible = false;

            //inicializace eventu, ktere bezi v logice rizeni
            ml.InfoMessageEvent += new EventHandler<InfoMessageSend>(EventHandlerNewMsgData);
            ml.LocomotiveDataEvent += new EventHandler<LocomotiveDataSend>(EventHandlerNewLocoData);
            ml.TurnoutsDataEvent += new EventHandler<TurnoutsDataSend>(EventHandlerNewTurnoutData);
            ml.NotificationMessageEvent += new EventHandler<NotificationSend>(EventHandlerNewNotification);

            //inicializace logiky rizeni
            MainLogic.Initialization(ml);
        }

        /// <summary>
        /// Akce, ktere je nutno vyvolat okamzite po spusteni windows form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormManual_Load(object sender, EventArgs e)
        {
            //inicializace eventu v user controlech pouzivanych v aplikaci
            uCAddManual.ButtonAddLocoClick += new EventHandler(UserControl_ButtonAddLocoClick);
            uCUnitSet.UnitInstructionEventClick += new EventHandler(UserControl_UnitInstructionClick); //user control pro zmenu nastaveni ridici jednotky
            uCTurnoutSet.TurnoutDefinitionStopsClick += new EventHandler(UserControl_TurnoutInstructionStops);
            uCTurnoutSet.TurnoutInstructionSetClick += new EventHandler(UserControl_TurnoutInstructionSet);

            //spusteni TCP serveru
            StartTCPClient();

            //nastav time handler na nastaveni usekovych jednotek a jednotek vyhybek
            timeToInitSoftwareStops = new System.Timers.Timer(1000);
            timeToInitSoftwareStops.Elapsed += (sender, e) => Timer_Elapsed_SW_Stops(sender, e);
            timeToInitSoftwareStops.Start();

        }

        /// <summary>
        /// MEtoda vyvolana zmenou velikosti okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormManual_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                // form is in full screen mode
            }
        }

        /// <summary>
        /// MEtoda vyvolana ukoncenim aplikace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormManual_FormClosing(object sender, FormClosingEventArgs e)
        {
            //vypni timery - logika nebude bezet dale
            if (IsConnect)
            {
                StopAll();
            }

            //vypni timery v user controlech
            UCJsonDisplay.refreshData.Stop();
            UCMap.refreshData.Stop();
            UCJsonDisplay.refreshData.Enabled = false;
            UCMap.refreshData.Enabled = false;
            UCJsonDisplay.refreshData.Elapsed -= uCdisplayJson.UpdateData_Tick;
            UCMap.refreshData.Elapsed -= uCMap.UpdateData_Tick;


            Thread.Sleep(350);

            //vypni timery v logice rizeni
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

        /// <summary>
        /// MEtoda vyvolana zmenou velikosti okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormManual_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                // form is in full screen mode
            }
        }

        #region TCP server (Start, Connect, Disconnect, Send Data)
        /// <summary>
        /// Metoda, ktera spusti TCP server
        /// </summary>
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

            //zaslani notifikace, ze byl spusten TCP server
            popUpNotification(notificationType.success, "The TCP server has been started.");
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

                popUpNotification(notificationType.warning, "TCP server has been stopped and all trains are stopped.");
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
        /// Zpracovani dat prijatych ze serioveho portu
        /// Ze serioveho portu neustale neco chodi. Zkouma chybove hlasky. V pripade chyby dojde k zastaveni vsech vlaku a zobrazeni notifikace o erroru na kolejisti
        /// </summary>
        /// <param name="e">Event na prichozi data z portu</param>
        private void DataProcessing(TCPReceivedEventArgs e)
        {
            //zasle prijata data ke zpracovani
            ProcessDataFromTCP.ProcessData(e);

            //testovani, zdali byl detekovan error
            bool testError = ProcessDataFromTCP.getErrors();

            //doslo k detekci erroru usekove jednotky nebo jednotky vyhybek
            //zastav vsechny vlaky, zobraz message box a pop up notifikaci
            if (testError)
            {
                StopAll();
                ProcessDataFromTCP.setErrors(false);
                MessageBox.Show("Section unit or switch unit error has occurred! All trains have been stopped!", "IMPORTANT!!!",
                MessageBoxButtons.OK);

                popUpNotification(notificationType.error, "Section unit or switch unit error has occurred! All trains have been stopped!");
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

        /// <summary>
        /// Metoda, ktera zobrazi dany user control
        /// </summary>
        /// <param name="uc">Instance vybraneho user controllu</param>
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

        /// <summary>
        /// Logika zobrazeni submenu pro nastaveni jednotek pro vyhybky ci useky
        /// </summary>
        /// <param name="subMenu"></param>
        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                subMenu.Visible = true;
            }
            else
                subMenu.Visible = false;
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu na vybrani lokomotivy pro povel k jizde
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddLoco_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCAddManual);

            //ControlLogic.MainLogic.controlLogic();

            labelTitle.Text = (sender as Button).Text;

            panelSettings.Visible = false;

        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu hlavni obrazovky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHome_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCHome);

            labelTitle.Text = (sender as Button).Text;

            panelSettings.Visible = false;
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu na zobrazeni mapy useku a polohy lokomotiv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSections_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCMap);

            labelTitle.Text = (sender as Button).Text;

            panelSettings.Visible = false;
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu k zobrazeni aktualnich JSON hodnot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnJSON_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCdisplayJson);

            labelTitle.Text = (sender as Button).Text;

            //uCdisplayJson.displayJson();

            panelSettings.Visible = false;
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu k zobrazeni nastaveni - dojde k rozbaleni submenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettings_Click(object sender, EventArgs e)
        {
            showSubMenu(panelSettings);
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu v submenu pro zobrazeni user controlu na nastaveni usekove jednotky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnitSettings_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCUnitSet);

            labelTitle.Text = (sender as Button).Text;

        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu v submenu pro zobrazeni user controlu na nastaveni jednotky vyhybek
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTurnoutSettings_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCTurnoutSet);

            labelTitle.Text = (sender as Button).Text;
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro nouzove zastaveni vsech vlaku. Zaroven dojde k zobrazeni notifikace, ze doslo k zastaveni vsech vlaku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            StopAll();
            popUpNotification(notificationType.warning, "All trains were stopped because the 'Central stop' button was clicked.");
        }

        /// <summary>
        /// Akce po stisknuti tlacitka na ukonceni aplikace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            //zobrazeni dialogu na potvrzeni, ze uzivatel skutecne chce ukoncit aplikaci
            DialogResult result = MessageBox.Show("Do you really want to close the client?", "IMPORTANT!!!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            //pokud chce skutecne ukoncit apliakce, ukonci aplikaci
            if (result == DialogResult.Yes)
            {
                FormMainMenu formmm = new FormMainMenu();
                formmm.StartPosition = FormStartPosition.Manual;
                formmm.Location = this.Location;
                formmm.Size = this.Size;
                
                this.Close();
                formmm.Show();

            }
        }

        #endregion

        /// <summary>
        /// Zpracovani vyvolaneho eventu pro pridani novych dat pro pohyb lokomotivy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UserControl_ButtonAddLocoClick(object sender, EventArgs e)
        {
            //vyzvednuti dat
            List<AddDataToSend> addData = uCAddManual.addNewLocoData;

            //zasli data pro aktualizaci dat v JSONu
            foreach (var item in addData)
            {
                //vyvola aktualizace dat JSONu a priznak, ze vlak chce jet
                MainLogic.addNewTrainDataFromClient(item.Name, item.CurrentPosition,item.Speed, item.Reverse, item.StartPosition, item.FinalPosition);
            }
            addData.Clear();
        }

        /// <summary>
        /// Vynutu zastaveni vsech vlaku
        /// </summary>
        private void StopAll()
        {
            //zastav vsechny lokomotivy
            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(locomotive, false, 0);

                SendTCPData(trainMotionPacket.TCPPacket);
            }

            //pockej na zastaveni vsech vlaku
            Thread.Sleep(250);

            //nastav hodnotu vsech vlaku na to, ze stoji a nemaji povel k jizde (nejedou ani necekaji na rozjezd)
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
            //vyzvednuti ulozenych dat
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
            //vyzvednuti ulozenych dat
            List<SetNewTurnoutStops> newTurnoutStops = uCTurnoutSet.newTurnoutStops;

            //vlozeni prijatych dat do promennych, vytvoreni packetu a zaslani
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
            //zjisteni dat pro nastaveni jednotky vyhybek
            List<SetNewTurnoutUnitData> newTurnoutData = uCTurnoutSet.newTurnoutData;

            //vyber jednotliva data ulozena a zasli je
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

            //rychlost vetsi nez 3, zmen rychlost
            if (speed > 3)
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(loco, reverse, speed);

                SendTCPData(trainMotionPacket.TCPPacket);

                TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(loco, true);

                SendTCPData(trainFunctionPacket.TCPPacket);
            }
            //rychlost 3, pomalu zastav vlak
            else if (speed == 3)
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(loco, false, 3);

                SendTCPData(trainMotionPacket.TCPPacket);

                TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(loco, false);

                SendTCPData(trainFunctionPacket.TCPPacket);
            }

            //rychlost 0, zavazny problem, okamzite zastav vlak
            else
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(loco, false, 0);

                SendTCPData(trainMotionPacket.TCPPacket);

                TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(loco, false);

                SendTCPData(trainFunctionPacket.TCPPacket);
            }
        }

        /// <summary>
        /// Event handler pro zorbazeni MessageBoxu
        /// MEssageBoxy zobrazeny pouze pri nejzavaznejsich problemech - proto nutno pote zastavit vlak
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EventHandlerNewMsgData(object sender, InfoMessageSend e)
        {
            string msg = e.InfoMessage.ToString();

            StopAll();

            MessageBox.Show(msg, "IMPORTANT!!!",
            MessageBoxButtons.OK);
        }

        /// <summary>
        /// Event handler vyvolany logikou rizeni
        /// Doslo k nejake zmene rizeni
        /// a) Vlak vyjel/dojel z/do stanice
        /// b) Vlak zastavil z duvodu nebezpeci pred kolizi
        /// c) Vlak zastavil z duvodu
        /// d) Vlak se opetovne rozjel
        /// e) Poloha vlaku neodpovida ocekavane poloze
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void EventHandlerNewNotification(object sender, NotificationSend e)
        {
            //zjisteni typu notifikace
            notificationType type;

            if (e.NotificationType == "warning")
                type = notificationType.warning;
            else if (e.NotificationType == "success")
                type = notificationType.success;
            else if (e.NotificationType == "error")
                type = notificationType.error;
            else
                return;

            //zjisteni textu zpravy z notifikace
            string msg = e.InfoMessage;

            //volani metody pro vytvoreni notifikace
            popUpNotification(type, msg);

        }

        /// <summary>
        /// Nastaveni softwarovych dorazu a rychlosti odesilani odberu proudu
        /// </summary>
        private void UnitSettingForLogic()
        {
            //nastaveni softwarovych dorazu vyhybek, aby se nastavili o spravnou vzdalenost
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

            //nastaveni prodlevy odesilani odberu proudu na 300 ms, aby bylo zajisteno spravne rizeni kolejiste na zaklade odberu proudu
            unitInstruction ui = unitInstruction.prodleva_odesilani_zmerenych_proudu;
            IEnumerable<int> unitNumbers = SearchLogic.GetModulesId();
            foreach(int number in unitNumbers)
            {
                UnitInstructionPacket unitInst = new UnitInstructionPacket(ui, (byte)number, (byte)30);

                SendTCPData(unitInst.TCPPacket);
            }

            //nastaveni prodlevy pred natocenim servopohonu na minimalni hodnotu, aby se vyhybka prepnula temer okamzite
            ti = turnoutInstruction.nastaveni_prodlevy_pred_natocenim;
            IEnumerable<int> turnoutNumbers = SearchLogic.GetTurnoutIDs();
            foreach(int number in turnoutNumbers)
            {
                TurnoutInstructionPacket turnoutInst = new TurnoutInstructionPacket(ti, (byte)number, (byte)10);
                SendTCPData(turnoutInst.TCPPacket);
            }

            string msg = "Stops for the switches have been set from the configuration file.";
            popUpNotification(notificationType.success, msg);
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

            //Inicializace softwarovych dorazu a odesilani odberu proudu
            UnitSettingForLogic();

            ((System.Timers.Timer)sender).Dispose();
        }

        /// <summary>
        /// MEtoda pro popup notifikace pro lepsi prehled uzivatele nad kolejistem
        /// </summary>
        /// <param name="type">Typ instrukce - Success/Warning/Error</param>
        /// <param name="msg">Zprava, ktera se ma zobrazit</param>
        private void popUpNotification(notificationType type, string msg)
        {
            //vytvoreni nove notifikace
            PopupNotifier popup = new PopupNotifier();
            
            //konkretni data k zobrazeni na zaklade typu notifikace
            switch (type)
            {
                case notificationType.warning:
                    popup.Image = Properties.Resources.warning;
                    popup.ImageSize = new(80, 80);

                    popup.BodyColor = Color.FromArgb(255, 193, 7);
                    popup.TitleText = "Warning!";
                    popup.TitleColor = Color.White;
                    popup.TitleFont = new Font("Century Gothic", 18, FontStyle.Bold);

                    popup.ContentText = msg;
                    popup.ContentColor = Color.White;
                    popup.ContentFont = new Font("Century Gothic", 12);
                    popup.Popup();
                    break;

                case notificationType.success:
                    popup.Image = Properties.Resources.success;
                    popup.ImageSize = new(80, 80);

                    popup.BodyColor = Color.FromArgb(40, 120, 69);
                    popup.TitleText = "Success";
                    popup.TitleColor = Color.White;
                    popup.TitleFont = new Font("Century Gothic", 18, FontStyle.Bold);

                    popup.ContentText = msg;
                    popup.ContentColor = Color.White;
                    popup.ContentFont = new Font("Century Gothic", 12);
                    popup.Popup();

                    break;
                case notificationType.error:
                    popup.Image = Properties.Resources.error;
                    popup.ImageSize = new(80, 80);

                    popup.BodyColor = Color.FromArgb(220, 23, 29);
                    popup.TitleText = "Error!";
                    popup.TitleColor = Color.White;
                    popup.TitleFont = new Font("Century Gothic", 18, FontStyle.Bold);

                    popup.ContentText = msg;
                    popup.ContentColor = Color.White;
                    popup.ContentFont = new Font("Century Gothic", 12);
                    popup.Popup();
                    break;

            }
        }
    }
}
