using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;


namespace TestDesignTT
{
    public partial class FormTimetable : Form
    {
        public Form f1;
        private int test = 0, countTest = 0;

        public BindingList<DataTimetable> timetable = new BindingList<DataTimetable>();

        public BindingList<MovingInTimetamble> movingTimetable = new BindingList<MovingInTimetamble>();

        //private List<DataForTimetable> dataForTimetable = new List<DataForTimetable>();
        

        private string myFileName = String.Empty;
        private int myRowIndex = 0;


        UCDataAdd ucDataAdd = new UCDataAdd();
        UCEditTimetable ucEditTimetable = new UCEditTimetable();
        UCTrainTimetable ucTrainTimetable = new UCTrainTimetable();
        UCEditMoving ucEditMoving = new UCEditMoving();
        UCDataLoad ucDataLoad = new UCDataLoad();
        UCTrainMoving ucTrainMoving = new UCTrainMoving();
        UCHome uCHome = new UCHome();


        public string fileName
        {
            set { myFileName = value; }
            get { return myFileName; }
        }

        /*
        public int myCountTest
        {
            set { countTest = value; }
            get { return countTest; }
        }
        */

        public int indexOfRow
        {
            set { myRowIndex = value; }
            get { return myRowIndex; }
        }


        


        public FormTimetable()
        {
            InitializeComponent();
            customizeDesing();
            DisplayInstance(uCHome);
        }

        private void FormMainMenu_Load(object sender, EventArgs e)
        {
            ucDataLoad.ButtonLoadClick += new EventHandler(UserControl_ButtonLoadClick);
            ucEditTimetable.ButtonDeleteTTClick += new EventHandler(UserControl_ButtonDeleteTTClick);
            ucEditTimetable.ButtonEditTTClick += new EventHandler(UserControl_ButtonEditTTClick);
            ucDataAdd.ButtonYesAddTrainClick += new EventHandler(UserControl_ButtonYesAddTrainClick);
            ucDataAdd.ButtonNoAddTrainClick += new EventHandler(UserControl_ButtonNoAddTrainClick);
            ucEditMoving.ButtonEditMoving += new EventHandler(UserControl_ButtonEditMovingClick);
        }

        protected void UserControl_ButtonLoadClick(object sender, EventArgs e)
        {
            //handle the event 
            myFileName = ucDataLoad.fileName;
            this.fileName = ucDataLoad.fileName;
            loadMyTimetamble();

        }

        protected void UserControl_ButtonDeleteTTClick(object sender, EventArgs e)
        {
            this.indexOfRow = ucEditTimetable.rowIndex;
            timetable.RemoveAt(indexOfRow);
            ucEditTimetable.loadTimetamble(timetable);
        }

        protected void UserControl_ButtonEditTTClick(object sender, EventArgs e)
        {
            this.indexOfRow = ucEditTimetable.rowIndex;
            timetable.RemoveAt(indexOfRow);
            ucEditTimetable.loadTimetamble(timetable);
        }

        protected void UserControl_ButtonYesAddTrainClick(object sender, EventArgs e)
        {
            bool addNow = false;
            DateTime nDeparture;
            if (ucDataAdd.Departure == default(DateTime))
            {
                nDeparture = DateTime.Now;
                addNow = true;
            }
            else
            {
                nDeparture = ucDataAdd.Departure;
                addNow = false;
            }


            if (addNow)
            {
                DateTime now = new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                //DateTime inTimetable = new DateTime(1, 1, 1, timetable[i].Departure.Hour, timetable[i].Departure.Minute, timetable[i].Departure.Second);
                double sec = now.Subtract(now).TotalSeconds;
                MovingInTimetamble mit = new MovingInTimetamble(ucDataAdd.Type, ucDataAdd.StartStation, ucDataAdd.FinalStation, nDeparture, ucDataAdd.Speed, ucDataAdd.Reverse, 1500, sec);
                movingTimetable.Add(mit);
            }

            hideSubMenu();
            DisplayInstance(ucTrainMoving);
            ucDataAdd.clearData();
            ucTrainMoving.loadMovingTimetamble(movingTimetable);
        }

        protected void UserControl_ButtonNoAddTrainClick(object sender, EventArgs e)
        {
            ucDataAdd.clearData();
        }

        protected void UserControl_ButtonEditMovingClick(object sender, EventArgs e)
        {
            //TODO
        }

        private void TimeInTimetableUpdated(object sender, EventArgs e)
        {
            if (panelDesktopPanel.Controls.Contains(ucTrainTimetable))
                ucTrainTimetable.loadTimetamble(timetable);
            if (panelDesktopPanel.Controls.Contains(ucEditTimetable))
                ucEditTimetable.loadTimetamble(timetable);
            if (panelDesktopPanel.Controls.Contains(ucTrainMoving))
                ucTrainMoving.loadMovingTimetamble(movingTimetable);
            if (panelDesktopPanel.Controls.Contains(ucEditMoving))
                ucEditMoving.loadData(movingTimetable,timetable);
        }

        private void FormMainMenu_SizeChanged(object sender, EventArgs e)
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

        private void customizeDesing()
        {
            panelTimetable.Visible = false;
            panelModifyData.Visible = false;
            //OpenChildForm(new Forms.FormHome(), sender);
        }

        private void hideSubMenu()
        {
            if (panelTimetable.Visible == true)
                panelTimetable.Visible = false;
            if (panelModifyData.Visible == true)
                panelModifyData.Visible = false;
        }

        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                hideSubMenu();
                subMenu.Visible = true;
            }
            else
                subMenu.Visible = false;
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


        private void btnMovingTrain_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            DisplayInstance(ucTrainMoving);
            labelTitle.Text = (sender as Button).Text;

            ucTrainMoving.loadMovingTimetamble(movingTimetable);


        }

        private void btnDisplayTimetable_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            //myFileName = MyControls.UCTrainTimetable.fileName;


            //DisplayInstance(UCTrainTimetable.Instance);
            //UCTrainTimetable ucTrainTimetable = new UCTrainTimetable();
            DisplayInstance(ucTrainTimetable);

            labelTitle.Text = (sender as Button).Text;
            ucTrainTimetable.loadTimetamble(timetable);
        }


        private void btnTimetable_Click(object sender, EventArgs e)
        {
            showSubMenu(panelTimetable);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            //DisplayInstance(UCHome.Instance);

            //UCHome uCHome = new UCHome();
            DisplayInstance(uCHome);
            labelTitle.Text = (sender as Button).Text;
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            //UCDataLoad ucDataLoad = new UCDataLoad();
            DisplayInstance(ucDataLoad);
            labelTitle.Text = (sender as Button).Text;
        }

        private void btnModifyData_Click(object sender, EventArgs e)
        {
            showSubMenu(panelModifyData);
        }

        private void btnAddTrain_Click(object sender, EventArgs e)
        {
            //ucDataAdd.clearData();
            hideSubMenu();
            DisplayInstance(ucDataAdd);
            labelTitle.Text = (sender as Button).Text;
        }


        private void btnEditTrain_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            DisplayInstance(ucEditMoving);
            labelTitle.Text = (sender as Button).Text;
            ucEditMoving.loadData(movingTimetable, timetable);
        }

        private void btnEditTimetable_Click(object sender, EventArgs e)
        {


            hideSubMenu();
            DisplayInstance(ucEditTimetable);

            labelTitle.Text = (sender as Button).Text;
            ucEditTimetable.loadTimetamble(timetable);
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void btnCentralStop_Click(object sender, EventArgs e)
        {
            hideSubMenu();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            hideSubMenu();
            FormMainMenu formmm = new FormMainMenu();
            formmm.StartPosition = FormStartPosition.Manual;
            formmm.Location = this.Location;
            formmm.Size = this.Size;
            this.Hide();
            formmm.Show();
            //FormMainMenu.;
        }

        public void loadMyTimetamble()
        {
            if (!String.IsNullOrEmpty(fileName))
            {
                try
                {
                    String[] lines = File.ReadAllLines(fileName); //, Encoding.GetEncoding("Windows-1250"));

                    timetable.Clear();

                    //dataGridView1.DataSource = timetable;

                    //dataForTimetable.Clear();

                    foreach (String line in lines)
                    {
                        /*
                        if (line.Contains("***"))
                        {
                            DataForTimetable data = new DataForTimetable(line);
                            dataForTimetable.Add(data);

                        }
                        else
                        {
                        */
                            DataTimetable note = new DataTimetable(line);
                            timetable.Add(note);
                        //}
                    }



                    /*
                    BindingList<DataTimetable> onScreen = new BindingList<DataTimetable>();

                    for (int i = 0; i < timetable.Count; i++)
                    {
                        onScreen.Add(timetable[i]);
                        //onScreen[i].FinalStation.Name = Packet.UnderLineToGap(onScreen[i].FinalStation.Name);
                        //onScreen[i].StartStation.Name = Packet.UnderLineToGap(onScreen[i].StartStation.Name);
                    }
                    */

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

                        //timetable.RemoveItem(timetable[i]);
                        //foreach (DataTimetable timetable in this.timetable)
                        //  timetable.Remove(timetable);
                        //onScreen[i].FinalStation.Name = Packet.UnderLineToGap(onScreen[i].FinalStation.Name);
                        //onScreen[i].StartStation.Name = Packet.UnderLineToGap(onScreen[i].StartStation.Name);
                    }

                    //dataGridView1.DataSource = onScreen;
                    //titleDisplayData.Text = "Toto jsou data jízdního øádu.";

                }
                catch (IOException)
                {
                    MessageBox.Show("An I/O error occurred while opening the file...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                //ucEditMoving.testLabelText.Text = "Naètìte data pro jízdní øád!";
                //titleDisplayData.Text = "Naètìte data pro jízdní øád!";
                //MessageBox.Show("Empty file...", "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
            }
        }

        /*
        private void FormTimetable_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormLocation = this.Location;
            FormSize = this.Size;
        }
        */

        private void timer1_Tick(object sender, EventArgs e)
        {

            for (int i = 0; i < timetable.Count; i++)
            {
                DateTime now = new DateTime(1, 1, 1, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

                DateTime inTimetable = new DateTime(1, 1, 1, timetable[i].Departure.Hour, timetable[i].Departure.Minute, timetable[i].Departure.Second);

                //zkontrolovat,jestli uz neni cas odjezdu
                if (now > inTimetable)
                {

                    double sec = now.Subtract(inTimetable).TotalSeconds;
                    MovingInTimetamble mit = new MovingInTimetamble(timetable[i].Type, timetable[i].StartStation, timetable[i].FinalStation, timetable[i].Departure, 0.7, true, 1500, sec);
                    movingTimetable.Add(mit);
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
    }
}