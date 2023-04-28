using Microsoft.Win32;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Documents;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;


namespace TestDesignTT
{
    public partial class FormTimetable : Form
    {
        public BindingList<DataTimetable> timetable = new BindingList<DataTimetable>();

        public BindingList<MovingInTimetamble> movingTimetable = new BindingList<MovingInTimetamble>();

        //private List<DataForTimetable> dataForTimetable = new List<DataForTimetable>();


        UCHome uCHome = new UCHome();
        UCTrainTimetable ucTrainTimetable = new UCTrainTimetable();
        UCDataLoad ucDataLoad = new UCDataLoad();
        UCJsonDisplay uCJsonDisplay = new UCJsonDisplay();
        UCMap uCMap = new UCMap();
        UCUnitSet uCUnitSet = new UCUnitSet();
        UCTurnoutsSettings ucTurnoutsSettings = new UCTurnoutsSettings();

        public FormTimetable()
        {
            InitializeComponent();
            DisplayInstance(uCHome);
            panelSettings.Visible = false;
            btnEnabledLogic();
        }

        private void FormMainMenu_Load(object sender, EventArgs e)
        {
            ucDataLoad.ButtonLoadClick += new EventHandler(UserControl_ButtonLoadClick);

        }

        protected void UserControl_ButtonLoadClick(object sender, EventArgs e)
        {
            //handle the event 


            //List<DataToLoad> dataLoad = new List<DataToLoad>();
            List<DataToLoad> dataLoad = ucDataLoad.dataToLoads;

            for (int i = 0; i < dataLoad.Count; i++)
            {
                loadMyTimetamble(dataLoad[i].Filename, dataLoad[i].InfinityData);
            }

            dataLoad.Clear();
            btnEnabledLogic();

        }

        private void TimeInTimetableUpdated(object sender, EventArgs e)
        {
            if (panelDesktopPanel.Controls.Contains(ucTrainTimetable))
                ucTrainTimetable.loadTimetamble(timetable);
        }

        private void FormMainMenu_SizeChanged(object sender, EventArgs e)
        {
            //TODO

            /*
            if (panelDesktopPanel.Controls.Contains(ucEditMoving))
                ucEditMoving.changeSize();
            if (panelDesktopPanel.Controls.Contains(ucTrainTimetable))
                ucTrainTimetable.changeSize();
            if (panelDesktopPanel.Controls.Contains(ucEditTimetable))
                ucEditTimetable.changeSize();
            if (panelDesktopPanel.Controls.Contains(ucTrainMoving))
                ucTrainMoving.changeSize();
            */
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
                        departure = DateTime.Today.Add(new TimeSpan(4, 0, 0));
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
                                    departure = departure.AddSeconds(45);
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

            for (int i = 0; i < timetable.Count; i++)
            {
                DateTime now = new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                DateTime inTimetable = new DateTime(1, 1, 1, timetable[i].Departure.Hour, timetable[i].Departure.Minute, timetable[i].Departure.Second);

                //zkontrolovat,jestli uz neni cas odjezdu
                if (now > inTimetable)
                {
                    /*
                    double sec = now.Subtract(inTimetable).TotalSeconds;
                    MovingInTimetamble mit = new MovingInTimetamble(timetable[i].Type, timetable[i].StartStation, timetable[i].FinalStation, timetable[i].Departure, 0.7, true, 1500, sec);
                    movingTimetable.Add(mit);
                    */

                    //mit.TimeOnTrack = sec;
                    timetable.RemoveAt(i);
                    i--;
                    TimeInTimetableUpdated(sender, e);
                    //tady bude dodelano poslani packetu

                }
                else
                    break;
            }



            for (int i = 0; i < movingTimetable.Count; i++)
            {
                DateTime now = new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                DateTime inTimetable = new DateTime(1, 1, 1, movingTimetable[i].Departure.Hour, movingTimetable[i].Departure.Minute, movingTimetable[i].Departure.Second);

                double sec = now.Subtract(inTimetable).TotalSeconds;

                movingTimetable[i].TimeOnTrack = sec;

                //TimeInTimetableUpdated(sender, e);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            //DisplayInstance(UCHome.Instance);

            //UCHome uCHome = new UCHome();
            panelSettings.Visible = false;
            btnEnabledLogic();

            DisplayInstance(uCHome);
            labelTitle.Text = (sender as Button).Text;
        }

        private void btnSections_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            btnEnabledLogic();

            DisplayInstance(uCMap);
            labelTitle.Text = (sender as Button).Text;
        }

        private void btnJSON_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            btnEnabledLogic();

            DisplayInstance(uCJsonDisplay);
            uCJsonDisplay.displayJson();

            labelTitle.Text = (sender as Button).Text;
        }

        private void btnLoadTimetable_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            btnEnabledLogic();

            DisplayInstance(ucDataLoad);

            labelTitle.Text = (sender as Button).Text;
        }

        private void btnDisplayTimetable_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            btnEnabledLogic();

            DisplayInstance(ucTrainTimetable);
            labelTitle.Text = (sender as Button).Text;

            ucTrainTimetable.loadTimetamble(timetable);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = true;
            btnEnabledLogic();
        }

        private void btnUnitSettings_Click(object sender, EventArgs e)
        {
            btnEnabledLogic();

            DisplayInstance(uCUnitSet);
            labelTitle.Text = (sender as Button).Text;
        }

        private void btnTurnoutSettings_Click(object sender, EventArgs e)
        {
            btnEnabledLogic();

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
            }
            btnEnabledLogic();
        }

        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
            btnEnabledLogic();
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

        private void btnEnabledLogic()
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
            }
            else
            {
                btnLoadTimetable.Enabled = true;
            }
        }

    }
}