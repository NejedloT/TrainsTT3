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

    public partial class UCDisplayJson : UserControl
    {
        private static List<Trains> trainsList = new List<Trains>();

        public UCDisplayJson()
        {
            InitializeComponent();
        }

        public void displayJson()
        {
            trainsList = ControlLogic.MainLogic.GetData();
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
