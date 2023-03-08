using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDesignTT
{
    public partial class UCHome : UserControl
    {
        private int test = 0;
        private int test2 = 0;

        public static int myTest
        {
            set ;
            get;
        }

        /*
        private static UCHome? _instance;
        public static UCHome Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UCHome();
                return _instance;
            }
        }
        */

        public UCHome()
        {
            InitializeComponent();
            UCHome_Load();
            //UCDataLoad ucDataLoad = new UCDataLoad();
            //ucDataLoad.ButtonLoadClick += new EventHandler(UserControl_ButtonLoadClick);
        }

        protected void UserControl_ButtonLoadClick(object sender, EventArgs e)
        {
            //handle the event 
            test2++;
            labelMyTest.Text = test2.ToString();
        }


        private void UCHome_Load()
        {
            timer1.Start();
            labelTime.Text = DateTime.Now.ToLongTimeString();
            labelDate.Text = DateTime.Now.ToLongDateString();
            labelMyTest.Text = "Zatim jen nejakej prazdnej label";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            test++;
            labelTime.Text = DateTime.Now.ToLongTimeString();
            labelTestCount.Text = test.ToString();
            //labelMyTest.Text = myTest.ToString();
        }
    }
}
