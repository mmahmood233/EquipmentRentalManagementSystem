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
    public partial class RentalRequestsUser : Form
    {

        private int userId;
        private string userRole;
        EquipmentRentalDbContext context;


      

        public RentalRequestsUser(int userId, string userRole)
        {
            InitializeComponent();
            this.context = new EquipmentRentalDbContext();
            this.userId = userId; // Store UserId
            this.userRole = userRole; // Store UserRole
        }

        private void RentalRequestsUser_Load(object sender, EventArgs e)
        {
            Loader();  

            comboBoxStatusFilter.Items.Add("All");
            comboBoxStatusFilter.Items.Add("Pending");
            comboBoxStatusFilter.Items.Add("Approved");
            comboBoxStatusFilter.Items.Add("Rejected");
            comboBoxStatusFilter.SelectedIndex = 0;
        }


        private void Loader(string statusFilter = null, string searchKeyword = null)
        {
            var query = context.RentalRequests.AsQueryable();

            // Filter the rental requests based on the logged-in user
            query = query.Where(r => r.CustomerId == userId);

            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                query = query.Where(r => r.Status == statusFilter);
            }

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                query = query.Where(r => r.Customer.FullName.Contains(searchKeyword) || r.Equipment.Name.Contains(searchKeyword));
            }

            var rentalRequests = query.ToList();

            dataGridViewRentalRequests.DataSource = rentalRequests;
        }


        private void btnSearchSubmit_Click(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();

            string selectedStatus = comboBoxStatusFilter.SelectedItem.ToString();

            Loader(selectedStatus, searchKeyword);
        }

        private void btnFilterSubmit_Click(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();

            string selectedStatus = comboBoxStatusFilter.SelectedItem.ToString();

            Loader(selectedStatus, searchKeyword);
        }





        private void ViewTransactionAndReturnDetails(int requestId)
        {
            var rentalRequest = context.RentalRequests.SingleOrDefault(r => r.RentalRequestId == requestId);

            if (rentalRequest != null && rentalRequest.Status == "Approved")
            {
                var transactionRecord = context.RentalTransactions.SingleOrDefault(t => t.RentalRequestId == rentalRequest.RentalRequestId);

                string transactionDetails = string.Empty;
                string returnDetails = string.Empty;

                if (transactionRecord != null)
                {
                    transactionDetails = $"Rental Fee: {transactionRecord.RentalFee}\n" +
                                         $"Rental Period: {transactionRecord.RentalPeriod} days\n" +
                                         $"Deposit: {transactionRecord.Deposit}\n" +
                                         $"Payment Status: {transactionRecord.PaymentStatus}";

                    var returnRecord = context.ReturnRecords.SingleOrDefault(r => r.RentalTransactionId == transactionRecord.RentalTransactionId);

                    if (returnRecord != null)
                    {
                        returnDetails = $"Actual Return Date: {returnRecord.ActualReturnDate}\n" +
                                        $"Return Condition: {returnRecord.ReturnCondition}\n" +
                                        $"Late Return Fee: {returnRecord.LateReturnFee}\n" +
                                        $"Additional Charges: {returnRecord.AdditionalCharges}\n" +
                                        $"Notes: {returnRecord.Notes}";
                    }
                    else
                    {
                        returnDetails = "No return record found.";
                    }
                }
                else
                {
                    transactionDetails = "No transaction record found.";
                }

                var detailsForm = new TransactionReturnDetailsForm(transactionDetails, returnDetails);
                detailsForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("This rental request is not approved or no records exist.");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridViewRentalRequests.SelectedRows.Count > 0)
            {
                int requestId = Convert.ToInt32(dataGridViewRentalRequests.SelectedRows[0].Cells["RentalRequestId"].Value);

                ViewTransactionAndReturnDetails(requestId);
            }
            else
            {
                MessageBox.Show("Please select a rental request to view details.");
            }
        }
    }
}
