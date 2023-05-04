using ControlLogic;
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
    public partial class FormDebug : Form
    {
        //bool hodnota, zdali je pripojen klient k TCP serveru
        private bool IsConnect = false;

        //TCP klient
        private static TCPClient klient = null;

        private static System.Timers.Timer timeToInitSoftwareStops;

        //enum pro typy notifikaci
        private enum notificationType
        {
            warning,
            success,
            error,
        }

        //definice user controlu
        UCHome uCHome = new UCHome();
        UCTurnouts uCTurnouts = new UCTurnouts();
        UCMap uCMap = new UCMap();
        UCAddDebugTrain uCAddDebugTrain = new UCAddDebugTrain();
        UCTurnoutsMulti uCmulti = new UCTurnoutsMulti();
        UCJsonEdit uCEditJson = new UCJsonEdit();
        UCUnitSet uCUnitSet = new UCUnitSet();
        UCTurnoutsSettings uCTurnoutSet = new UCTurnoutsSettings();

        //list pro data z JSONu
        private static List<Trains> trainsList = new List<Trains>();

        public FormDebug()
        {
            //inicializace vsech komponent
            InitializeComponent();

            //zobrazeni domovske obrazovky user controlu
            DisplayInstance(uCHome);

            //inicializace logiky vyhledavani
            SearchLogic.InitSearch();
        }

        #region Form actions (Load, Init, Closed, Resized)

        /// <summary>
        /// Definice event handleru vyvolanych v user controlech, spusteni TCP serveru a casovacu ridicich jednotek
        /// </summary>
        /// <param name="sender">Event vyvolany v user controlu</param>
        /// <param name="e">Event vyvolany v user controlu</param>
        private void FormDebug_Load(object sender, EventArgs e)
        {

            //inicializace eventu v user controlech pouzivanych v aplikaci
            uCmulti.MultiTurnoutButtonAddClick += new EventHandler(UserControl_MultiTurnoutClick); //user control pro screen Multiturnout
            uCTurnouts.TurnoutButtonSendClick += new EventHandler(UserControl_TurnoutClick); //user control pro screen Turnout
            uCAddDebugTrain.ChangeOfTrainData += new EventHandler(UserControl_TrainDataChange); //user control pro zmenu rizeni vlaku
            uCEditJson.ButtonChangeJsonClick += new EventHandler(UserControl_EditJsonClick); //user control pro zmenu dat v JSONu
            uCUnitSet.UnitInstructionEventClick += new EventHandler(UserControl_UnitInstructionClick); //user control pro zmenu nastaveni ridici jednotky
            uCTurnoutSet.TurnoutDefinitionStopsClick += new EventHandler(UserControl_TurnoutInstructionStops);
            uCTurnoutSet.TurnoutInstructionSetClick += new EventHandler(UserControl_TurnoutInstructionSet);

            //spusteni tcp serveru
            StartTCPClient();

            //nastav time handler na nastaveni usekovych jednotek a jednotek vyhybek
            timeToInitSoftwareStops = new System.Timers.Timer(1000);
            timeToInitSoftwareStops.Elapsed += (sender, e) => Timer_Elapsed_SW_Stops(sender, e);
            timeToInitSoftwareStops.Start();
        }

        /// <summary>
        /// Akce nutne pri vypnuti okna programu
        /// </summary>
        /// <param name="sender">Event vyvolany zavrenim okna</param>
        /// <param name="e">Event vyvolany zavrenim okna</param>
        private void FormDebug_FormClosing(object sender, FormClosingEventArgs e)
        {
            //pokud jsem pripojen, zastav vsechny vlaky
            if (IsConnect)
            {
                StopAll();
            }

            Thread.Sleep(500);

            //vycisti klienta
            KlientCleanUp();
            if (klient != null)
            {
                klient.Dispose();
                klient = null;
            }
        }

        /// <summary>
        /// Upraveni velikosti mapy, pokud doslo k full screenu (nezobrazi se jinak spravne)
        /// </summary>
        /// <param name="sender">Event vyvolany otevrenim okna ve fullscreen modu</param>
        /// <param name="e">Event vyvolany otevrenim okna ve fullscreen modu</param>
        private void FormDebug_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                // form is in full screen mode
            }
        }

        /// <summary>
        /// zmeneni velikosti nekterych user controls, pokud se zmeni velikost okna
        /// </summary>
        /// <param name="sender">Event vyvolany zmenou velikosti okna</param>
        /// <param name="e">Event vyvolany zmenou velikosti okna</param>
        private void FormDebug_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                // form is in full screen mode
            }
        }

        #endregion

        #region TCP server (Start, Connect, Disconnect, Send Data)

        /// <summary>
        /// Spusteni a vytvoreni TCP serveru
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
            popUpNotification(notificationType.success, "TCP server has been started.");
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
        /// zpracovani dat prijatych
        /// </summary>
        /// <param name="e">Event na prichozi data z portu</param>
        private void DataProcessing(TCPReceivedEventArgs e)
        {
            //zasle prijata data ke zpracovani
            ProcessDataFromTCP.ProcessData(e);

            //testovani, zdali byl detekovan error ridicich jednotek
            bool testError = ProcessDataFromTCP.getErrors();

            //doslo k detekci erroru usekove jednotky nebo jednotky vyhybek
            //zastav vsechny vlaky, zobraz message box a pop up notifikaci
            if (testError)
            {
                StopAll();
                ProcessDataFromTCP.setErrors(false);
                DialogResult result = MessageBox.Show("Section unit or switch unit error has occurred! All trains have been stopped!", "IMPORTANT!!!",
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

        #region Definition of buttons in the left menu and user control

        /// <summary>
        /// Metoda pro zobrazeni user controlu, ktery byl vybran
        /// </summary>
        /// <param name="uc">Konkretni pouziti User Control</param>
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
        /// Akce na button Exit, kdy dojde k opusteni windows form
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            //zobrazeni dialogu na potvrzeni, ze uzivatel skutecne chce ukoncit aplikaci
            DialogResult result = MessageBox.Show("Do you really want to leave the app? Are you sure that correct JSON values were set using 'Update JSON' button?!?!", "WARNING!!!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            //pokud chce skutecne ukoncit apliakce, ukonci aplikaci
            if (result == DialogResult.Yes)
            {
                //zobrazeni hlavniho menu
                FormMainMenu formmm = new FormMainMenu();   //nacti windows form pro hlavni menu
                formmm.StartPosition = FormStartPosition.Manual;
                formmm.Location = this.Location;
                formmm.Size = this.Size;

                this.Close();
                formmm.Show();
            }
        }


        /// <summary>
        /// Akce na button pro rizeni lokomotiv
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnAddLoco_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCAddDebugTrain);

            labelTitle.Text = (sender as Button).Text;
        }


        /// <summary>
        /// Akce na button po stisknuti Home
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnHome_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCHome);

            labelTitle.Text = (sender as Button).Text;
        }


        /// <summary>
        /// Akce na button pro vyhybky
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnTurnouts_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCTurnouts);

            labelTitle.Text = (sender as Button).Text;
            uCTurnouts.clearData();
        }

        /// <summary>
        /// Akce na button pro multi vyhybky
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnMultiTurnouts_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCmulti);

            labelTitle.Text = (sender as Button).Text;
            uCmulti.ClearData();
        }

        /// <summary>
        /// Akce na button po stisknuti tlacitka pro aktualizaci JSONu
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnUpdateJson_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCEditJson);
            labelTitle.Text = (sender as Button).Text;
            uCEditJson.ClearData();
        }


        /// <summary>
        /// Akce na button, ktery slouzi pro nastaveni ridicich jednotek
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnUnitInstruction_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCUnitSet);
            labelTitle.Text = (sender as Button).Text;
        }

        /// <summary>
        /// Akce na button, ktery slouzi pro nastaveni jednotek pro vyhybky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTurnoutInstruction_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCTurnoutSet);
            labelTitle.Text = (sender as Button).Text;
        }

        /// <summary>
        /// Akce na button, ktery slouzi na vypnuti a zastaveni vsech lokomotiv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            //zastav vsechny vlaky!
            StopAll();

            popUpNotification(notificationType.warning, "All trains have been stopped because Central Stop button has been clicked.");

        }


        #endregion

        #region Zpracovani eventu a odeslani dat, eventy vyvolany v user controlech
        /// <summary>
        /// Nastaveni softwarovych dorazu
        /// </summary>
        private void softwareStops()
        {
            //pridani typu instrukce
            turnoutInstruction ti = turnoutInstruction.nastaveni_dorazu;

            //nacti vsechny ID ridicich jednotek kolejovych useku
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

            //softwarove dorazy byly nastaveny
            string msg = "Software stops for switches from configuration file were set!";
            popUpNotification(notificationType.success, msg);
        }

        /// <summary>
        /// Nastaveni softwarovych dorazu po spusteni TCP serveru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed_SW_Stops(object sender, ElapsedEventArgs e)
        {
            //Zastav timer
            timeToInitSoftwareStops.Stop();

            //Inicializace softwarovych dorazu
            softwareStops();

            ((System.Timers.Timer)sender).Dispose();
        }


        /// <summary>
        /// Metoda vyvolana Event Handlerem
        /// Rozjede nebo zastavi lokomotivu podle stisknuti tlacitka start/stop dane lokomotivy o dane rychlosti a smeru
        /// Vyvolano stisknutim tlacitka Start/Stop v user controlu
        /// </summary>
        /// <param name="sender">Event Handler z user controlu</param>
        /// <param name="e">Event Handler z user controlu</param>
        protected void UserControl_TrainDataChange(object sender, EventArgs e)
        {
            //Data lokomotivy, u ktere bylo stisknuto Start/Stop
            List<ChangeTrainData> trainDataChange = uCAddDebugTrain.trainDataChange;

            bool foundMatch = false;

            //projed vsechny lokomotivy z konfiguracniho souboru
            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {
                string loco = locomotive.Name.ToString();

                foreach (ChangeTrainData data in trainDataChange)
                {
                    //ChangeTrainData data = trainDataChange[i];
                    string loc = Packet.GapToUnderLine(data.Lokomotive);

                    //nalezena shoda mezi lokomotivou v konfiguracnim souboru a lokomotivou, jejiz data byla vlozena
                    if (loco == loc)
                    {
                        //Lokomotiva se ma rozjet a rychlost je vetsi nez 3, vytvori se paket a zasle se
                        if (data.StartStop && data.Speed > 3)
                        {
                            bool reverse = data.Reverze;

                            byte speed = data.Speed;

                            TrainMotionPacket trainMotionPacket = new TrainMotionPacket(locomotive, reverse, speed);

                            SendTCPData(trainMotionPacket.TCPPacket);

                            TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(locomotive, true);

                            SendTCPData(trainFunctionPacket.TCPPacket);

                            string msg = "Train " + locomotive.Name + " start moving with speed " + speed + " and in direction " + (reverse ? "reverse" : "ahaed" + ".");
                            popUpNotification(notificationType.success, msg);

                        }

                        //zastavit lokomotivu, pro niz byla poslana data
                        //vytvori se packet a posle se
                        else
                        {
                            TrainMotionPacket trainMotionPacket = new TrainMotionPacket(locomotive, false, 3);

                            SendTCPData(trainMotionPacket.TCPPacket);

                            TrainFunctionPacket trainFunctionPacket = new TrainFunctionPacket(locomotive, false);

                            SendTCPData(trainFunctionPacket.TCPPacket);

                            string stopmsg = "Train " + locomotive.Name + " has been stopped.";
                            popUpNotification(notificationType.success, stopmsg);
                        }

                        foundMatch = true;
                        break;
                    }
                }
                if (foundMatch)
                    break;
            }
            //vycisti seznam pozadavku na zmenu
            trainDataChange.Clear();

        }

        /// <summary>
        /// Metoda vyvolana Event Handlerem
        /// Prisel pozadavek na zmenu dat v JSON filu (pozice nejake lokomotivy)
        /// </summary>
        /// <param name="sender">Event Handler z user controlu</param>
        /// <param name="e">Event Handler z user controlu</param>
        protected void UserControl_EditJsonClick(object sender, EventArgs e)
        {
            List<ChangeJsonData> changeData = uCEditJson.changeJsonData;

            //data z JSONu
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();


            bool foundMatch = false;

            string name = null;

            //najdi data, ktera maji shodny nazev lokomotivy s lokomotivou z jsonu
            foreach (Trains train in trainsList)
            {
                foreach (ChangeJsonData data in changeData)
                {
                    //nalezena shoda mezi vlakem v JSONu a vybranym vlakem
                    if (train.name == data.Name)
                    {
                        name = train.name;

                        //aktualizace dat, aby bylo mozne automaticke rizeni
                        train.currentPosition = data.CurrentPosition;
                        train.lastPosition = data.PreviousPosition;
                        train.nextPosition = null;
                        train.finalPosition = null;
                        train.reverse = data.Reverse;
                        train.circuit = SearchLogic.GetCurrentCircuit(train.currentPosition);
                        train.startPosition = data.StartPosition;
                        train.mapOrientation = data.Orientation;
                        foundMatch = true;
                        break;

                    }
                }
                if (foundMatch)
                    break;
            }
            //uloz zpet do JSONu
            td.SaveJson(trainsList);

            string msg = "Json data for train " + name + " have been updated.";
            popUpNotification(notificationType.success, msg);

            //vycisti prijata data
            changeData.Clear();
        }

        /// <summary>
        /// Metoda vyvolana Event Handlerem na zmenu vyhybek
        /// Prisel pozadavek z user controlu. Data budou vlozeno do packetu a zaslana
        /// </summary>
        /// <param name="sender">Event Handler z user controlu</param>
        /// <param name="e">Event Handler z user controlu</param>
        protected void UserControl_MultiTurnoutClick(object sender, EventArgs e)
        {
            //zjisteni dat pro nastaveni vyhybek
            List<Turnouts> turnouts = uCmulti.turnouts;

            //vyber jednotliva data ulozena a zasli je
            for (int i = 0; i < turnouts.Count; i++)
            {
                uint numberOfUnit = turnouts[i].UnitID;

                byte data1 = turnouts[i].Change;

                byte data2 = turnouts[i].Position;

                TurnoutInstructionPacket turnoutInstructionPacket = new TurnoutInstructionPacket(Packet.turnoutInstruction.nastaveni_vyhybky, numberOfUnit, data1, data2);

                SendTCPData(turnoutInstructionPacket.TCPPacket);

            }
            //vymaz prijata data
            turnouts.Clear();
            string msg = "Switches have been set.";
            popUpNotification(notificationType.success, msg);
        }

        /// <summary>
        /// Metoda vyvolana Event Handlerem na zmenu vyhybek
        /// Prisel pozadavek z user controlu. Data budou vlozeno do packetu a zaslana
        /// </summary>
        /// <param name="sender">Event Handler z user controlu</param>
        /// <param name="e">Event Handler z user controlu</param>
        protected void UserControl_TurnoutClick(object sender, EventArgs e)
        {
            //zjisteni dat pro nastaveni vyhybek
            List<Turnouts> turnouts = uCTurnouts.turnouts;

            //vlozeni prijatych dat do promennych, vytvoreni packetu a zaslani
            for (int i = 0; i < turnouts.Count; i++)
            {
                uint numberOfUnit = turnouts[i].UnitID;

                byte data1 = turnouts[i].Change;

                byte data2 = turnouts[i].Position;

                TurnoutInstructionPacket turnoutInstructionPacket = new TurnoutInstructionPacket(Packet.turnoutInstruction.nastaveni_vyhybky, numberOfUnit, data1, data2);

                SendTCPData(turnoutInstructionPacket.TCPPacket);

            }
            //vycisti prijata data
            turnouts.Clear();

            //vytvor push notifikaci o nastaveni vyhybek
            string msg = "Switches have been set.";
            popUpNotification(notificationType.success, msg);
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
            
            //vytvor popup notifikaci o nastaveni vyhybek
            string msg = "Unit section data has been set.";
            popUpNotification(notificationType.success, msg);

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

            //vytvor popup notifikaci o nastaveni softwarovych dorazu
            string msg = "Turnout unit data for software stops has been set";
            popUpNotification(notificationType.success, msg);

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
            //vyzvednuti ulozenych dat
            List<SetNewTurnoutUnitData> newTurnoutData = uCTurnoutSet.newTurnoutData;

            //vlozeni prijatych dat do promennych, vytvoreni packetu a zaslani
            for (int i = 0; i < newTurnoutData.Count; i++)
            {
                turnoutInstruction ti = newTurnoutData[i].Type;

                byte numberOfUnit = newTurnoutData[i].NumberOfUnit;

                byte data = newTurnoutData[i].Data;

                TurnoutInstructionPacket turnoutInst = new TurnoutInstructionPacket(ti, numberOfUnit, data);

                SendTCPData(turnoutInst.TCPPacket);
            }

            //vytvor popup notifikaci o nastaveni ridici jednotky vyhybek
            string msg = "Turnout unit data has been sent";
            popUpNotification(notificationType.success, msg);

            newTurnoutData.Clear();
        }
        #endregion

        /// <summary>
        /// Metoda, ktera zastavi vsechny lokomotivy z konfiguracniho souboru
        /// </summary>
        private void StopAll()
        {
            //zastav vsechny lokomotivy
            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(locomotive, false, 0);

                SendTCPData(trainMotionPacket.TCPPacket);
            }

            //nastav hodnotu vsech vlaku na to, ze stoji a nemaji povel k jizde(nejedou ani necekaji na rozjezd)
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();
            foreach (Trains train in trainsList)
            {
                train.move = 0;
            }

            //uloz JSON
            td.SaveJson(trainsList);

            //vytvor popup notifikaci o zastaveni vsech vlaku
            string msg = "All trains have been stoped!";
            popUpNotification(notificationType.warning, msg);
        }

        /// <summary>
        /// MEtoda pro popup notifikace pro lepsi prehled uzivatele nad kolejistem
        /// </summary>
        /// <param name="type">Typ popup zpravy</param>
        /// <param name="msg">Zprava</param>
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
