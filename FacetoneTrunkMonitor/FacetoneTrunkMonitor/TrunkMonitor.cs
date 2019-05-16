using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkAPIHandler;

namespace FacetoneTrunkMonitor
{
    public partial class TrunkMonitor : Form
    {
        public TrunkMonitor()
        {
            InitializeComponent();
        }

        public delegate void UpdateGrid(List<Trunk> trunks);

        private void UpdateUI(List<Trunk> trunks)
        {
            dataGridTrunkList.DataSource = trunks;
            if(dataGridTrunkList.Rows[0] != null && dataGridTrunkList.Rows[0].Cells[0] != null)
            {
                dataGridTrunkList.Rows[0].Cells[0].Selected = false;
            }            

            foreach (DataGridViewRow row in dataGridTrunkList.Rows)
                if (row.Cells[2].Value != null && row.Cells[2].Value.ToString() == "UP")
                {
                    row.DefaultCellStyle.BackColor = Color.Green;
                }
                else if (row.Cells[2].Value != null && row.Cells[2].Value.ToString() == "DOWN")
                {
                    row.DefaultCellStyle.BackColor = Color.OrangeRed;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.Gold;
                }
        }

        private void TrunkMonitor_Load(object sender, EventArgs e)
        {
            //CALL API AND GET TRUNK INFORMATION
            var trunkHandler = new TrunkHandler();

            Task.Run(async () =>
            {
                while (true)
                {
                    // do the work in the loop
                    string newData = DateTime.Now.ToLongTimeString();

                    var trunksList = trunkHandler.GetTrunks();

                    // update the UI on the UI thread
                    dataGridTrunkList.Invoke(new UpdateGrid(UpdateUI), trunksList);

                    // don't run again for at least 200 milliseconds
                    await Task.Delay(60000);
                }
            });


        }
    }
}
