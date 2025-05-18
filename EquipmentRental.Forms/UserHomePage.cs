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
    public partial class UserHomePage : Form
    {
        int userId = GlobalUserInfo.UserId;
        string userRole = GlobalUserInfo.UserRole;
        public UserHomePage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Access userId and userRole from the GlobalUserInfo class
            

            RentalRequestsUser rentalRequests = new RentalRequestsUser(userId, userRole);
            rentalRequests.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RentalRequestsUser rentalRequestsUser = new RentalRequestsUser(userId, userRole);
            rentalRequestsUser.Show();
        }
    }
}
