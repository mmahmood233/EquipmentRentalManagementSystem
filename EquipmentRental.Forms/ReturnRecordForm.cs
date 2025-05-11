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
    public partial class ReturnRecordForm : Form
    {

        private EquipmentRentalDbContext context;

        public ReturnRecordForm()
        {
            InitializeComponent();
        }

        private void ReturnRecordForm_Load(object sender, EventArgs e)
        {
            context = new EquipmentRentalDbContext();
            LoadReturnRecords(); // initial load

        }
        private void LoadReturnRecords(string customerName = "", string equipmentName = "")
        {
            var query = context.ReturnRecords
                .Include(r => r.RentalTransaction)
                    .ThenInclude(rt => rt.Equipment)
                .Include(r => r.RentalTransaction)
                    .ThenInclude(rt => rt.Customer)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(r => r.RentalTransaction.Customer.FullName.Contains(customerName));
            }

            if (!string.IsNullOrWhiteSpace(equipmentName))
            {
                query = query.Where(r => r.RentalTransaction.Equipment.Name.Contains(equipmentName));
            }

            var data = query
                .Select(r => new
                {
                    r.ReturnRecordId,
                    Equipment = r.RentalTransaction.Equipment.Name,
                    Customer = r.RentalTransaction.Customer.FullName,
                    r.ActualReturnDate,
                    r.ReturnCondition,
                    r.LateReturnFee,
                    r.AdditionalCharges,
                    r.Notes
                })
                .OrderByDescending(r => r.ActualReturnDate)
                .ToList();

            dgvReturnRecords.DataSource = data;
        }

        private void btnSearchReturns_Click(object sender, EventArgs e)
        {
            string customer = txtSearchCustomer.Text.Trim();
            string equipment = txtSearchEquipment.Text.Trim();
            LoadReturnRecords(customer, equipment);
        }

        private void btnRefreshReturns_Click(object sender, EventArgs e)
        {
            txtSearchCustomer.Clear();
            txtSearchEquipment.Clear();
            LoadReturnRecords();
        }

        private void btnAddReturn_Click(object sender, EventArgs e)
        {
            ReturnRecordAddEditForm addForm = new ReturnRecordAddEditForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadReturnRecords(); // refresh after adding
            }
        }

        private void btnEditReturn_Click(object sender, EventArgs e)
        {
            if (dgvReturnRecords.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a return record to edit.");
                return;
            }

            int selectedId = Convert.ToInt32(dgvReturnRecords.SelectedRows[0].Cells["ReturnRecordId"].Value);

            var record = context.ReturnRecords
                .Include(r => r.RentalTransaction)
                .FirstOrDefault(r => r.ReturnRecordId == selectedId);

            if (record != null)
            {
                var editForm = new ReturnRecordAddEditForm();
                editForm.editingRecord = record; // 👈 Assign directly to the public field

                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadReturnRecords(); // Refresh after saving
                }
            }
            else
            {
                MessageBox.Show("Record not found.");
            }
        }
    }
}
