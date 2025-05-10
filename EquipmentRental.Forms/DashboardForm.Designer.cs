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
            dgvOverdueRentals = new DataGridView();
            btnRefreshDashboard = new Button();
            lblTotalRevenue = new Label();
            lblOverdueCount = new Label();
            label1 = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvEquipmentSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRentalStats).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvOverdueRentals).BeginInit();
            SuspendLayout();
            // 
            // dgvEquipmentSummary
            // 
            dgvEquipmentSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEquipmentSummary.Location = new Point(12, 266);
            dgvEquipmentSummary.Name = "dgvEquipmentSummary";
            dgvEquipmentSummary.RowTemplate.Height = 25;
            dgvEquipmentSummary.Size = new Size(776, 114);
            dgvEquipmentSummary.TabIndex = 0;
            // 
            // dgvRentalStats
            // 
            dgvRentalStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRentalStats.Location = new Point(12, 25);
            dgvRentalStats.Name = "dgvRentalStats";
            dgvRentalStats.RowTemplate.Height = 25;
            dgvRentalStats.Size = new Size(776, 117);
            dgvRentalStats.TabIndex = 1;
            // 
            // dgvOverdueRentals
            // 
            dgvOverdueRentals.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOverdueRentals.Location = new Point(12, 148);
            dgvOverdueRentals.Name = "dgvOverdueRentals";
            dgvOverdueRentals.RowTemplate.Height = 25;
            dgvOverdueRentals.Size = new Size(776, 112);
            dgvOverdueRentals.TabIndex = 2;
            // 
            // btnRefreshDashboard
            // 
            btnRefreshDashboard.Location = new Point(695, 395);
            btnRefreshDashboard.Name = "btnRefreshDashboard";
            btnRefreshDashboard.Size = new Size(93, 34);
            btnRefreshDashboard.TabIndex = 3;
            btnRefreshDashboard.Text = "Refresh";
            btnRefreshDashboard.UseVisualStyleBackColor = true;
            // 
            // lblTotalRevenue
            // 
            lblTotalRevenue.AutoSize = true;
            lblTotalRevenue.Location = new Point(101, 405);
            lblTotalRevenue.Name = "lblTotalRevenue";
            lblTotalRevenue.Size = new Size(38, 15);
            lblTotalRevenue.TabIndex = 4;
            lblTotalRevenue.Text = "label1";
            // 
            // lblOverdueCount
            // 
            lblOverdueCount.AutoSize = true;
            lblOverdueCount.Location = new Point(276, 405);
            lblOverdueCount.Name = "lblOverdueCount";
            lblOverdueCount.Size = new Size(38, 15);
            lblOverdueCount.TabIndex = 5;
            lblOverdueCount.Text = "label2";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 405);
            label1.Name = "label1";
            label1.Size = new Size(83, 15);
            label1.TabIndex = 6;
            label1.Text = "Total Revenue:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(187, 405);
            label2.Name = "label2";
            label2.Size = new Size(91, 15);
            label2.TabIndex = 7;
            label2.Text = "Overdue Count:";
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lblOverdueCount);
            Controls.Add(lblTotalRevenue);
            Controls.Add(btnRefreshDashboard);
            Controls.Add(dgvOverdueRentals);
            Controls.Add(dgvRentalStats);
            Controls.Add(dgvEquipmentSummary);
            Name = "DashboardForm";
            Text = "DashboardForm";
            Load += DashboardForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvEquipmentSummary).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvRentalStats).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvOverdueRentals).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvEquipmentSummary;
        private DataGridView dgvRentalStats;
        private DataGridView dgvOverdueRentals;
        private Button btnRefreshDashboard;
        private Label lblTotalRevenue;
        private Label lblOverdueCount;
        private Label label1;
        private Label label2;
    }
}