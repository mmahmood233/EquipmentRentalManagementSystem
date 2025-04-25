using System;
using System.Linq;
using System.Windows.Forms;
using EquipmentRental.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.Forms
{
    public partial class RentalTransactionsAdmin : Form
    {
        private EquipmentRentalDbContext context;
        private int? selectedTransactionId = null; // Store the selected transaction ID for update

        public RentalTransactionsAdmin()
        {
            InitializeComponent();
            context = new EquipmentRentalDbContext();
        }

        private void RentalTransactionsAdmin_Load(object sender, EventArgs e)
        {
            loader();  // Load all transactions when the form loads
        }

        private void loader()
        {
            comboBoxPaymentStatus.Items.Clear();
            comboBoxPaymentStatus.Items.Add("All");
            comboBoxPaymentStatus.Items.Add("Pending");
            comboBoxPaymentStatus.Items.Add("Paid");
            comboBoxPaymentStatus.Items.Add("Overdue");

            comboBoxPaymentStatus.SelectedIndex = 0;

            var transactions = context.RentalTransactions
                                    .Include(t => t.Equipment)
                                    .Include(t => t.RentalRequest)
                                    .ToList();  // Retrieve all transactions

            // Bind the result to a DataGridView
            dataGridViewRentalTransactions.DataSource = transactions;
        }

        private void dataGridViewRentalTransactions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Get the selected transaction ID from the DataGridView
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewRentalTransactions.Rows[e.RowIndex];

                // Populate the selectedTransactionId for updating
                selectedTransactionId = Convert.ToInt32(row.Cells["RentalTransactionId"].Value);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Ensure a transaction is selected before updating
            if (selectedTransactionId.HasValue)
            {
                // Open the Update Transaction form and pass the selected transaction's ID
                UpdateTransactionForm updateForm = new UpdateTransactionForm(selectedTransactionId.Value);
                updateForm.ShowDialog();

                // After updating, refresh the grid to show updated data
                loader();
            }
            else
            {
                MessageBox.Show("Please select a transaction to update.");
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchKeyword))
            {
                MessageBox.Show("Please enter a search keyword.");
                return;
            }
            SearchTransactions(searchKeyword);  // Use the search method
        }

        private void SearchTransactions(string searchKeyword)
        {
            var transactions = context.RentalTransactions
                                      .Include(t => t.Equipment)
                                      .Include(t => t.RentalRequest)
                                      .Where(t => t.Equipment.Name.Contains(searchKeyword) ||
                                                 t.RentalRequest.Description.Contains(searchKeyword))
                                      .ToList();

            dataGridViewRentalTransactions.DataSource = transactions;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            // Ensure the user has selected a valid status
            if (comboBoxPaymentStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment status to filter.");
                return;
            }

            string selectedStatus = comboBoxPaymentStatus.SelectedItem.ToString();
            FilterTransactions(selectedStatus);  // Use the filter method
        }

        private void FilterTransactions(string statusFilter)
        {
            if (statusFilter == "All")
            {
                // If "All" is selected, reload all transactions without any filtering
                loader();
            }
            else
            {
                var filteredTransactions = context.RentalTransactions
                                                  .Include(t => t.Equipment)
                                                  .Where(t => t.PaymentStatus == statusFilter)
                                                  .ToList();

                dataGridViewRentalTransactions.DataSource = filteredTransactions;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateTransaction createTransaction = new CreateTransaction();
            createTransaction.ShowDialog();

            loader();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            loader();
        }
    }
}
