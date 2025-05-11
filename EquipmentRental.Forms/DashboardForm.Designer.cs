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
            dgvRecentReturns = new DataGridView();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvEquipmentSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRentalStats).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvOverdueRentals).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvRecentReturns).BeginInit();
            SuspendLayout();
            // 
            // dgvEquipmentSummary
            // 
            dgvEquipmentSummary.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvEquipmentSummary.Location = new Point(12, 366);
            dgvEquipmentSummary.Name = "dgvEquipmentSummary";
            dgvEquipmentSummary.RowTemplate.Height = 25;
            dgvEquipmentSummary.Size = new Size(390, 114);
            dgvEquipmentSummary.TabIndex = 0;
            // 
            // dgvRentalStats
            // 
            dgvRentalStats.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRentalStats.Location = new Point(408, 366);
            dgvRentalStats.Name = "dgvRentalStats";
            dgvRentalStats.RowTemplate.Height = 25;
            dgvRentalStats.Size = new Size(242, 114);
            dgvRentalStats.TabIndex = 1;
            // 
            // dgvOverdueRentals
            // 
            dgvOverdueRentals.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvOverdueRentals.Location = new Point(12, 205);
            dgvOverdueRentals.Name = "dgvOverdueRentals";
            dgvOverdueRentals.RowTemplate.Height = 25;
            dgvOverdueRentals.Size = new Size(638, 112);
            dgvOverdueRentals.TabIndex = 2;
            // 
            // btnRefreshDashboard
            // 
            btnRefreshDashboard.Location = new Point(557, 489);
            btnRefreshDashboard.Name = "btnRefreshDashboard";
            btnRefreshDashboard.Size = new Size(93, 34);
            btnRefreshDashboard.TabIndex = 3;
            btnRefreshDashboard.Text = "Refresh";
            btnRefreshDashboard.UseVisualStyleBackColor = true;
            // 
            // lblTotalRevenue
            // 
            lblTotalRevenue.AutoSize = true;
            lblTotalRevenue.Location = new Point(99, 508);
            lblTotalRevenue.Name = "lblTotalRevenue";
            lblTotalRevenue.Size = new Size(38, 15);
            lblTotalRevenue.TabIndex = 4;
            lblTotalRevenue.Text = "label1";
            // 
            // lblOverdueCount
            // 
            lblOverdueCount.AutoSize = true;
            lblOverdueCount.Location = new Point(274, 508);
            lblOverdueCount.Name = "lblOverdueCount";
            lblOverdueCount.Size = new Size(38, 15);
            lblOverdueCount.TabIndex = 5;
            lblOverdueCount.Text = "label2";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 508);
            label1.Name = "label1";
            label1.Size = new Size(83, 15);
            label1.TabIndex = 6;
            label1.Text = "Total Revenue:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(185, 508);
            label2.Name = "label2";
            label2.Size = new Size(91, 15);
            label2.TabIndex = 7;
            label2.Text = "Overdue Count:";
            // 
            // dgvRecentReturns
            // 
            dgvRecentReturns.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRecentReturns.Location = new Point(12, 41);
            dgvRecentReturns.Name = "dgvRecentReturns";
            dgvRecentReturns.RowTemplate.Height = 25;
            dgvRecentReturns.Size = new Size(638, 112);
            dgvRecentReturns.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 9);
            label3.Name = "label3";
            label3.Size = new Size(89, 15);
            label3.TabIndex = 9;
            label3.Text = "Recent Returns:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 176);
            label4.Name = "label4";
            label4.Size = new Size(96, 15);
            label4.TabIndex = 10;
            label4.Text = "Overdue Rentals:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 338);
            label5.Name = "label5";
            label5.Size = new Size(122, 15);
            label5.TabIndex = 11;
            label5.Text = "Equipment Summary:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(408, 338);
            label6.Name = "label6";
            label6.Size = new Size(71, 15);
            label6.TabIndex = 12;
            label6.Text = "Rental Stats:";
            // 
            // DashboardForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(683, 535);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(dgvRecentReturns);
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
            ((System.ComponentModel.ISupportInitialize)dgvRecentReturns).EndInit();
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
        private DataGridView dgvRecentReturns;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}