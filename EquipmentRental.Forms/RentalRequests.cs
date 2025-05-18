using System;
using System.Linq;
using System.Windows.Forms;
using EquipmentRental.DataAccess.Models;

namespace EquipmentRental.Forms
{
    public partial class RentalRequests : Form
    {
        EquipmentRentalDbContext context;

        public RentalRequests()
        {
            InitializeComponent();
            context = new EquipmentRentalDbContext();
        }

        private void RentalRequests_Load(object sender, EventArgs e)
        {
            Loader();  // Load rental requests with default parameters

            // Populate ComboBox with filter options
            comboBoxStatusFilter.Items.Add("All");
            comboBoxStatusFilter.Items.Add("Pending");
            comboBoxStatusFilter.Items.Add("Approved");
            comboBoxStatusFilter.Items.Add("Rejected");
            comboBoxStatusFilter.SelectedIndex = 0; 
        }

        // Load and filter rental requests based on status and search
        private void Loader(string statusFilter = null, string searchKeyword = null)
        {
            var query = context.RentalRequests.AsQueryable();

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


        // Update the status of a rental request
        private void UpdateRentalRequestStatus(int requestId, string newStatus)
        {
            var rentalRequest = context.RentalRequests.SingleOrDefault(r => r.RentalRequestId == requestId);

            if (rentalRequest != null)
            {
                rentalRequest.Status = newStatus;
                context.SaveChanges();

                MessageBox.Show($"Rental Request status changed to {newStatus}.");
                Loader();  // Refresh the list after updating the status
            }
            else
            {
                MessageBox.Show("Rental Request not found.");
            }
        }

        // Approve the selected rental request
        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (dataGridViewRentalRequests.SelectedRows.Count > 0)
            {
                int requestId = Convert.ToInt32(dataGridViewRentalRequests.SelectedRows[0].Cells["RentalRequestId"].Value);

                UpdateRentalRequestStatus(requestId, "Approved");
            }
            else
            {
                MessageBox.Show("Please select a rental request to approve.");
            }
        }

        // Reject the selected rental request
        private void btnReject_Click(object sender, EventArgs e)
        {
            if (dataGridViewRentalRequests.SelectedRows.Count > 0)
            {
                int requestId = Convert.ToInt32(dataGridViewRentalRequests.SelectedRows[0].Cells["RentalRequestId"].Value);

                UpdateRentalRequestStatus(requestId, "Rejected");
            }
            else
            {
                MessageBox.Show("Please select a rental request to reject.");
            }
        }

        private void btnFilterSubmit_Click_1(object sender, EventArgs e)
        {
            string selectedStatus = comboBoxStatusFilter.SelectedItem.ToString();

            Loader(selectedStatus);


        }

        private void btnSearchSubmit_Click(object sender, EventArgs e)
        {
            string searchKeyword = txtSearch.Text.Trim();

            string selectedStatus = comboBoxStatusFilter.SelectedItem.ToString();

            Loader(selectedStatus, searchKeyword);  

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
    }
}
