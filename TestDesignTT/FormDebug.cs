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
using System.Windows.Markup;
using System.Xml;
using TrainTTLibrary;
using static TrainTTLibrary.Packet;

namespace TestDesignTT
{
    public partial class FormDebug : Form
    {
        private bool IsConnect = false;

        private static TCPClient klient = null;

        private static Dictionary<uint, DateTime> lastPacketTimeByNumberOfUnit = new Dictionary<uint, DateTime>();
        private static Dictionary<uint, System.Timers.Timer> timersByNumberOfUnit = new Dictionary<uint, System.Timers.Timer>();



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
        }

        #region Form actions (Load, Init, Closed, Resized)

        /// <summary>
        /// Definice event handleru vyvolanych v user controlech, spusteni TCP serveru a casovacu ridicich jednotek
        /// </summary>
        /// <param name="sender">Event vyvolany v user controlu</param>
        /// <param name="e">Event vyvolany v user controlu</param>
        private void FormDebug_Load(object sender, EventArgs e)
        {
            //definice event handleru vyuzivanych v user controlech
            uCmulti.MultiTurnoutButtonAddClick += new EventHandler(UserControl_MultiTurnoutClick); //user control pro screen Multiturnout
            uCTurnouts.TurnoutButtonSendClick += new EventHandler(UserControl_TurnoutClick); //user control pro screen Turnout
            uCLocomotives.ChangeOfTrainData += new EventHandler(UserControl_TrainDataChange); //user control pro zmenu rizeni vlaku
            uCEditJson.ButtonChangeJsonClick += new EventHandler(UserControl_EditJsonClick); //user control pro zmenu dat v JSONu
            uCUnitSet.UnitInstructionEventClick += new EventHandler(UserControl_UnitInstructionClick); //user control pro zmenu nastaveni ridici jednotky

            //spusteni tcp serveru
            StartTCPClient();

            //pocatecni spusteni timeru sledujici odbery proudu
            for (int i = 1; i < 3; i++)
            {
                SetTimers((uint)i);
            }
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

            //zastav timery, jak dlouho neposlali ridici jednotky zadna data
            foreach (var timer in timersByNumberOfUnit.Values)
            {
                timer.Stop();
            }

            Thread.Sleep(30);

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
                uCMap.setLabels();
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
            if (panelDesktopPanel.Controls.Contains(ucEditMoving)) //
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
        /// zpracovani dat ze serioveho portu
        /// </summary>
        /// <param name="e">Event na prichozi data z portu</param>
        private void DataProcessing(TCPReceivedEventArgs e)
        {

            if (e.data is String)
            {
                String s = e.data as String;

                //zjisteni, zdali se jedna o zpravu o obsazenosti useku
                if (Packet.RecognizeTCPType(s) == Packet.dataType.occupancy_section)
                {
                    //packet obsahujici data o usecich
                    OccupancySectionPacket occupancySectionPacket = new OccupancySectionPacket(s);

                    //uloz cas prijatych dat od ridici jednotky
                    lastPacketTimeByNumberOfUnit[(uint)occupancySectionPacket.NumberOfUnit] = DateTime.Now;

                    //nastav timery
                    SetTimers((uint)occupancySectionPacket.NumberOfUnit);

                }
            }
        }

        /// <summary>
        /// Nastaveni timeru jednotek posilani kolejovych useku
        /// </summary>
        /// <param name="numberOfUnit">Cislo sledovane ridici jednotky</param>
        private void SetTimers(uint numberOfUnit)
        {
            System.Timers.Timer timer = new System.Timers.Timer();

            // Je funkcni timer merici interval o obsazenosti useku daneho?
            if (!timersByNumberOfUnit.ContainsKey(numberOfUnit))
            {
                timer.Interval = 3500; // 4.5 sekundy
                timer.Elapsed += (sender, ev) => OnTimerElapsed(numberOfUnit);
                timersByNumberOfUnit[numberOfUnit] = timer;
                timer.Start();
            }
            else
            {
                // reset timeru pro danou jednotku
                timersByNumberOfUnit[numberOfUnit].Stop();
                timersByNumberOfUnit[numberOfUnit].Start();
            }
        }

        /// <summary>
        /// Vyhodnoceni casu od poslednich prijatych dat
        /// </summary>
        /// <param name="numberOfUnit">Cislo sledovane ridici jednotky</param>
        private void OnTimerElapsed(uint numberOfUnit)
        {
            DateTime lastPacketTime;

            //Pokud zadna data jeste nikdy neprisla (nemelo by nastat)
            if (!lastPacketTimeByNumberOfUnit.TryGetValue(numberOfUnit, out lastPacketTime))
            {
                return;
            }

            //Pokud byl prekroceny interval (nejaka porucha), tak zastav vlaky a vyrestartuj h-mustek
            if ((DateTime.Now - lastPacketTime).TotalSeconds >= 3.5)
            {
                StopAll();
                ResetAllBridges();
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
            DialogResult result = MessageBox.Show("Opravdu chcete odejít? JSte si jisti, že je nastavena správná poloha vlaků v JSONu?!?!", "DŮLEŽITÉ!!!",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                StopAll(); //zastav vlaky

                //vynulovani priznaku jizdy vsech vlaku (nemusi byt ale pro jistotu)
                trainsList = ControlLogic.MainLogic.GetData();
                foreach (Trains train in trainsList)
                {
                    train.move = "0";
                }

                //zobrazeni hlavniho menu
                FormMainMenu formmm = new FormMainMenu();   //nacti windows form pro hlavni menu
                formmm.StartPosition = FormStartPosition.Manual;
                formmm.Location = this.Location;
                formmm.Size = this.Size;
                this.Hide();
                formmm.Show();
            }
        }


        /// <summary>
        /// Akce na button Sections pro zobrazeni mapy
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnSections_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCMap);

            labelTitle.Text = (sender as Button).Text;
            uCMap.setLabels();
        }

        /// <summary>
        /// Akce na button pro rizeni lokomotiv
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnAddLoco_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCLocomotives);

            labelTitle.Text = (sender as Button).Text;
            //uCLocomotives.setLabels();
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
        /// Akce na button, ktery slouzi pro zobrazeni izolovanych useku
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnOccupancy_Click(object sender, EventArgs e)
        {
            DisplayInstance(uCOccupancy);
            labelTitle.Text = (sender as Button).Text;

            //List<Section> occupancySections = TCPServerTrainTT.Program.GetOccupancySections();
            List<Section> occupancySections = TrainTTLibrary.SectionInfo.listOfSection;

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
        /// Akce na button, ktery slouzi na vypnuti a zastaveni vsech lokomotiv
        /// </summary>
        /// <param name="sender">Event na stisknuti tlacitka</param>
        /// <param name="e">Event na stisknuti tlacitka</param>
        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            //Stop all trains
            StopAll();
        }


        #endregion

        /// <summary>
        /// Metoda, ktera zastavi vsechny lokomotivy z konfiguracniho souboru
        /// </summary>
        private void StopAll()
        {
            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {
                TrainMotionPacket trainMotionPacket = new TrainMotionPacket(locomotive, false, 3);

                SendTCPData(trainMotionPacket.TCPPacket);
            }
        }

        /// <summary>
        /// Metoda, ktera restartuje vsechny definovane H-mustky
        /// </summary>
        private void ResetAllBridges()
        {
            for (int i = 2; i < 4; i++)
            {
                UnitInstructionPacket unitInstructionPacket = new UnitInstructionPacket(unitInstruction.restart_H_mustku, (uint)i, (byte)1);
                SendTCPData(unitInstructionPacket.TCPPacket);
            }
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
            List<ChangeTrainData> trainDataChange = uCLocomotives.trainDataChange;

            bool foundMatch = false;

            foreach (Locomotive locomotive in LocomotiveInfo.listOfLocomotives)
            {
                string loco = locomotive.Name.ToString();

                foreach (ChangeTrainData data in trainDataChange)
                {
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
                        }

                        //zastavit lokomotivu, pro niz byla poslana data
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
            trainsList = ControlLogic.MainLogic.GetData();

            bool foundMatch = false;

            foreach (Trains train in trainsList)
            {
                foreach (ChangeJsonData data in changeData)
                {
                    //nalezena shoda mezi vlakem v JSONu a vybranym vlakem
                    if (train.name == data.Id)
                    {
                        //aktualizace dat, aby bylo mozne automaticke rizeni
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
            //uloz zpet do JSONu
            StoreJson sj = new StoreJson();
            sj.SaveJson(trainsList);

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
            List<Turnouts> turnouts = uCmulti.turnouts;
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
        }

        /// <summary>
        /// Metoda vyvolana Event Handlerem na zmenu vyhybek
        /// Prisel pozadavek z user controlu. Data budou vlozeno do packetu a zaslana
        /// </summary>
        /// <param name="sender">Event Handler z user controlu</param>
        /// <param name="e">Event Handler z user controlu</param>
        protected void UserControl_TurnoutClick(object sender, EventArgs e)
        {
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
    }
}
