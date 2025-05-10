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
            cmbStatusFilter = new ComboBox();
            btnEditEquipment = new Button();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvEquipment).BeginInit();
            SuspendLayout();
            // 
            // dgvEquipment
            // 
            dgvEquipment.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEquipment.Location = new Point(12, 146);
            dgvEquipment.Name = "dgvEquipment";
            dgvEquipment.RowTemplate.Height = 25;
            dgvEquipment.Size = new Size(776, 238);
            dgvEquipment.TabIndex = 0;
            // 
            // txtSearchEquipment
            // 
            txtSearchEquipment.Location = new Point(76, 27);
            txtSearchEquipment.Name = "txtSearchEquipment";
            txtSearchEquipment.Size = new Size(137, 23);
            txtSearchEquipment.TabIndex = 1;
            // 
            // btnSearchEquipment
            // 
            btnSearchEquipment.Location = new Point(246, 85);
            btnSearchEquipment.Name = "btnSearchEquipment";
            btnSearchEquipment.Size = new Size(107, 55);
            btnSearchEquipment.TabIndex = 2;
            btnSearchEquipment.Text = "Search/Filter";
            btnSearchEquipment.UseVisualStyleBackColor = true;
            btnSearchEquipment.Click += btnSearchEquipment_Click;
            // 
            // btnAddEquipment
            // 
            btnAddEquipment.Location = new Point(12, 406);
            btnAddEquipment.Name = "btnAddEquipment";
            btnAddEquipment.Size = new Size(75, 23);
            btnAddEquipment.TabIndex = 3;
            btnAddEquipment.Text = "Add";
            btnAddEquipment.UseVisualStyleBackColor = true;
            btnAddEquipment.Click += btnAddEquipment_Click;
            // 
            // btnDeleteEquipment
            // 
            btnDeleteEquipment.BackColor = Color.IndianRed;
            btnDeleteEquipment.Location = new Point(211, 406);
            btnDeleteEquipment.Name = "btnDeleteEquipment";
            btnDeleteEquipment.Size = new Size(75, 23);
            btnDeleteEquipment.TabIndex = 4;
            btnDeleteEquipment.Text = "Delete";
            btnDeleteEquipment.UseVisualStyleBackColor = false;
            // 
            // btnRefreshEquipment
            // 
            btnRefreshEquipment.Location = new Point(385, 85);
            btnRefreshEquipment.Name = "btnRefreshEquipment";
            btnRefreshEquipment.Size = new Size(112, 55);
            btnRefreshEquipment.TabIndex = 5;
            btnRefreshEquipment.Text = "Refresh";
            btnRefreshEquipment.UseVisualStyleBackColor = true;
            btnRefreshEquipment.Click += btnRefreshEquipment_Click;
            // 
            // cmbCategoryFilter
            // 
            cmbCategoryFilter.FormattingEnabled = true;
            cmbCategoryFilter.Location = new Point(76, 73);
            cmbCategoryFilter.Name = "cmbCategoryFilter";
            cmbCategoryFilter.Size = new Size(137, 23);
            cmbCategoryFilter.TabIndex = 6;
            // 
            // cmbStatusFilter
            // 
            cmbStatusFilter.FormattingEnabled = true;
            cmbStatusFilter.Location = new Point(76, 115);
            cmbStatusFilter.Name = "cmbStatusFilter";
            cmbStatusFilter.Size = new Size(137, 23);
            cmbStatusFilter.TabIndex = 8;
            // 
            // btnEditEquipment
            // 
            btnEditEquipment.Location = new Point(106, 406);
            btnEditEquipment.Name = "btnEditEquipment";
            btnEditEquipment.Size = new Size(75, 23);
            btnEditEquipment.TabIndex = 9;
            btnEditEquipment.Text = "Edit";
            btnEditEquipment.UseVisualStyleBackColor = true;
            btnEditEquipment.Click += btnEditEquipment_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 31);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 10;
            label2.Text = "Name:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 76);
            label3.Name = "label3";
            label3.Size = new Size(58, 15);
            label3.TabIndex = 11;
            label3.Text = "Category:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 118);
            label4.Name = "label4";
            label4.Size = new Size(42, 15);
            label4.TabIndex = 12;
            label4.Text = "Status:";
            // 
            // EquipmentForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(btnEditEquipment);
            Controls.Add(cmbStatusFilter);
            Controls.Add(cmbCategoryFilter);
            Controls.Add(btnRefreshEquipment);
            Controls.Add(btnDeleteEquipment);
            Controls.Add(btnAddEquipment);
            Controls.Add(btnSearchEquipment);
            Controls.Add(txtSearchEquipment);
            Controls.Add(dgvEquipment);
            Name = "EquipmentForm";
            Text = "EquipmentForm";
            Load += EquipmentForm_Load;
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
        private ComboBox cmbStatusFilter;
        private Button btnEditEquipment;
        private Label label2;
        private Label label3;
        private Label label4;
    }
}