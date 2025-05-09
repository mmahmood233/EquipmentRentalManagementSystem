namespace EquipmentRental.Forms
{
    partial class EquipmentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dgvEquipment = new DataGridView();
            txtSearchEquipment = new TextBox();
            btnSearchEquipment = new Button();
            btnAddEquipment = new Button();
            btnDeleteEquipment = new Button();
            btnRefreshEquipment = new Button();
            cmbCategoryFilter = new ComboBox();
            label1 = new Label();
            cmbStatusFilter = new ComboBox();
            btnEditEquipment = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvEquipment).BeginInit();
            SuspendLayout();
            // 
            // dgvEquipment
            // 
            dgvEquipment.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEquipment.Location = new Point(12, 71);
            dgvEquipment.Name = "dgvEquipment";
            dgvEquipment.RowTemplate.Height = 25;
            dgvEquipment.Size = new Size(776, 255);
            dgvEquipment.TabIndex = 0;
            // 
            // txtSearchEquipment
            // 
            txtSearchEquipment.Location = new Point(12, 28);
            txtSearchEquipment.Name = "txtSearchEquipment";
            txtSearchEquipment.Size = new Size(141, 23);
            txtSearchEquipment.TabIndex = 1;
            // 
            // btnSearchEquipment
            // 
            btnSearchEquipment.Location = new Point(174, 28);
            btnSearchEquipment.Name = "btnSearchEquipment";
            btnSearchEquipment.Size = new Size(90, 23);
            btnSearchEquipment.TabIndex = 2;
            btnSearchEquipment.Text = "Search/Filter";
            btnSearchEquipment.UseVisualStyleBackColor = true;
            // 
            // btnAddEquipment
            // 
            btnAddEquipment.Location = new Point(12, 372);
            btnAddEquipment.Name = "btnAddEquipment";
            btnAddEquipment.Size = new Size(75, 23);
            btnAddEquipment.TabIndex = 3;
            btnAddEquipment.Text = "Add";
            btnAddEquipment.UseVisualStyleBackColor = true;
            // 
            // btnDeleteEquipment
            // 
            btnDeleteEquipment.BackColor = Color.IndianRed;
            btnDeleteEquipment.Location = new Point(245, 372);
            btnDeleteEquipment.Name = "btnDeleteEquipment";
            btnDeleteEquipment.Size = new Size(75, 23);
            btnDeleteEquipment.TabIndex = 4;
            btnDeleteEquipment.Text = "Delete";
            btnDeleteEquipment.UseVisualStyleBackColor = false;
            // 
            // btnRefreshEquipment
            // 
            btnRefreshEquipment.Location = new Point(713, 27);
            btnRefreshEquipment.Name = "btnRefreshEquipment";
            btnRefreshEquipment.Size = new Size(75, 23);
            btnRefreshEquipment.TabIndex = 5;
            btnRefreshEquipment.Text = "Refresh";
            btnRefreshEquipment.UseVisualStyleBackColor = true;
            // 
            // cmbCategoryFilter
            // 
            cmbCategoryFilter.FormattingEnabled = true;
            cmbCategoryFilter.Location = new Point(354, 28);
            cmbCategoryFilter.Name = "cmbCategoryFilter";
            cmbCategoryFilter.Size = new Size(121, 23);
            cmbCategoryFilter.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(312, 32);
            label1.Name = "label1";
            label1.Size = new Size(36, 15);
            label1.TabIndex = 7;
            label1.Text = "Filter:";
            // 
            // cmbStatusFilter
            // 
            cmbStatusFilter.FormattingEnabled = true;
            cmbStatusFilter.Location = new Point(494, 29);
            cmbStatusFilter.Name = "cmbStatusFilter";
            cmbStatusFilter.Size = new Size(121, 23);
            cmbStatusFilter.TabIndex = 8;
            // 
            // btnEditEquipment
            // 
            btnEditEquipment.Location = new Point(106, 372);
            btnEditEquipment.Name = "btnEditEquipment";
            btnEditEquipment.Size = new Size(75, 23);
            btnEditEquipment.TabIndex = 9;
            btnEditEquipment.Text = "Edit";
            btnEditEquipment.UseVisualStyleBackColor = true;
            // 
            // EquipmentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnEditEquipment);
            Controls.Add(cmbStatusFilter);
            Controls.Add(label1);
            Controls.Add(cmbCategoryFilter);
            Controls.Add(btnRefreshEquipment);
            Controls.Add(btnDeleteEquipment);
            Controls.Add(btnAddEquipment);
            Controls.Add(btnSearchEquipment);
            Controls.Add(txtSearchEquipment);
            Controls.Add(dgvEquipment);
            Name = "EquipmentForm";
            Text = "EquipmentForm";
            ((System.ComponentModel.ISupportInitialize)dgvEquipment).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvEquipment;
        private TextBox txtSearchEquipment;
        private Button btnSearchEquipment;
        private Button btnAddEquipment;
        private Button btnDeleteEquipment;
        private Button btnRefreshEquipment;
        private ComboBox cmbCategoryFilter;
        private Label label1;
        private ComboBox cmbStatusFilter;
        private Button btnEditEquipment;
    }
}