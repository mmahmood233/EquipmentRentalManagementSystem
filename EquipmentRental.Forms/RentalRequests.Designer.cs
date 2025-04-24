namespace EquipmentRental.Forms
{
    partial class RentalRequests
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
            txtSearch = new TextBox();
            label1 = new Label();
            comboBoxStatusFilter = new ComboBox();
            label2 = new Label();
            dataGridViewRentalRequests = new DataGridView();
            btnFilterSubmit = new Button();
            btnSearchSubmit = new Button();
            btnApprove = new Button();
            btnReject = new Button();
            button1 = new Button();
            lblTransactionDetails = new Label();
            lblReturnDetails = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridViewRentalRequests).BeginInit();
            SuspendLayout();
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(103, 109);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(267, 39);
            txtSearch.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 109);
            label1.Name = "label1";
            label1.Size = new Size(85, 32);
            label1.TabIndex = 1;
            label1.Text = "Search";
            // 
            // comboBoxStatusFilter
            // 
            comboBoxStatusFilter.FormattingEnabled = true;
            comboBoxStatusFilter.Location = new Point(103, 28);
            comboBoxStatusFilter.Name = "comboBoxStatusFilter";
            comboBoxStatusFilter.Size = new Size(267, 40);
            comboBoxStatusFilter.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 28);
            label2.Name = "label2";
            label2.Size = new Size(67, 32);
            label2.TabIndex = 3;
            label2.Text = "Filter";
            // 
            // dataGridViewRentalRequests
            // 
            dataGridViewRentalRequests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewRentalRequests.Location = new Point(45, 186);
            dataGridViewRentalRequests.Name = "dataGridViewRentalRequests";
            dataGridViewRentalRequests.RowHeadersWidth = 82;
            dataGridViewRentalRequests.RowTemplate.Height = 41;
            dataGridViewRentalRequests.Size = new Size(1754, 355);
            dataGridViewRentalRequests.TabIndex = 4;
            // 
            // btnFilterSubmit
            // 
            btnFilterSubmit.Location = new Point(402, 102);
            btnFilterSubmit.Name = "btnFilterSubmit";
            btnFilterSubmit.Size = new Size(150, 46);
            btnFilterSubmit.TabIndex = 5;
            btnFilterSubmit.Text = "Submit";
            btnFilterSubmit.UseVisualStyleBackColor = true;
            btnFilterSubmit.Click += btnFilterSubmit_Click_1;
            // 
            // btnSearchSubmit
            // 
            btnSearchSubmit.Location = new Point(402, 24);
            btnSearchSubmit.Name = "btnSearchSubmit";
            btnSearchSubmit.Size = new Size(150, 46);
            btnSearchSubmit.TabIndex = 6;
            btnSearchSubmit.Text = "Submit";
            btnSearchSubmit.UseVisualStyleBackColor = true;
            btnSearchSubmit.Click += btnSearchSubmit_Click;
            // 
            // btnApprove
            // 
            btnApprove.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnApprove.BackColor = Color.LightGreen;
            btnApprove.Font = new Font("Segoe UI Black", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnApprove.Location = new Point(1649, 41);
            btnApprove.Name = "btnApprove";
            btnApprove.Size = new Size(150, 46);
            btnApprove.TabIndex = 7;
            btnApprove.Text = "Approve";
            btnApprove.UseVisualStyleBackColor = false;
            btnApprove.Click += btnApprove_Click;
            // 
            // btnReject
            // 
            btnReject.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnReject.BackColor = Color.Crimson;
            btnReject.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            btnReject.Location = new Point(1649, 105);
            btnReject.Name = "btnReject";
            btnReject.Size = new Size(150, 46);
            btnReject.TabIndex = 8;
            btnReject.Text = "Reject";
            btnReject.UseVisualStyleBackColor = false;
            btnReject.Click += btnReject_Click;
            // 
            // button1
            // 
            button1.Location = new Point(1606, 556);
            button1.Name = "button1";
            button1.Size = new Size(193, 46);
            button1.TabIndex = 9;
            button1.Text = "View Details";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // lblTransactionDetails
            // 
            lblTransactionDetails.AutoSize = true;
            lblTransactionDetails.Location = new Point(787, 29);
            lblTransactionDetails.Name = "lblTransactionDetails";
            lblTransactionDetails.Size = new Size(0, 32);
            lblTransactionDetails.TabIndex = 10;
            // 
            // lblReturnDetails
            // 
            lblReturnDetails.AutoSize = true;
            lblReturnDetails.Location = new Point(870, 116);
            lblReturnDetails.Name = "lblReturnDetails";
            lblReturnDetails.Size = new Size(0, 32);
            lblReturnDetails.TabIndex = 11;
            // 
            // RentalRequests
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1865, 656);
            Controls.Add(lblReturnDetails);
            Controls.Add(lblTransactionDetails);
            Controls.Add(button1);
            Controls.Add(btnReject);
            Controls.Add(btnApprove);
            Controls.Add(btnSearchSubmit);
            Controls.Add(btnFilterSubmit);
            Controls.Add(dataGridViewRentalRequests);
            Controls.Add(label2);
            Controls.Add(comboBoxStatusFilter);
            Controls.Add(label1);
            Controls.Add(txtSearch);
            Name = "RentalRequests";
            Text = "RentalRequests";
            Load += RentalRequests_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewRentalRequests).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSearch;
        private Label label1;
        private ComboBox comboBoxStatusFilter;
        private Label label2;
        private DataGridView dataGridViewRentalRequests;
        private Button btnFilterSubmit;
        private Button btnSearchSubmit;
        private Button btnApprove;
        private Button btnReject;
        private Button button1;
        private Label lblTransactionDetails;
        private Label lblReturnDetails;
    }
}