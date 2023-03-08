using System.ComponentModel;

namespace TestDesignTT
{
    public partial class UCEditTimetable : UserControl
    {

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonDeleteTTClick;

        [Browsable(true)]
        [Category("Action")]
        [Description("Invoked when user clicks button")]
        public event EventHandler ButtonEditTTClick;

        private int rowIndexAction = 0;


        public int rowIndex
        {
            set { rowIndexAction = value; }
            get { return rowIndexAction; }
        }

        public UCEditTimetable()
        {
            InitializeComponent();
            buttonsClickable(false);
        }


        public void loadTimetamble(BindingList<DataTimetable> timetable)
        {

            if (timetable.Count < 1)
                titleDisplayData.Text = "Žádné vlaky nebyly načteny";
            else
                titleDisplayData.Text = "Zde lze upravit data v jízdním řádu";

            BindingList<DataTimetable> onScreen = new BindingList<DataTimetable>();

            for (int i = 0; i < timetable.Count; i++)
            {
                onScreen.Add(timetable[i]);
            }

            dataGridView1.DataSource = onScreen;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            dataGridView1.Columns[5].Visible = false;

            changeSize();
        }

        private void buttonsClickable(bool b)
        {
            if (b)
            {
                btnEditTT.Enabled = true;
                btnDeleteTT.Enabled = true;
            }
            else
            {
                btnEditTT.Enabled = false;
                btnDeleteTT.Enabled = false;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            buttonsClickable(true);
        }

        private void btnDeleteTT_Click(object sender, EventArgs e)
        {
            int rowIndexToDelete = dataGridView1.CurrentCell.RowIndex;
            this.rowIndex = rowIndexToDelete;
            ButtonDeleteTTClick?.Invoke(this, e);
        }

        private void btnEditTT_Click(object sender, EventArgs e)
        {
            int rowIndexToEdit = dataGridView1.CurrentCell.RowIndex;
            this.rowIndex = rowIndexToEdit;
            ButtonEditTTClick?.Invoke(this, e);
        }

        public void changeSize()
        {
            panel4.Height = dataGridView1.ColumnHeadersHeight + (dataGridView1.RowCount+1) * dataGridView1.RowTemplate.Height;

            if (dataGridView1.RowCount < 1)
                panel4.Height = 0;
        }
    }
}
