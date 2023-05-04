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

        //preddefinovani listu s jizdnim radem
        public BindingList<DataTimetable> timetable = new BindingList<DataTimetable>();

        //definice timeru pro nastaveni usekovych jednotek a jednotek vyhybek
        private static System.Timers.Timer timeToInitSoftwareStops;

        //definice user controlu
        UCHome uCHome = new UCHome();
        UCTrainTimetable ucTrainTimetable = new UCTrainTimetable();
        UCDataLoad ucDataLoad = new UCDataLoad();
        UCJsonDisplay uCJsonDisplay = new UCJsonDisplay();
        UCMap uCMap = new UCMap();
        UCUnitSet uCUnitSet = new UCUnitSet();
        UCTurnoutsSettings ucTurnoutsSettings = new UCTurnoutsSettings();

        //vytvoreni instance logiky rizeni
        MainLogic ml = new MainLogic();

        //list pro data z JSONu
        private static List<Trains> trainsList = new List<Trains>();

        public FormTimetable()
        {
            //inicializace vsech komponent
            InitializeComponent();

            //zobrazeni domovske obrazovky user controlu
            DisplayInstance(uCHome);

            //inicializace logiky vyhledavani
            SearchLogic.InitSearch();

            //skryti postranniho panelu
            panelSettings.Visible = false;

            //kontrola logiky, ktere buttony mohou byt aktivni
            checkBtnLogic();

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
        private void FormMainMenu_Load(object sender, EventArgs e)
        {
            //inicializace eventu v user controlech pouzivanych v aplikaci
            ucDataLoad.ButtonLoadClick += new EventHandler(UserControl_ButtonLoadClick);
            uCUnitSet.UnitInstructionEventClick += new EventHandler(UserControl_UnitInstructionClick); //user control pro zmenu nastaveni ridici jednotky
            ucTurnoutsSettings.TurnoutDefinitionStopsClick += new EventHandler(UserControl_TurnoutInstructionStops);
            ucTurnoutsSettings.TurnoutInstructionSetClick += new EventHandler(UserControl_TurnoutInstructionSet);

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
        private void FormTimetable_FormClosing(object sender, FormClosingEventArgs e)
        {
            //vypni timery - logika nebude bezet dale
            if (IsConnect)
            {
                StopAll();
            }

            Thread.Sleep(350);

            //vypni timery v logice rizeni
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

        /// <summary>
        /// MEtoda vyvolana zmenou velikosti okna
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMainMenu_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                // form is in full screen mode
            }
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

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu hlavni obrazovky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHome_Click(object sender, EventArgs e)
        {
            //DisplayInstance(UCHome.Instance);

            //UCHome uCHome = new UCHome();
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(uCHome);
            labelTitle.Text = (sender as Button).Text;
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu na zobrazeni mapy useku a polohy lokomotiv
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSections_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(uCMap);
            labelTitle.Text = (sender as Button).Text;
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu k zobrazeni aktualnich JSON hodnot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnJSON_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(uCJsonDisplay);

            labelTitle.Text = (sender as Button).Text;

            uCJsonDisplay.displayJson();
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu k nacteni jizdniho radu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadTimetable_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(ucDataLoad);

            labelTitle.Text = (sender as Button).Text;

            ucDataLoad.CheckEnabled();
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu k zobrazeni jizdniho radu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDisplayTimetable_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            checkBtnLogic();

            DisplayInstance(ucTrainTimetable);
            labelTitle.Text = (sender as Button).Text;

            ucTrainTimetable.loadTimetamble(timetable);
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro zobrazeni user controlu k zobrazeni nastaveni - dojde k rozbaleni submenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSettings_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = true;
            checkBtnLogic();
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu v submenu pro zobrazeni user controlu na nastaveni usekove jednotky
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUnitSettings_Click(object sender, EventArgs e)
        {
            checkBtnLogic();

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
            checkBtnLogic();

            DisplayInstance(ucTurnoutsSettings);
            labelTitle.Text = (sender as Button).Text;
        }

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu v submenu pro zobrazeni user controlu pro spusteni jizdniho radu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPlay_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;

            //pokud je button Play, nastav Pause (vlaky pojedou)
            if (btnPlay.IconChar == FontAwesome.Sharp.IconChar.Play)
            {
                if (panelDesktopPanel.Controls.Contains(ucDataLoad))
                {
                    panelDesktopPanel.Controls.Add(uCHome);
                    uCHome.Dock = DockStyle.Fill;
                    uCHome.BringToFront();
                }
                //zmen ikonu a text
                btnPlay.IconChar = FontAwesome.Sharp.IconChar.Pause;
                btnPlay.Text = "Pause";
            }
            else
            {
                //zmen ikonu a text na Play
                btnPlay.IconChar = FontAwesome.Sharp.IconChar.Play;
                btnPlay.Text = "Play";

                //odstran zaznamy v jizdnim radu pred aktualnim casem
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

        /// <summary>
        /// Stisknuti tlacitka v postrannim menu pro nouzove zastaveni vsech vlaku. Zaroven dojde k zobrazeni notifikace, ze doslo k zastaveni vsech vlaku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            StopAll();

            btnPlay.Text = "Play";
            btnPlay.IconChar = FontAwesome.Sharp.IconChar.Play;

            popUpNotification(notificationType.warning, "All trains have been stopped because Central Stop button has been clicked.");

            checkBtnLogic();
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
        /// Metoda pro testovani logiky, kdy ma byt ktery button aktivni
        /// </summary>
        private void checkBtnLogic()
        {
            //pokud neni nacten timetable, nelze stisknout tlacitko Play a zobrazeni jizdniho radu
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

            //Pokud je zobrazeno tlaticko Pause (vlaky jezdi),
            if (btnPlay.Text == "Pause")
            {
                //povol timer pro jizdni rad a zakaz nacteni jizdniho radu
                btnLoadTimetable.Enabled = false;
                timer1.Enabled = true;
            }
            else
            {
                //vypni timer pro jizdni rad a povol nacteni jizdniho radu
                btnLoadTimetable.Enabled = true;
                timer1.Enabled = false;
            }
        }

        /// <summary>
        /// Metoda vyvolana zmenou jizdniho radu (doslo k casu odjezdu vlaku)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeInTimetableUpdated(object sender, EventArgs e)
        {
            if (panelDesktopPanel.Controls.Contains(ucTrainTimetable))
                ucTrainTimetable.loadTimetamble(timetable);
        }

        /// <summary>
        /// Metoda pro nacteni jizdniho radu
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="infinity"></param>
        public void loadMyTimetamble(string fileName, bool infinity)
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                try
                {
                    timetable.Clear();

                    //cas od kdy se udela jizdni rad
                    DateTime departure = DateTime.Today.Add(new TimeSpan(1, 0, 0));

                    //cas od kdy se udela jizdni rad - pro mode na prestavky
                    List<DateTime> departureStart = new List<DateTime>()
                    {
                        DateTime.Today.Add(new TimeSpan(7, 15, 0))
                    };

                    //prvni odjezd podle jizdniho radu pro mode na prestavky
                    DateTime currentDeparture = DateTime.Today.Add(new TimeSpan(7, 15, 0));
                    while (currentDeparture < DateTime.Today.Add(new TimeSpan(18, 30, 0)))
                    {
                        //vypocteno na cas prestavek
                        currentDeparture = currentDeparture.Add(new TimeSpan(0, 55, 0));
                        departureStart.Add(currentDeparture);
                    }

                    //prvni konec odjezdu vlaku (aby to pokrylo prestavku)
                    List<DateTime> departureStop = new List<DateTime>()
                    {
                        DateTime.Today.Add(new TimeSpan(7, 35, 0))
                    };
                    DateTime currentEndDeparture = DateTime.Today.Add(new TimeSpan(7, 30, 0));
                    while (currentEndDeparture < DateTime.Today.Add(new TimeSpan(18, 45, 0)))
                    {
                        //vypocteno na cas prestavek
                        currentEndDeparture = currentEndDeparture.Add(new TimeSpan(0, 55, 0));
                        departureStop.Add(currentEndDeparture);
                    }

                    //prvni cas odjezdu vlaku v zavislosti na zvolenem modu jizdniho radu
                    int index = 0;
                    if (infinity)
                    {
                        //odjezdy nepretrzite
                        departure = DateTime.Today.Add(new TimeSpan(0, 0, 30));
                        //departure = DateTime.Now.AddMinutes(0.5);
                    }
                    else
                    {
                        //odjezdy o prestavkach
                        departure = departureStart[index];
                    }

                    while (true)
                    {
                        //vytvoreni jizdniho radu pro nekonecny jizdni rad
                        if (infinity)
                        {
                            //vytvoreni jizdniho radu od 00:00:30 do 23:55:00
                            if (departure.TimeOfDay < new TimeSpan(23, 50, 0))
                            {
                                //vytvori sadu 10 radek jizdniho radu dle CSV souboru s intervalem 30 vterin
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
                        //vytvoreni jizdniho radu na prestavku
                        else
                        {
                            //dokud je cas mensi nez cas konce prestavek
                            if (departure < departureStop[index])
                            {
                                //vytvori sadu 10 radek jizdniho radu dle CSV souboru s intervalem 30 vterin
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
                                //bude dalsi index
                                index++;

                                //pokud jsem dosel na konec listu, tak konec
                                if (index == departureStart.Count())
                                    break;

                                //nastaveni noveho casu odjezdu
                                departure = departureStart[index];
                            }
                        }
                    }

                    //odstraneni vsech radek, ktere jsou pred soucasnym odjezdem vlaku
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
                    popUpNotification(notificationType.error, "Timetable couldn't be loaded due to some error!");
                }
            }
            else
            {
                MessageBox.Show("Invalid file name!!\n");
            }
        }

        /// <summary>
        /// MEtoda pro logiku rizeni kolejiste pomoci jizdniho radu
        /// V teto metoda dojde i k otestovani, jestli danou trasu pri dane orientaci vlaku a zvolenych pocatecnich a koncovych stanicich muze vlak vykonat, v opacnem pripade mu nebude umozneno vyjet!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            //list vsech nazvu konecnych stanic
            List<string> stationNames = SearchLogic.GetStationNames();

            //list vsech nazvu konecnych koleji ve stanicich
            List<string> endTracks = SearchLogic.GetAllStationTracks();

            //bool hodnoty, jestli ma vlak definovanou finalni / pocatecni kolej
            bool startTrackSpecific = false;
            bool finalTrackSpecific = false;

            //pocatecni pozice
            string startPosition = null;

            string textForMsgBox = null;

            //cyklus pres jednotlive radky jizdniho radu
            for (int i = 0; i < timetable.Count(); i++)
            {
                DateTime now = new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                DateTime inTimetable = new DateTime(1, 1, 1, timetable[i].Departure.Hour, timetable[i].Departure.Minute, timetable[i].Departure.Second);

                //zkontrolovat,jestli uz neni cas odjezdu
                if (now > inTimetable)
                {
                    //nacti json data
                    TrainDataJSON td = new TrainDataJSON();
                    trainsList = td.LoadJson();

                    bool foundTrain = false;

                    //hledej v seznamu vlaku vlaky
                    foreach (Trains train in trainsList)
                    {
                        //nalezen hledany vlak, ktery ma stejne jmeno a nepohybuje se jeste
                        if (train.name == timetable[i].Name && train.move == 0)
                        {
                            foundTrain = true;

                            //vsechny stanice na nadrazi testuje, zdali vlak ma zadanou cilovou stanici nebo cilovou kolej
                            string finStation = null;
                            if (endTracks.Contains(timetable[i].FinalStation))
                                finStation = SearchLogic.getStationNameFromTrack(timetable[i].FinalStation);
                            else
                                finStation = timetable[i].FinalStation;

                            //najde vsechny mozne cilove koleje v dane stanici
                            List<string> getTracksForFinalPositon = SearchLogic.GetTracksForStation(finStation);

                            //v jizdnim radu je odjezd z nadrazi (vlak tam je) nebo v jizdnim radu je odjezd na specifickou kolej (vlak se na ni nachazi)
                            //vlak se jiz nachazi v cilove stanici - neni potreba zadna dalsi akce
                            if (train.currentPosition == timetable[i].FinalStation || getTracksForFinalPositon.Contains(train.currentPosition))
                            {
                                textForMsgBox = "Train " + train.name + " is already in the final station!";

                                popUpNotification(notificationType.warning, textForMsgBox);
                                break;
                            }

                            //preddefinvoani kolekci moznych pocatecnich a cilovych stanic
                            IEnumerable<string> fromStart = null;
                            IEnumerable<string> final = null;
                            bool crit = false;

                            //zjisteni jestli vlak je v kritickem useku
                            if (train.circuit == 0 || train.circuit == 4 || train.circuit == 7)
                            {
                                crit = true;
                                //najdi mozne pocatecni stanice v kritickem useku
                                fromStart = SearchLogic.GetStartStationInCritical(train.currentPosition, train.lastPosition);
                            }
                            else
                            {
                                crit = false;
                                //najdi mozne pocatecni stanice v otevrene trati
                                fromStart = SearchLogic.GetStartStationOutside(train.currentPosition, train.lastPosition);
                            }

                            //vlak je v kritickem useku
                            if (crit)
                            {
                                //najdi mozne finalni stanice v kritickem useku
                                final = SearchLogic.GetFinalStationInCritical(train.currentPosition, train.lastPosition);
                            }
                            else
                            {
                                //najdi mozne finalni stanice v otevrene trati
                                final = SearchLogic.GetFinalStationOutside(train.currentPosition, train.lastPosition);
                            }

                            //pokud vlak ma jet opacne, nez jel posledne, tak pocatecni stanice bude cilova a naopak - prohod hodnoty
                            if (timetable[i].Reverse != train.reverse)
                            {
                                IEnumerable<string> HelpVar = fromStart;
                                fromStart = final;
                                final = HelpVar;
                            }

                            //zjisteni jedinecnych pocatecnich a cilovych stanic
                            List<string> uniqueFinal = final.Distinct().ToList();
                            List<string> uniqueStart = fromStart.Distinct().ToList();

                            //obsahuji nazvy stanic pocatecni stanici dle jizdniho radu?
                            if (stationNames.Contains(timetable[i].StartStation))
                            {
                                startTrackSpecific = false;

                                //Je mozne aby vlak pri dane orientaci zacinal v teto stanici?
                                if (!uniqueStart.Contains(timetable[i].StartStation))
                                {
                                    //neni, konec, vlak nemuze jet
                                    textForMsgBox = train.name + " will not start. It's timetable data doesn't match possible START position with current direction " + (timetable[i].Reverse ? "reverse" : "ahaed" + ".");
                                    popUpNotification(notificationType.error, textForMsgBox);
                                    break;
                                }
                            }
                            else
                            {
                                startTrackSpecific = true;

                                //zna se pocatecni kolej, zjisti pocatecni stanici z nazvu koleje
                                string startStation = SearchLogic.getStationNameFromTrack(timetable[i].StartStation);
                                
                                //Je mozne aby vlak pri dane orientaci zacinal v teto stanici?
                                if (!uniqueStart.Contains(timetable[i].StartStation))
                                {
                                    //neni, konec, vlak nemuze jet
                                    textForMsgBox = train.name + " will not start. It's timetable data doesn't match possible START position with current direction " + (timetable[i].Reverse ? "reverse" : "ahaed" + ".");
                                    popUpNotification(notificationType.error, textForMsgBox);
                                    break;
                                }
                            }

                            //obsahuji nazvy stanic koncovou stanici dle jizdniho radu?
                            if (stationNames.Contains(timetable[i].FinalStation))
                            {
                                finalTrackSpecific = false;

                                //Je mozne aby vlak pri dane orientaci zacinal v teto stanici?
                                if (!uniqueFinal.Contains(timetable[i].FinalStation))
                                {
                                    //neni, konec, vlak nemuze jet
                                    textForMsgBox = train.name + " will not start. It's timetable data doesn't match possible FINAL position with current direction " + (timetable[i].Reverse ? "reverse" : "ahaed" + ".");
                                    popUpNotification(notificationType.error, textForMsgBox);
                                    break;
                                }
                            }
                            else
                            {
                                bool checkFinalTrack = false;

                                //najdi mozne cilove koleje pro cilovou stanici, ktera odpovida chtene cilove stanice
                                IEnumerable<XElement> finTrack = SearchLogic.GetFinalTrackOutside(train, finStation);

                                //okruh cilove stanice
                                int finalCircuit = SearchLogic.GetFinalStationCircuit(finStation);

                                //testovani jednotlivych objevenych cilovych useku
                                foreach (XElement x in finTrack)
                                {
                                    //testovani, zdali je vlak v kritickem useku u ciloveho nadrazi ci nikoliv
                                    //pokud neni a hleda se cesta z nadrazi A, ale jede do nadrazi B, tak jsou urcene cesty do cilovych stanic z jednotlivych okruhu
                                    //pokud ano, tak se z kolejich ve stanci vyberou ty, kam muze vlak jet
                                    if ((train.circuit == 0 && finalCircuit == 0)
                                        || (train.circuit == 4 && finalCircuit == 4)
                                        || (train.circuit == 7 && finalCircuit == 7))
                                    {
                                        bool bb;

                                        //najdi mozne koleje v zavislosti na orientaci
                                        if (timetable[i].Reverse == train.reverse)
                                            bb = SearchLogic.GetFinalTrackInside(train.currentPosition, train.lastPosition, x.Value);
                                        else
                                            bb = SearchLogic.GetFinalTrackInside(train.lastPosition, train.currentPosition, x.Value);

                                        //pokud na danou kolej vlak muze ze soucasne pozice jet
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

                                //pokud na danou stanici vlak nemùže jet
                                if (!checkFinalTrack)
                                {
                                    textForMsgBox = train.name + " will not start. It's timetable data doesn't match possible START position with current direction " + (timetable[i].Reverse ? "reverse" : "ahaed" + ".");
                                    popUpNotification(notificationType.error, textForMsgBox);
                                    break;
                                }
                            }

                            //Vlak muze podniknout cestu pri dane orientaci z bodu A do bodu B
                            //MainLogic.addNewTrainDataFromClient(train.name, train.currentPosition, (byte)timetable[i].Speed, timetable[i].Reverse, timetable[i].StartStation, timetable[i].FinalStation);
                            textForMsgBox = "Train " + train.name + " will be sent!";
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

                    //vlak nebyl nalezen!
                    if (!foundTrain)
                    {
                        textForMsgBox = "Train " + timetable[i].Name + " was not found!";
                        popUpNotification(notificationType.error, textForMsgBox);
                    }

                    //odstraneni zkontrolovaneho radku a aktualizace jizdniho radu
                    timetable.RemoveAt(i);
                    i--;
                    TimeInTimetableUpdated(sender, e);
                    

                }
                else
                    break;
            }
        }


        #region Actions with event handlers (load timetable, unit instruction, turnout instruction, loco move, info msg)

        /// <summary>
        /// Zpracovani vyvolaneho eventu pro nacteni jizdniho radu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void UserControl_ButtonLoadClick(object sender, EventArgs e)
        {
            //vyzvednuti dat
            List<DataToLoad> dataLoad = ucDataLoad.dataToLoads;

            //nacteni jizdniho radu
            for (int i = 0; i < dataLoad.Count; i++)
            {
                loadMyTimetamble(dataLoad[i].Filename, dataLoad[i].InfinityData);
            }

            //kontrola logiky tlacitek (kdy maji byt aktivni a kdy nikoliv)
            dataLoad.Clear();
            checkBtnLogic();

            string msg = "Timetable was successfully loaded!";
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
            List<SetNewTurnoutStops> newTurnoutStops = ucTurnoutsSettings.newTurnoutStops;

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
            List<SetNewTurnoutUnitData> newTurnoutData = ucTurnoutsSettings.newTurnoutData;

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

            timer1.Enabled = false;

            MessageBox.Show(msg, "IMPORTANT!!!",
            MessageBoxButtons.OK);

            popUpNotification(notificationType.warning, msg);
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
        #endregion

        /// <summary>
        /// Metoda, ktera zastavi vsechny vlaky
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
            foreach (int number in unitNumbers)
            {
                UnitInstructionPacket unitInst = new UnitInstructionPacket(ui, (byte)number, (byte)30);

                SendTCPData(unitInst.TCPPacket);
            }


            //nastaveni prodlevy pred natocenim servopohonu na minimalni hodnotu, aby se vyhybka prepnula temer okamzite
            ti = turnoutInstruction.nastaveni_prodlevy_pred_natocenim;
            IEnumerable<int> turnoutNumbers = SearchLogic.GetTurnoutIDs();
            foreach (int number in turnoutNumbers)
            {
                TurnoutInstructionPacket turnoutInst = new TurnoutInstructionPacket(ti, (byte)number, (byte)10);
                SendTCPData(turnoutInst.TCPPacket);
            }

            string msg = "Software stops for switches from configuration file were set!";
            popUpNotification(notificationType.success, msg);
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

                    popup.BodyColor = Color.FromArgb(170, 150, 0);
                    popup.TitleText = "Warning!";
                    popup.TitleColor = Color.White;
                    popup.TitleFont = new Font("Century Gothic", 19, FontStyle.Bold);

                    popup.ContentText = msg;
                    popup.ContentColor = Color.White;
                    popup.ContentFont = new Font("Century Gothic", 11);
                    popup.Popup();
                    break;

                case notificationType.success:
                    popup.Image = Properties.Resources.success;
                    popup.ImageSize = new(80, 80);

                    popup.BodyColor = Color.FromArgb(40, 120, 69);
                    popup.TitleText = "Success";
                    popup.TitleColor = Color.White;
                    popup.TitleFont = new Font("Century Gothic", 19, FontStyle.Bold);

                    popup.ContentText = msg;
                    popup.ContentColor = Color.White;
                    popup.ContentFont = new Font("Century Gothic", 11);
                    popup.Popup();

                    break;
                case notificationType.error:
                    popup.Image = Properties.Resources.error;
                    popup.ImageSize = new(80, 80);

                    popup.BodyColor = Color.FromArgb(220, 23, 29);
                    popup.TitleText = "Error!";
                    popup.TitleColor = Color.White;
                    popup.TitleFont = new Font("Century Gothic", 19, FontStyle.Bold);

                    popup.ContentText = msg;
                    popup.ContentColor = Color.White;
                    popup.ContentFont = new Font("Century Gothic", 11);
                    popup.Popup();
                    break;

            }
        }
    }
}