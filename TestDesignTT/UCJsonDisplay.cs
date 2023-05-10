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
using System.Xml;

namespace TestDesignTT
{

    public partial class UCJsonDisplay : UserControl
    {
        //list nesouci data JSONu preo zobrazeni
        private static List<Trains> trainsList = new List<Trains>();

        //timer, diky ktreremu dojde v pravidelnem intervalu ke znovunacteni dat
        public static System.Timers.Timer refreshData;

        private static BindingList<Trains> data = new BindingList<Trains>();

        public static readonly object locking = new object();

        public UCJsonDisplay()
        {
            InitializeComponent();
            displayJson();
            startTimer();
        }

        /// <summary>
        /// Spsuteni timeru aby dochazelo pravidelne k aktualizaci dat
        /// </summary>
        public void startTimer()
        {
            refreshData = new System.Timers.Timer(500);

            refreshData.Elapsed += UpdateData_Tick;

            refreshData.AutoReset = true;

            refreshData.Enabled = true;
        }

        /// <summary>
        /// Timer, ktery kazdou vterinu aktualizuje data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateData_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            //nutno pouzit i InvokeRequired z duvodu behu samostatneho vlakna
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.BeginInvoke(new Action(() =>
                {
                    displayJson();
                }));
            }
            else
            {
                displayJson();
            }
        }

        /// <summary>
        /// Metoda, ktera slouzi k zobrazeni aktualnich JSON hodnot
        /// </summary>
        public void displayJson()
        {
            lock (locking)
            {
                //data z JSONu

                TrainDataJSON td = new TrainDataJSON();
                trainsList = td.LoadJson();
                
                if (refreshData == null || !refreshData.Enabled)
                    return;

                //testovani, zdali mohu smazat aktualni data a jak je mohu smazat, aby nedoslo ke crashi aplikace
                if (dataGridView1.InvokeRequired)
                {
                    if (dataGridView1.IsHandleCreated)
                    {
                        dataGridView1.BeginInvoke(new Action(() =>
                        {
                            dataGridView1.Rows.Clear();
                            dataGridView1.DataSource = null;
                        }));
                    }
                }
                else
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.DataSource = null;
                }

                if (refreshData == null || !refreshData.Enabled)
                    return;

                //data do tabulky
                dataGridView1.DataSource = trainsList;

                //pridani jednotlivych radku
                for (int i = 0; i < trainsList.Count; i++)
                {
                    data.Add(trainsList[i]);
                }

                dataGridView1.DataSource = data;

                //nastaveni nazvu hlavicky pro kazdy sloupec JSONu
                dataGridView1.Columns[0].HeaderText = "ID";
                dataGridView1.Columns[1].HeaderText = "Name";
                dataGridView1.Columns[2].HeaderText = "Current position";
                dataGridView1.Columns[3].HeaderText = "Last position";
                dataGridView1.Columns[4].HeaderText = "Next position";
                dataGridView1.Columns[5].HeaderText = "Moving";
                dataGridView1.Columns[6].HeaderText = "Speed";
                dataGridView1.Columns[7].HeaderText = "Reverze";
                dataGridView1.Columns[8].HeaderText = "Orientation";
                dataGridView1.Columns[9].HeaderText = "Circuit";
                dataGridView1.Columns[10].HeaderText = "Start";
                dataGridView1.Columns[11].HeaderText = "End";
            }
        }
        
    }
}
