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
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.Forms
{
    public partial class ReturnRecordAddEditForm : Form
    {

        private readonly EquipmentRentalDbContext context;
        public ReturnRecord editingRecord = null;


        public ReturnRecordAddEditForm()
        {
            InitializeComponent();
            context = new EquipmentRentalDbContext();

        }

        private void ReturnRecordAddEditForm_Load(object sender, EventArgs e)
        {
            LoadRentalTransactions();
            LoadConditionOptions();

            if (editingRecord == null)
            {
                cmbRentalTransaction.SelectedIndex = -1;
                cmbReturnCondition.SelectedIndex = -1;
                dtReturnDate.Value = DateTime.Now;
            }
            else
            {
                //  Safelys load values after combo boxes are ready
                cmbRentalTransaction.SelectedValue = editingRecord.RentalTransactionId;
                dtReturnDate.Value = editingRecord.ActualReturnDate;
                cmbReturnCondition.SelectedItem = editingRecord.ReturnCondition;
                txtLateFee.Text = editingRecord.LateReturnFee.ToString("0.00");
                txtAdditionalCharges.Text = editingRecord.AdditionalCharges.ToString("0.00");
            }
        }
       

        private void LoadRentalTransactions()
        {
            var transactions = context.RentalTransactions
                .Include(t => t.Equipment)
                .Include(t => t.Customer)
                .Select(t => new
                {
                    t.RentalTransactionId,
                    Label = $"{t.RentalTransactionId} - {t.Equipment.Name} ({t.Customer.FullName})"
                })
                .ToList();

            cmbRentalTransaction.DataSource = transactions;
            cmbRentalTransaction.DisplayMember = "Label";
            cmbRentalTransaction.ValueMember = "RentalTransactionId";
        }

        private void LoadConditionOptions()
        {
            cmbReturnCondition.Items.Clear();
            cmbReturnCondition.Items.AddRange(new string[]
            {
                "Good",
                "Damaged",
                "Lost"
            });
        }

        private void btnSaveReturn_Click(object sender, EventArgs e)
        {
            if (cmbRentalTransaction.SelectedIndex == -1 || cmbReturnCondition.SelectedIndex == -1)
            {
                MessageBox.Show("Please select both a rental transaction and return condition.");
                return;
            }

            if (!decimal.TryParse(txtLateFee.Text, out decimal lateFee) ||
                !decimal.TryParse(txtAdditionalCharges.Text, out decimal additional))
            {
                MessageBox.Show("Please enter valid amounts for fees.");
                return;
            }

            if (editingRecord == null)
            {
                // ADD mode
                var newRecord = new ReturnRecord
                {
                    RentalTransactionId = (int)cmbRentalTransaction.SelectedValue,
                    ActualReturnDate = dtReturnDate.Value,
                    ReturnCondition = cmbReturnCondition.SelectedItem.ToString(),
                    LateReturnFee = lateFee,
                    AdditionalCharges = additional,
                    
                };

                context.ReturnRecords.Add(newRecord);
            }
            else
            {
                // EDIT mode
                editingRecord.RentalTransactionId = (int)cmbRentalTransaction.SelectedValue;
                editingRecord.ActualReturnDate = dtReturnDate.Value;
                editingRecord.ReturnCondition = cmbReturnCondition.SelectedItem.ToString();
                editingRecord.LateReturnFee = lateFee;
                editingRecord.AdditionalCharges = additional;
                

                context.ReturnRecords.Update(editingRecord);
            }

            context.SaveChanges();
            MessageBox.Show("Return record saved.");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelReturn_Click(object sender, EventArgs e)
        {
             this.Close();
        }
    }
}

