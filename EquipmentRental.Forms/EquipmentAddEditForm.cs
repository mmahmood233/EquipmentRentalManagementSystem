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
    public partial class EquipmentAddEditForm : Form
    {

        private readonly EquipmentRentalDbContext context;
        private Equipment editingEquipment; // null = add mode
        public EquipmentAddEditForm()
        {
            InitializeComponent();
            context = new EquipmentRentalDbContext();

        }

        public void LoadEquipment(Equipment equipment)
        {
            editingEquipment = equipment;

            txtName.Text = equipment.Name;
            txtDescription.Text = equipment.Description;
            txtRentalPrice.Text = equipment.RentalPrice.ToString("0.00");
            cmbAvailability.SelectedItem = equipment.AvailabilityStatus;
            cmbCondition.SelectedItem = equipment.ConditionStatus;
            cmbCategory.SelectedValue = equipment.CategoryId;
        }

        private void EquipmentAddEditForm_Load(object sender, EventArgs e)
        {
            LoadCategories();
            LoadDropdowns();

            if (editingEquipment != null)
            {
                LoadFormData(); // Only load data if editing
            }
            else
            {
                // Make sure everything is cleared
                cmbCategory.SelectedIndex = -1;
                cmbAvailability.SelectedIndex = -1;
                cmbCondition.SelectedIndex = -1;
            }
        }
        private void LoadFormData()
        {
            txtName.Text = editingEquipment.Name;
            txtDescription.Text = editingEquipment.Description;
            txtRentalPrice.Text = editingEquipment.RentalPrice.ToString("0.00");

            // Set selected category by value
            cmbCategory.SelectedValue = editingEquipment.CategoryId;

            // Set Availability and Condition by matching the string exactly
            cmbAvailability.SelectedItem = editingEquipment.AvailabilityStatus;
            cmbCondition.SelectedItem = editingEquipment.ConditionStatus;
        }
        private void LoadCategories()
        {
            var categories = context.Categories
      .Select(c => new { c.CategoryId, c.CategoryName })
      .ToList();

            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "CategoryName";
            cmbCategory.ValueMember = "CategoryId";
            cmbCategory.SelectedIndex = -1; // Clear selection
        }

        private void LoadDropdowns()
        {
            cmbAvailability.Items.AddRange(new string[]
            {
                "Available",
                "Rented",
                "Unavailable",
                "Under Maintenance"
            });

            cmbCondition.Items.AddRange(new string[]
            {
                "New",
                "Good",
                "Damaged"
            });
        }

        private void btnSaveEquipment_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            decimal price;
            if (!decimal.TryParse(txtRentalPrice.Text, out price))
            {
                MessageBox.Show("Rental price must be a number.");
                return;
            }

            if (editingEquipment == null)
            {
                // Add mode
                var newEquipment = new Equipment
                {
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    CategoryId = (int)cmbCategory.SelectedValue,
                    RentalPrice = price,
                    AvailabilityStatus = cmbAvailability.SelectedItem?.ToString(),
                    ConditionStatus = cmbCondition.SelectedItem?.ToString(),
                    CreatedAt = DateTime.Now
                };

                context.Equipment.Add(newEquipment);
            }
            else
            {
                // Edit mode
                editingEquipment.Name = txtName.Text.Trim();
                editingEquipment.Description = txtDescription.Text.Trim();
                editingEquipment.CategoryId = (int)cmbCategory.SelectedValue;
                editingEquipment.RentalPrice = price;
                editingEquipment.AvailabilityStatus = cmbAvailability.SelectedItem?.ToString();
                editingEquipment.ConditionStatus = cmbCondition.SelectedItem?.ToString();

                context.Equipment.Update(editingEquipment);
            }

            context.SaveChanges();
            MessageBox.Show("Equipment saved successfully.");
            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void btnCancelEdit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
