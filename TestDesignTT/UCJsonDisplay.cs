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
        private static List<Trains> trainsList = new List<Trains>();

        public UCJsonDisplay()
        {
            InitializeComponent();
        }

        public void displayJson()
        {
            //data z JSONu
            TrainDataJSON td = new TrainDataJSON();
            trainsList = td.LoadJson();

            dataGridView1.DataSource = trainsList;

            BindingList<Trains> data = new BindingList<Trains>();

            for (int i = 0; i < trainsList.Count; i++)
            {
                data.Add(trainsList[i]);
            }

            dataGridView1.DataSource = data;
        }
    }
}
