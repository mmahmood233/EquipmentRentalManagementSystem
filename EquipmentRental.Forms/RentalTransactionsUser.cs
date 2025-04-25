using System;
using System.Linq;
using System.Windows.Forms;
using EquipmentRental.DataAccess.Models;

namespace EquipmentRental.Forms
{
    public partial class RentalTransactionsUser : Form
    {
        private int userId;
        private string userRole;
        private EquipmentRentalDbContext context;

        public RentalTransactionsUser(int userId, string userRole)
        {
            InitializeComponent();
            this.context = new EquipmentRentalDbContext();
            this.userId = userId;
            this.userRole = userRole;
        }

        private void RentalTransactionsUser_Load(object sender, EventArgs e)
        {
            LoadRentalTransactions();

            comboBoxPaymentStatus.Items.Add("All");
            comboBoxPaymentStatus.Items.Add("Pending");
            comboBoxPaymentStatus.Items.Add("Paid");
            comboBoxPaymentStatus.Items.Add("Overdue");

            comboBoxPaymentStatus.SelectedIndex = 0;
        }

        private void LoadRentalTransactions(string statusFilter = null, string searchKeyword = null)
        {
            var query = context.RentalTransactions.AsQueryable();

            query = query.Where(rt => rt.CustomerId == userId);

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                query = query.Where(rt => rt.PaymentStatus == statusFilter);
            }

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query = query.Where(rt => rt.Equipment.Name.Contains(searchKeyword) ||
                                           rt.RentalRequest.Description.Contains(searchKeyword));
            }

            var rentalTransactions = query.ToList();

            dataGridViewRentalTransactions.DataSource = rentalTransactions;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string selectedStatus = comboBoxPaymentStatus.SelectedItem.ToString();
            LoadRentalTransactions(selectedStatus);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();
            string selectedStatus = comboBoxPaymentStatus.SelectedItem.ToString();
            LoadRentalTransactions(selectedStatus, searchKeyword);
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            LoadRentalTransactions();
        }

        private void btnCreateTransaction_Click(object sender, EventArgs e)
        {
            CreateTransactionUser createTransactionUser = new CreateTransactionUser();
            createTransactionUser.ShowDialog();
        }
    }
}
