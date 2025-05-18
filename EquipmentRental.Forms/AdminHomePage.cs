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
    }
}
