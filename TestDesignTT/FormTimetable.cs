using ControlLogic;
using Microsoft.Win32;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Threading;
using System.Timers;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Xml.Linq;
using TrainTTLibrary;
using Tulpep.NotificationWindow;
using static System.Windows.Forms.DataFormats;
using static TrainTTLibrary.Packet;


namespace TestDesignTT
{
    public partial class FormTimetable : Form
    {
        private bool IsConnect = false;

        private static TCPClient klient = null;

        private enum notificationType
        {
            warning,
            success,
            error,
        }


        public BindingList<DataTimetable> timetable = new BindingList<DataTimetable>();

        private static System.Timers.Timer timeToInitSoftwareStops;

        //public BindingList<MovingInTimetamble> movingTimetable = new BindingList<MovingInTimetamble>();

        UCHome uCHome = new UCHome();
        UCTrainTimetable ucTrainTimetable = new UCTrainTimetable();
        UCDataLoad ucDataLoad = new UCDataLoad();
        UCJsonDisplay uCJsonDisplay = new UCJsonDisplay();
        UCMap uCMap = new UCMap();
        UCUnitSet uCUnitSet = new UCUnitSet();
        UCTurnoutsSettings ucTurnoutsSettings = new UCTurnoutsSettings();


        MainLogic ml = new MainLogic();

        private static List<Trains> trainsList = new List<Trains>();

        public FormTimetable()
        {
            InitializeComponent();
            DisplayInstance(uCHome);
            panelSettings.Visible = false;
            checkBtnLogic();

            ml.InfoMessageEvent += new EventHandler<InfoMessageSend>(EventHandlerNewMsgData);
            ml.LocomotiveDataEvent += new EventHandler<LocomotiveDataSend>(EventHandlerNewLocoData);
            ml.TurnoutsDataEvent += new EventHandler<TurnoutsDataSend>(EventHandlerNewTurnoutData);

            MainLogic.Initialization(ml);
        }

        private void FormMainMenu_Load(object sender, EventArgs e)
        {
            ucDataLoad.ButtonLoadClick += new EventHandler(UserControl_ButtonLoadClick);
            uCUnitSet.UnitInstructionEventClick += new EventHandler(UserControl_UnitInstructionClick); //user control pro zmenu nastaveni ridici jednotky
            ucTurnoutsSettings.TurnoutDefinitionStopsClick += new EventHandler(UserControl_TurnoutInstructionStops);
            ucTurnoutsSettings.TurnoutInstructionSetClick += new EventHandler(UserControl_TurnoutInstructionSet);

            StartTCPClient();

            timeToInitSoftwareStops = new System.Timers.Timer(1000);

            // Set the event handler for the Elapsed event
            timeToInitSoftwareStops.Elapsed += (sender, e) => Timer_Elapsed_SW_Stops(sender, e);

            // Start the timer
            timeToInitSoftwareStops.Start();
        }

        private void FormTimetable_FormClosing(object sender, FormClosingEventArgs e)
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

        private void FormMainMenu_SizeChanged(object sender, EventArgs e)
        {
            //TODO
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

                timer1.Enabled = false;
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


        #region Actions for buttons in the left menu
        private void btnHome_Click(object sender, EventArgs e)
        {
            //DisplayInstance(UCHome.Instance);

            //UCHome uCHome = new UCHome();
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(uCHome);
            labelTitle.Text = (sender as Button).Text;
        }

        private void btnSections_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(uCMap);
            labelTitle.Text = (sender as Button).Text;
        }

        private void btnJSON_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(uCJsonDisplay);
            uCJsonDisplay.displayJson();

            labelTitle.Text = (sender as Button).Text;
        }

        private void btnLoadTimetable_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(ucDataLoad);

            labelTitle.Text = (sender as Button).Text;

            ucDataLoad.CheckEnabled();
        }

        private void btnDisplayTimetable_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(ucTrainTimetable);
            labelTitle.Text = (sender as Button).Text;

            ucTrainTimetable.loadTimetamble(timetable);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = true;
            checkBtnLogic();
        }

        private void btnUnitSettings_Click(object sender, EventArgs e)
        {
            checkBtnLogic();

            DisplayInstance(uCUnitSet);
            labelTitle.Text = (sender as Button).Text;
        }

        private void btnTurnoutSettings_Click(object sender, EventArgs e)
        {
            checkBtnLogic();

            DisplayInstance(ucTurnoutsSettings);
            labelTitle.Text = (sender as Button).Text;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;

            if (btnPlay.IconChar == FontAwesome.Sharp.IconChar.Play)
            {
                if (panelDesktopPanel.Controls.Contains(ucDataLoad))
                {
                    panelDesktopPanel.Controls.Add(uCHome);
                    uCHome.Dock = DockStyle.Fill;
                    uCHome.BringToFront();
                }
                btnPlay.IconChar = FontAwesome.Sharp.IconChar.Pause;
                btnPlay.Text = "Pause";
            }
            else
            {
                btnPlay.IconChar = FontAwesome.Sharp.IconChar.Play;
                btnPlay.Text = "Play";

                for (int i = 0; i < timetable.Count; i++)
                {
                    DateTime now = new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                    DateTime inTimetable = new DateTime(1, 1, 1, timetable[i].Departure.Hour, timetable[i].Departure.Minute, timetable[i].Departure.Second);

                    if (now > inTimetable)
                    {
                        timetable.RemoveAt(i);
                        i--;
                    }
                    else
                        break;
                }
            }
            checkBtnLogic();
        }

        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            StopAll();

            btnPlay.Text = "Play";
            btnPlay.IconChar = FontAwesome.Sharp.IconChar.Play;

            checkBtnLogic();
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            FormMainMenu formmm = new FormMainMenu();
            formmm.StartPosition = FormStartPosition.Manual;
            formmm.Location = this.Location;
            formmm.Size = this.Size;
            this.Hide();
            formmm.Show();
            //FormMainMenu.;
        }
        #endregion

        /// <summary>
        /// Zobrazeni spravneho user controlu dle kliknuti uzivatele
        /// </summary>
        /// <param name="uc"></param>
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
        /// Metoda preo testovani logiky, kdy ma byt ktery button aktivni
        /// </summary>
        private void checkBtnLogic()
        {
            if (timetable.Count != 0)
            {
                btnPlay.Enabled = true;
                btnDisplayTimetable.Enabled = true;
            }
            else
            {
                btnPlay.Enabled = false;
                btnDisplayTimetable.Enabled = false;
            }

            if (btnPlay.Text == "Pause")
            {
                btnLoadTimetable.Enabled = false;
                timer1.Enabled = true;
            }
            else
            {
                btnLoadTimetable.Enabled = true;
                timer1.Enabled = false;
            }
        }

        private void TimeInTimetableUpdated(object sender, EventArgs e)
        {
            if (panelDesktopPanel.Controls.Contains(ucTrainTimetable))
                ucTrainTimetable.loadTimetamble(timetable);
        }

        public void loadMyTimetamble(string fileName, bool infinity)
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                try
                {
                    timetable.Clear();

                    DateTime departure = DateTime.Today.Add(new TimeSpan(4, 0, 0));

                    List<DateTime> departureStart = new List<DateTime>()
                    {
                        DateTime.Today.Add(new TimeSpan(7, 15, 0))
                    };

                    DateTime currentDeparture = DateTime.Today.Add(new TimeSpan(7, 15, 0));
                    while (currentDeparture < DateTime.Today.Add(new TimeSpan(18, 30, 0)))
                    {
                        currentDeparture = currentDeparture.Add(new TimeSpan(0, 55, 0));
                        departureStart.Add(currentDeparture);
                    }

                    List<DateTime> departureStop = new List<DateTime>()
                    {
                        DateTime.Today.Add(new TimeSpan(7, 35, 0))
                    };
                    DateTime currentEndDeparture = DateTime.Today.Add(new TimeSpan(7, 30, 0));
                    while (currentEndDeparture < DateTime.Today.Add(new TimeSpan(18, 45, 0)))
                    {
                        currentEndDeparture = currentEndDeparture.Add(new TimeSpan(0, 55, 0));
                        departureStop.Add(currentEndDeparture);
                    }


                    int index = 0;
                    if (infinity)
                    {
                        //departure = DateTime.Today.Add(new TimeSpan(0, 0, 0));
                        departure = DateTime.Now.AddMinutes(0.5);
                    }
                    else
                    {
                        departure = departureStart[index];
                    }

                    while (true)
                    {
                        if (infinity)
                        {
                            if (departure.TimeOfDay < new TimeSpan(23, 0, 0))
                            {
                                String[] lines = File.ReadAllLines(fileName);
                                foreach (String line in lines)
                                {

                                    DataTimetable note = new DataTimetable(line, departure);
                                    timetable.Add(note);
                                    departure = departure.AddSeconds(30);
                                }
                            }
                            else
                                break;
                        }
                        else
                        {
                            if (departure < departureStop[index])
                            {
                                String[] lines = File.ReadAllLines(fileName);
                                foreach (String line in lines)
                                {

                                    DataTimetable note = new DataTimetable(line, departure);
                                    timetable.Add(note);
                                    departure = departure.AddSeconds(45);
                                }
                            }
                            else
                            {
                                index++;

                                if (index == departureStart.Count())
                                    break;

                                departure = departureStart[index];
                            }
                        }
                    }


                    for (int i = 0; i < timetable.Count; i++)
                    {
                        DateTime now = new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                        DateTime inTimetable = new DateTime(1, 1, 1, timetable[i].Departure.Hour, timetable[i].Departure.Minute, timetable[i].Departure.Second);

                        if (now > inTimetable)
                        {
                            timetable.RemoveAt(i);
                            i--;
                        }
                        else
                            break;
                    }


                }
                catch (IOException)
                {
                    MessageBox.Show("An I/O error occurred while opening the file...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid file name!!\n");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            List<string> stationNames = SearchLogic.GetStationNames();
            List<string> endTracks = SearchLogic.GetAllStationTracks();
            bool startTrackSpecific = false;
            bool finalTrackSpecific = false;
            string startPosition = null;

            string textForMsgBox = null;

            for (int i = 0; i < timetable.Count(); i++)
            {
                DateTime now = new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                DateTime inTimetable = new DateTime(1, 1, 1, timetable[i].Departure.Hour, timetable[i].Departure.Minute, timetable[i].Departure.Second);

                //zkontrolovat,jestli uz neni cas odjezdu
                if (now > inTimetable)
                {
                    TrainDataJSON td = new TrainDataJSON();
                    trainsList = td.LoadJson();

                    bool foundTrain = false;
                    foreach (Trains train in trainsList)
                    {
                        //nalezen vlak a nepohybuje se jeste
                        if (train.name == timetable[i].Name && train.move == 0)
                        {
                            foundTrain = true;
                            //vsechny stanice na nadrazi testuje
                            //List<string> getTracksForStartPositon = SearchLogic.GetTracksForStation(timetable[i].StartStation);
                            string finStation = null;

                            if (endTracks.Contains(timetable[i].FinalStation))
                                finStation = SearchLogic.getStationNameFromTrack(timetable[i].FinalStation);
                            else
                                finStation = timetable[i].FinalStation;

                            List<string> getTracksForFinalPositon = SearchLogic.GetTracksForStation(finStation);

                            //v jizdnim radu je odjezd z nadrazi (vlak tam je) nebo v jizdnim radu je odjezd na specifickou kolej (vlak se na ni nachazi)
                            //vlak se jiz nachazi v cilove stanici - neni potreba zadna dalsi akce
                            if (train.currentPosition == timetable[i].FinalStation || getTracksForFinalPositon.Contains(train.currentPosition))
                            {
                                textForMsgBox = "Train" + train.name + "is already in the final station!";

                                popUpNotification(notificationType.warning, textForMsgBox);
                                break;
                            }

                            IEnumerable<string> fromStart = null;
                            IEnumerable<string> final = null;
                            bool crit = false;

                            //zjisteni jestli vlak pojede opacnym smerem nez jel posledne
                            if (train.circuit == 0 || train.circuit == 4 || train.circuit == 7)
                            {
                                crit = true;
                                fromStart = SearchLogic.GetStartStationInCritical(train.currentPosition, train.lastPosition);
                            }
                            else
                            {
                                crit = false;
                                fromStart = SearchLogic.GetStartStationOutside(train.currentPosition, train.lastPosition);
                            }

                            if (crit)
                            {
                                final = SearchLogic.GetFinalStationInCritical(train.currentPosition, train.lastPosition);
                            }
                            else
                            {
                                final = SearchLogic.GetFinalStationOutside(train.currentPosition, train.lastPosition);
                            }

                            if (timetable[i].Reverse != train.reverse)
                            {
                                IEnumerable<string> HelpVar = fromStart;
                                fromStart = final;
                                final = HelpVar;
                            }

                            List<string> uniqueFinal = final.Distinct().ToList();
                            List<string> uniqueStart = fromStart.Distinct().ToList();

                            if (stationNames.Contains(timetable[i].StartStation))
                            {
                                startTrackSpecific = false;
                                if (!uniqueStart.Contains(timetable[i].StartStation))
                                {
                                    textForMsgBox = "Train " + train.name + " doesn't have valid Start destination!";
                                    popUpNotification(notificationType.error, textForMsgBox);
                                    break;
                                }
                            }
                            else
                            {
                                startTrackSpecific = true;
                                string startStation = SearchLogic.getStationNameFromTrack(timetable[i].StartStation);
                                if (!uniqueStart.Contains(timetable[i].StartStation))
                                {
                                    textForMsgBox = "Train " + train.name + " doesn't have valid Start destination!";
                                    popUpNotification(notificationType.error, textForMsgBox);
                                    break;
                                }
                            }

                            if (stationNames.Contains(timetable[i].FinalStation))
                            {
                                finalTrackSpecific = false;
                                if (!uniqueFinal.Contains(timetable[i].FinalStation))
                                {
                                    textForMsgBox = "Train " + train.name + " doesn't have valid final destination!";
                                    popUpNotification(notificationType.error, textForMsgBox);
                                    break;
                                }
                            }
                            else
                            {
                                bool checkFinalTrack = false;

                                IEnumerable<XElement> finTrack = SearchLogic.GetFinalTrackOutside(train, finStation);

                                int finalCircuit = SearchLogic.GetFinalStationCircuit(finStation);

                                foreach (XElement x in finTrack)
                                {
                                    if ((train.circuit == 0 && finalCircuit == 0)
                                        || (train.circuit == 4 && finalCircuit == 4)
                                        || (train.circuit == 7 && finalCircuit == 7))
                                    {
                                        bool bb;

                                        if (timetable[i].Reverse == train.reverse)
                                            bb = SearchLogic.GetFinalTrackInside(train.currentPosition, train.lastPosition, x.Value);
                                        else
                                            bb = SearchLogic.GetFinalTrackInside(train.lastPosition, train.currentPosition, x.Value);

                                        if (bb && (timetable[i].FinalStation == x.Value))
                                        {
                                            checkFinalTrack = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (timetable[i].FinalStation == x.Value)
                                        {
                                            checkFinalTrack = true;
                                            break;
                                        }
                                    }
                                }
                                if (!checkFinalTrack)
                                {
                                    textForMsgBox = "Train " + train.name + " doesn't have valid Final destination!";
                                    popUpNotification(notificationType.error, textForMsgBox);
                                    break;
                                }
                            }
                            //MainLogic.addNewTrainDataFromClient(train.name, train.currentPosition, (byte)timetable[i].Speed, timetable[i].Reverse, timetable[i].FinalStation);

                            textForMsgBox = "Train " + train.name + " has been sent!";
                            popUpNotification(notificationType.success, textForMsgBox);
                            break;


                        }

                        //prisel prikaz na vlak, ale vlak jiz ma prikaz k pohybu
                        else if (train.name == timetable[i].Name && train.move != 0)
                        {
                            foundTrain = true;
                            textForMsgBox = "Train " + train.name + " is already moving!";
                            popUpNotification(notificationType.warning, textForMsgBox);
                            break;
                        }
                        else { }
                    }
                    if (!foundTrain)
                    {
                        textForMsgBox = "Train " + timetable[i].Name + " was not found!";
                        popUpNotification(notificationType.error, textForMsgBox);
                    }

                    timetable.RemoveAt(i);
                    i--;
                    TimeInTimetableUpdated(sender, e);
                    /*
                    if (textForMsgBox != null)
                        MessageBox.Show(textForMsgBox, "Info from timetable control!", MessageBoxButtons.OK);
                    */

                }
                else
                    break;
            }
        }


        #region Actions with event handlers (load timetable, unit instruction, turnout instruction, loco move, info msg)
        protected void UserControl_ButtonLoadClick(object sender, EventArgs e)
        {
            List<DataToLoad> dataLoad = ucDataLoad.dataToLoads;

            for (int i = 0; i < dataLoad.Count; i++)
            {
                loadMyTimetamble(dataLoad[i].Filename, dataLoad[i].InfinityData);
            }

            dataLoad.Clear();
            checkBtnLogic();
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
            List<SetNewTurnoutStops> newTurnoutStops = ucTurnoutsSettings.newTurnoutStops;

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
            List<SetNewTurnoutUnitData> newTurnoutData = ucTurnoutsSettings.newTurnoutData;

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

            timer1.Enabled = false;

            MessageBox.Show(msg, "IMPORTANT!!!",
            MessageBoxButtons.OK);
        }
        #endregion

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
        /// Timer, ktery po vterine vyvola metodu pro spravne nastaveni jednotek pro rizeni kolejiste
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
            foreach (int number in unitNumbers)
            {
                UnitInstructionPacket unitInst = new UnitInstructionPacket(ui, (byte)number, (byte)30);

                SendTCPData(unitInst.TCPPacket);
            }

            ti = turnoutInstruction.nastaveni_prodlevy_pred_natocenim;
            IEnumerable<int> turnoutNumbers = SearchLogic.GetTurnoutIDs();
            foreach (int number in turnoutNumbers)
            {
                TurnoutInstructionPacket turnoutInst = new TurnoutInstructionPacket(ti, (byte)number, (byte)10);
                SendTCPData(turnoutInst.TCPPacket);
            }
        }

        private void popUpNotification(notificationType type, string msg)
        {
            PopupNotifier popup = new PopupNotifier();
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
                    popup.ImageSize = new(80,80);

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