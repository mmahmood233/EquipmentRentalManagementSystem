using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EquipmentRental.DataAccess.Models;

namespace EquipmentRental.Forms
{
    public partial class AdminHomePage : Form
    {
        public AdminHomePage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RentalRequests rentalRequests = new RentalRequests();
            rentalRequests.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            RentalTransactionsAdmin rentalTransactionsAdmin = new RentalTransactionsAdmin();
            rentalTransactionsAdmin.Show();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            DashboardForm dashboardForm = new DashboardForm();
            dashboardForm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            EquipmentForm equipmentForm = new EquipmentForm();
            equipmentForm.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LogsForm logsForm = new LogsForm();
            logsForm.Show();
        }
    }
}
