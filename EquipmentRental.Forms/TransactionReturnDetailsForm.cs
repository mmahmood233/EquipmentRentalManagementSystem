using System;
using System.Windows.Forms;

namespace EquipmentRental.Forms
{
    public partial class TransactionReturnDetailsForm : Form
    {
        // Constructor accepts transaction and return details as strings
        public TransactionReturnDetailsForm(string transactionDetails, string returnDetails)
        {
            InitializeComponent();

            lblTransactionDetails.Text = transactionDetails;
            lblReturnDetails.Text = returnDetails;
        }

        private void TransactionReturnDetailsForm_Load(object sender, EventArgs e)
        {
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
