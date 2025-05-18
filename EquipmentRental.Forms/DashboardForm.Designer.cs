namespace EquipmentRental.Forms
{
    partial class DashboardForm
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
            dgvEquipmentSummary = new DataGridView();
            dgvRentalStats = new DataGridView();
            dgvOverdue = new DataGridView();
            btnRefreshDashboard = new Button();
            lblTotalRevenue = new Label();
            lblOverdueCount = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvEquipmentSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRentalStats).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvOverdue).BeginInit();
            SuspendLayout();
            // 
            // dgvEquipmentSummary
            // 
            dgvEquipmentSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEquipmentSummary.Location = new Point(12, 128);
            dgvEquipmentSummary.Name = "dgvEquipmentSummary";
            dgvEquipmentSummary.RowTemplate.Height = 25;
            dgvEquipmentSummary.Size = new Size(240, 150);
            dgvEquipmentSummary.TabIndex = 0;
            // 
            // dgvRentalStats
            // 
            dgvRentalStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRentalStats.Location = new Point(282, 128);
            dgvRentalStats.Name = "dgvRentalStats";
            dgvRentalStats.RowTemplate.Height = 25;
            dgvRentalStats.Size = new Size(240, 150);
            dgvRentalStats.TabIndex = 1;
            // 
            // dgvOverdue
            // 
            dgvOverdue.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOverdue.Location = new Point(548, 128);
            dgvOverdue.Name = "dgvOverdue";
            dgvOverdue.RowTemplate.Height = 25;
            dgvOverdue.Size = new Size(240, 150);
            dgvOverdue.TabIndex = 2;
            // 
            // btnRefreshDashboard
            // 
            btnRefreshDashboard.Location = new Point(366, 55);
            btnRefreshDashboard.Name = "btnRefreshDashboard";
            btnRefreshDashboard.Size = new Size(75, 23);
            btnRefreshDashboard.TabIndex = 3;
            btnRefreshDashboard.Text = "Refresh";
            btnRefreshDashboard.UseVisualStyleBackColor = true;
            // 
            // lblTotalRevenue
            // 
            lblTotalRevenue.AutoSize = true;
            lblTotalRevenue.Location = new Point(12, 327);
            lblTotalRevenue.Name = "lblTotalRevenue";
            lblTotalRevenue.Size = new Size(38, 15);
            lblTotalRevenue.TabIndex = 4;
            lblTotalRevenue.Text = "label1";
            // 
            // lblOverdueCount
            // 
            lblOverdueCount.AutoSize = true;
            lblOverdueCount.Location = new Point(12, 372);
            lblOverdueCount.Name = "lblOverdueCount";
            lblOverdueCount.Size = new Size(38, 15);
            lblOverdueCount.TabIndex = 5;
            lblOverdueCount.Text = "label2";
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblOverdueCount);
            Controls.Add(lblTotalRevenue);
            Controls.Add(btnRefreshDashboard);
            Controls.Add(dgvOverdue);
            Controls.Add(dgvRentalStats);
            Controls.Add(dgvEquipmentSummary);
            Name = "DashboardForm";
            Text = "DashboardForm";
            ((System.ComponentModel.ISupportInitialize)dgvEquipmentSummary).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRentalStats).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvOverdue).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvEquipmentSummary;
        private DataGridView dgvRentalStats;
        private DataGridView dgvOverdue;
        private Button btnRefreshDashboard;
        private Label lblTotalRevenue;
        private Label lblOverdueCount;
    }
}