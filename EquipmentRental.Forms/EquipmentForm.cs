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
    public partial class EquipmentForm : Form
    {

        private EquipmentRentalDbContext context;

        public EquipmentForm()
        {
            InitializeComponent();
        }

        private void EquipmentForm_Load(object sender, EventArgs e)
        {
            context = new EquipmentRentalDbContext();

            LoadCategories();
            LoadStatusOptions();
            LoadEquipmentData(); // initial load
        }
        private void LoadCategories()
        {
            var categories = context.Categories
                .Select(c => new { c.CategoryId, c.CategoryName })
                .ToList();

            cmbCategoryFilter.DataSource = categories;
            cmbCategoryFilter.DisplayMember = "CategoryName";
            cmbCategoryFilter.ValueMember = "CategoryId";
            cmbCategoryFilter.SelectedIndex = -1;
        }

        private void LoadStatusOptions()
        {
            cmbStatusFilter.Items.Clear();
            cmbStatusFilter.Items.AddRange(new string[]
            {
                "Available",
                "Rented",
                "Under Maintenance",
                "Unavailable"
            });
            cmbStatusFilter.SelectedIndex = -1;
        }

        private void LoadEquipmentData(string keyword = "", int? categoryId = null, string status = "")
        {
            var query = context.Equipment
                .Include(e => e.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(e => e.Name.Contains(keyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(e => e.AvailabilityStatus == status);
            }

            var equipmentList = query.Select(e => new
            {
                e.EquipmentId,
                e.Name,
                Category = e.Category.CategoryName,
                e.Description,
                e.RentalPrice,
                e.AvailabilityStatus,
                e.ConditionStatus,
                e.CreatedAt
            }).ToList();

            dgvEquipment.DataSource = equipmentList;
        }

        private void btnSearchEquipment_Click(object sender, EventArgs e)
        {
            string keyword = txtSearchEquipment.Text.Trim();

            int? categoryId = cmbCategoryFilter.SelectedIndex != -1
                ? (int?)cmbCategoryFilter.SelectedValue
                : null;

            string status = cmbStatusFilter.SelectedItem?.ToString();

            LoadEquipmentData(keyword, categoryId, status);
        }

        private void btnRefreshEquipment_Click(object sender, EventArgs e)
        {
            txtSearchEquipment.Clear();
            cmbCategoryFilter.SelectedIndex = -1;
            cmbStatusFilter.SelectedIndex = -1;

            LoadEquipmentData(); // reset and reload all
        }

        private void btnAddEquipment_Click(object sender, EventArgs e)
        {
            EquipmentAddEditForm addForm = new EquipmentAddEditForm();

            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadEquipmentData(); // Refresh grid after adding
            }
        }

        private void btnEditEquipment_Click(object sender, EventArgs e)
        {
            if (dgvEquipment.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to edit.");
                return;
            }

            int selectedId = Convert.ToInt32(dgvEquipment.SelectedRows[0].Cells["EquipmentId"].Value);
            var equipment = context.Equipment.FirstOrDefault(e => e.EquipmentId == selectedId);

            if (equipment != null)
            {
                EquipmentAddEditForm editForm = new EquipmentAddEditForm();
                editForm.LoadEquipment(equipment); // pass existing data

                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadEquipmentData(); // Refresh after edit
                }
            }
        }

        private void btnDeleteEquipment_Click(object sender, EventArgs e)
        {
            if (dgvEquipment.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }

            // Get selected EquipmentId
            int selectedId = Convert.ToInt32(dgvEquipment.SelectedRows[0].Cells["EquipmentId"].Value);

            var equipment = context.Equipment.FirstOrDefault(e => e.EquipmentId == selectedId);

            if (equipment == null)
            {
                MessageBox.Show("Equipment not found.");
                return;
            }

            // Confirm deletion
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete '{equipment.Name}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                context.Equipment.Remove(equipment);
                context.SaveChanges();
                MessageBox.Show("Equipment deleted successfully.");

                LoadEquipmentData(); // Refresh the DataGridView
            }
        }
    }
}
