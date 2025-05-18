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
    public partial class CreateTransactionUser : Form
    {
        EquipmentRentalDbContext context;

        public CreateTransactionUser()
        {
            InitializeComponent();
        }

        private void CreateTransactionUser_Load(object sender, EventArgs e)
        {
            context = new EquipmentRentalDbContext();
            txtCustomerId.Text = GlobalUserInfo.UserId.ToString();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate input fields using TryParse to avoid exceptions
            if (!int.TryParse(txtRentalRequestId.Text, out int rentalRequestId))
            {
                MessageBox.Show("Please enter a valid Rental Request ID.");
                return;
            }

            if (!int.TryParse(txtEquipmentId.Text, out int equipmentId))
            {
                MessageBox.Show("Please enter a valid Equipment ID.");
                return;
            }

            if (!decimal.TryParse(txtRentalFee.Text, out decimal rentalFee))
            {
                MessageBox.Show("Please enter a valid Rental Fee.");
                return;
            }

            if (!decimal.TryParse(txtDeposit.Text, out decimal deposit))
            {
                MessageBox.Show("Please enter a valid Deposit.");
                return;
            }

            // Check if the start date is before the end date
            if (datePickerStartDate.Value >= datePickerEndDate.Value)
            {
                MessageBox.Show("The rental start date must be before the rental end date.");
                return;
            }

            // Create a new RentalTransaction object and populate fields
            var rentalTransaction = new RentalTransaction
            {
                RentalRequestId = rentalRequestId,
                EquipmentId = equipmentId,
                CustomerId = GlobalUserInfo.UserId,  // Using the global UserId for customer ID
                RentalStartDate = datePickerStartDate.Value,
                RentalEndDate = datePickerEndDate.Value,
                RentalPeriod = (datePickerEndDate.Value - datePickerStartDate.Value).Days,
                RentalFee = rentalFee,
                Deposit = deposit,
                PaymentStatus = "Pending",  // Default to Pending
                CreatedAt = DateTime.Now
            };

            try
            {
                // Add the new transaction to the database
                context.RentalTransactions.Add(rentalTransaction);
                context.SaveChanges();

                MessageBox.Show("Rental transaction created successfully!");
                this.Close(); // Close the dialog form after saving
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the transaction: " + ex.Message);
                if (ex.InnerException != null)
                {
                    MessageBox.Show("Inner Exception: " + ex.InnerException.Message);
                }
            }
        }
    }
}
