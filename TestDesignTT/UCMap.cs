using ControlLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Xml.Linq;

namespace TestDesignTT
{
    public partial class UCMap : UserControl
    {

        //list aktualnich JSON dat
        private static List<Trains> trainsList = new List<Trains>();

        //zamek, aby nepristupovalo vice souboru ke stejnym metodam a nedochazelo ke kolizi
        public static readonly object locking = new object();

        //testovaci hodnoty pro nastaveni spravnych pozic na mape (pri aktualizaci polohy nevyuzito)
        int[] x_positions = { 380, 210, 115, 115, 300, 110, 60, 85, //1-8
            372, 250, 175, 140, 135, 155, 160, 555, //9-16
            320, 185, 685, 650, 360, 880, 870, 920, //17-24
            1122, 1170, 1440, 1500, 1375, 1800, 1670, 1680, //25-32
            430, 490, 455, 495, 555, 765, 505, 740, //33-40
            510, 525, 575, 585, 595, 640, 645, 775, //41-48
            710, 740, 685, 940, 700, 850, 830, 705,};
        int[] y_positions = { 1015, 1045, 1225, 1295, 995, 1040, 1220, 1470, //1-8
            1050, 1048, 1125, 1185, 1265, 1230, 1330, 1465, //9-16
            1485, 1470, 1485, 1510, 1540, 1540, 1485, 1510, //17-24
            1485, 1510, 1485, 1510, 1540, 1540, 1410, 1445, //25-32
            995, 1015, 1090, 1040, 1040, 1115, 1085, 1150, //33-40
            1105, 1135, 1145, 1165, 1015, 1180, 1205, 1220, //41-48
            1160, 1085, 1030, 1100, 1015, 1055, 1270, 1245};

        //konstanty odpovidajici velikosti obrazku
        int constant_x = 2000;
        int constant_y = 1610;

        //timer pro aktualizaci dat
        public static System.Timers.Timer refreshData;

        public UCMap()
        {
            InitializeComponent();
            setLabels();
            startTimer();
        }

        /// <summary>
        /// Nastaveni timeru, diky kteremu budou neustale vykreslovana nova data
        /// </summary>
        public void startTimer()
        {
            refreshData = new System.Timers.Timer(1000);

            refreshData.Elapsed += UpdateData_Tick;

            refreshData.AutoReset = true;

            refreshData.Enabled = true;
        }

        /// <summary>
        /// Timer
        /// KAzdou vterinu smaze veskera data a budou vykreslena nova
        /// Diky tomu bude kazdou vterinu aktualizovana poloha
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void UpdateData_Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            if (pictureBox1.IsHandleCreated)
            {
                pictureBox1.BeginInvoke(new Action(() =>
                {
                    lock (locking)
                    {
                        //nejprve odstran zobrazene labely
                        foreach (var control in pictureBox1.Controls.OfType<Label>().ToList())
                        {
                            pictureBox1.Controls.Remove(control);
                        }
                    }
                    //nastav nove
                    setLabels();
                }));
            }
        }

        /// <summary>
        /// Metoda pro vykresleni soucasne polohy v mape
        /// </summary>
        public void setLabels()
        {
            //zdaĺi se jedna oa testovani a zjistovani polohy (true) nebo ma vykreslovat polohu z jsonu (false)
            bool testing = true;

            //rozmery picture boxu pro nasledujici prepocet
            int width = pictureBox1.ClientSize.Width;
            int height = pictureBox1.ClientSize.Height;


            if (testing)
            {
                lock(locking)
                {
                    //vykresleni zadefinovanych bodu z hodni casti soubru
                    for (int i = 0; i < x_positions.Length; i++)
                    {
                        int x = (int)(width * x_positions[i] / constant_x); // center horizontally
                        int y = (int)(height * y_positions[i] / constant_y); // center vertically
                        Label label = new Label();
                        label.Text = "•" + (i + 1).ToString();
                        label.Font = new Font("Arial", 10, FontStyle.Bold);
                        label.BackColor = Color.Transparent;
                        label.AutoSize = true;
                        label.Location = new Point(x, y);
                        label.BorderStyle = BorderStyle.FixedSingle;

                        if (refreshData == null || !refreshData.Enabled)
                            return;


                        if (pictureBox1.IsHandleCreated)
                        {
                            pictureBox1.BeginInvoke(new Action(() => pictureBox1.Controls.Add(label)));
                            pictureBox1.BeginInvoke(new Action(() => label.BringToFront()));
                        }
                        else
                        {
                            pictureBox1.Controls.Add(label);
                            label.BringToFront();
                        }
                    }
                }
            }

            else
            {
                //nacteni jsounu se soucasnyma polohama
                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();

                lock (locking)
                {
                    //vykresleni aktualni polohy vlaku dle souradnic v konfiguracnim souboru
                    for (int i = 0; i < trainsList.Count; i++)
                    {
                        //ziskani souradnic soucasne polohy z konfiguracniho souboru
                        XElement mapCoordinates = SearchLogic.GetCoordinatesForSection(trainsList[i].currentPosition);

                        if (refreshData == null || !refreshData.Enabled)
                            return;


                        //v pripade, ze soucasna poloha je validni, vykresli data
                        if (mapCoordinates != null)
                        {
                            //konkretni souradnice
                            int xpos = int.Parse(mapCoordinates.Element("xpos").Value);
                            int ypos = int.Parse(mapCoordinates.Element("ypos").Value);

                            //parametry labelu pro vykresleni
                            int x = (int)(width * xpos / constant_x); // center horizontally
                            int y = (int)(height * ypos / constant_y); // center vertically
                            Label label = new Label();
                            label.Text = "•" + (i + 1).ToString();
                            label.Font = new Font("Arial", 10, FontStyle.Bold);
                            label.BackColor = Color.Transparent;
                            label.AutoSize = true;

                            label.BorderStyle = BorderStyle.FixedSingle;
                            label.ForeColor = Color.Black;

                            label.Location = new Point(x, y);


                            //vykresli data
                            if (pictureBox1.IsHandleCreated)
                            {
                                pictureBox1.BeginInvoke(new Action(() => pictureBox1.Controls.Add(label)));
                                pictureBox1.BeginInvoke(new Action(() => label.BringToFront()));
                            }
                            else
                            {
                                pictureBox1.Controls.Add(label);
                                label.BringToFront();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metoda vyvolana zmenou velikosti okna
        /// Diky tomu dojde k prizpusobeni labelu v mape
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UCMap_SizeChanged(object sender, EventArgs e)
        {
            lock (locking)
            {
                //pri zmene velikosti okna nejprve odstran predchozi labely
                foreach (var control in pictureBox1.Controls.OfType<Label>().ToList())
                {
                    pictureBox1.Controls.Remove(control);
                }
            }
            //nastav nove labely
            setLabels();
            
        }

        /// <summary>
        /// Metoda vyvolana zmenou velikosti okna
        /// Diky tomu dojde k prizpusobeni labelu v mape
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            lock (locking)
            {
                //pri zmene velikosti okna nejprve odstran vsechny predchozi labely
                foreach (var control in pictureBox1.Controls.OfType<Label>().ToList())
                {
                    pictureBox1.Controls.Remove(control);
                }
            }
            //nastav nove labely
            setLabels();
            
        }
    }
}
