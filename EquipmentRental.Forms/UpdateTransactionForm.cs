using System;
using System.Linq;
using System.Windows.Forms;
using EquipmentRental.DataAccess.Models;

namespace EquipmentRental.Forms
{
    public partial class UpdateTransactionForm : Form
    {
        private EquipmentRentalDbContext context;
        private int transactionId;

        public UpdateTransactionForm(int transactionId)
        {
            InitializeComponent();
            this.transactionId = transactionId;
            context = new EquipmentRentalDbContext();
        }

        private void UpdateTransactionForm_Load(object sender, EventArgs e)
        {
            // Load payment status options into ComboBox
            comboBoxPaymentStatus.Items.Clear();
            comboBoxPaymentStatus.Items.Add("Pending");
            comboBoxPaymentStatus.Items.Add("Paid");
            comboBoxPaymentStatus.Items.Add("Overdue");

            // Set the default selected item to "Pending" or whatever status is currently set
            comboBoxPaymentStatus.SelectedIndex = 0; // Default to "Pending" if no status is set

            // Load the selected rental transaction by transactionId
            var rentalTransaction = context.RentalTransactions
                                          .Where(rt => rt.RentalTransactionId == transactionId)
                                          .FirstOrDefault();

            if (rentalTransaction != null)
            {
                // Populate the form fields with the selected rental transaction's data
                txtRentalRequestId.Text = rentalTransaction.RentalRequestId.ToString();
                txtEquipmentId.Text = rentalTransaction.EquipmentId.ToString();
                txtCustomerId.Text = rentalTransaction.CustomerId.ToString();
                datePickerStartDate.Value = rentalTransaction.RentalStartDate;
                datePickerEndDate.Value = rentalTransaction.RentalEndDate;
                txtRentalFee.Text = rentalTransaction.RentalFee.ToString();
                txtDeposit.Text = rentalTransaction.Deposit.ToString();

                // Set the ComboBox selected value to match the current PaymentStatus
                comboBoxPaymentStatus.SelectedItem = rentalTransaction.PaymentStatus;
            }
            else
            {
                MessageBox.Show("Transaction not found.");
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (!int.TryParse(txtRentalRequestId.Text, out int rentalRequestId) ||
                !int.TryParse(txtEquipmentId.Text, out int equipmentId) ||
                !int.TryParse(txtCustomerId.Text, out int customerId) ||
                !decimal.TryParse(txtRentalFee.Text, out decimal rentalFee) ||
                !decimal.TryParse(txtDeposit.Text, out decimal deposit))
            {
                MessageBox.Show("Please ensure all fields are correctly filled.");
                return;
            }

            var rentalTransaction = context.RentalTransactions
                                          .Where(rt => rt.RentalTransactionId == transactionId)
                                          .FirstOrDefault();

            if (rentalTransaction != null)
            {
                // Update the transaction details
                rentalTransaction.RentalRequestId = rentalRequestId;
                rentalTransaction.EquipmentId = equipmentId;
                rentalTransaction.CustomerId = customerId;
                rentalTransaction.RentalStartDate = datePickerStartDate.Value;
                rentalTransaction.RentalEndDate = datePickerEndDate.Value;
                rentalTransaction.RentalPeriod = (datePickerEndDate.Value - datePickerStartDate.Value).Days;
                rentalTransaction.RentalFee = rentalFee;
                rentalTransaction.Deposit = deposit;
                rentalTransaction.PaymentStatus = comboBoxPaymentStatus.SelectedItem.ToString();

                try
                {
                    // Save changes to the database
                    context.SaveChanges();
                    MessageBox.Show("Rental transaction updated successfully!");

                    // Close the form
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while updating the transaction: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Transaction not found.");
            }
        }
    }
}
