namespace EquipmentRental.Forms
{
    partial class RentalTransactionsUser
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
            Refresh = new Button();
            btnCreateTransaction = new Button();
            btnFilter = new Button();
            btnSearch = new Button();
            label2 = new Label();
            comboBoxPaymentStatus = new ComboBox();
            label1 = new Label();
            txtSearch = new TextBox();
            dataGridViewRentalTransactions = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridViewRentalTransactions).BeginInit();
            SuspendLayout();
            // 
            // Refresh
            // 
            Refresh.Location = new Point(1215, 86);
            Refresh.Name = "Refresh";
            Refresh.Size = new Size(299, 46);
            Refresh.TabIndex = 25;
            Refresh.Text = "Refersh";
            Refresh.UseVisualStyleBackColor = true;
            Refresh.Click += Refresh_Click;
            // 
            // btnCreateTransaction
            // 
            btnCreateTransaction.Location = new Point(1215, 15);
            btnCreateTransaction.Name = "btnCreateTransaction";
            btnCreateTransaction.Size = new Size(299, 46);
            btnCreateTransaction.TabIndex = 23;
            btnCreateTransaction.Text = "Create Transaction ";
            btnCreateTransaction.UseVisualStyleBackColor = true;
            btnCreateTransaction.Click += btnCreateTransaction_Click;
            // 
            // btnFilter
            // 
            btnFilter.Location = new Point(445, 15);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(150, 46);
            btnFilter.TabIndex = 22;
            btnFilter.Text = "Submit";
            btnFilter.UseVisualStyleBackColor = true;
            btnFilter.Click += btnFilter_Click;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(445, 93);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(150, 46);
            btnSearch.TabIndex = 21;
            btnSearch.Text = "Submit";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(55, 19);
            label2.Name = "label2";
            label2.Size = new Size(67, 32);
            label2.TabIndex = 20;
            label2.Text = "Filter";
            // 
            // comboBoxPaymentStatus
            // 
            comboBoxPaymentStatus.FormattingEnabled = true;
            comboBoxPaymentStatus.Location = new Point(146, 19);
            comboBoxPaymentStatus.Name = "comboBoxPaymentStatus";
            comboBoxPaymentStatus.Size = new Size(267, 40);
            comboBoxPaymentStatus.TabIndex = 19;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(55, 100);
            label1.Name = "label1";
            label1.Size = new Size(85, 32);
            label1.TabIndex = 18;
            label1.Text = "Search";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(146, 100);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(267, 39);
            txtSearch.TabIndex = 17;
            // 
            // dataGridViewRentalTransactions
            // 
            dataGridViewRentalTransactions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewRentalTransactions.Location = new Point(50, 206);
            dataGridViewRentalTransactions.Name = "dataGridViewRentalTransactions";
            dataGridViewRentalTransactions.RowHeadersWidth = 82;
            dataGridViewRentalTransactions.RowTemplate.Height = 41;
            dataGridViewRentalTransactions.Size = new Size(1496, 300);
            dataGridViewRentalTransactions.TabIndex = 16;
            // 
            // RentalTransactionsUser
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1593, 518);
            Controls.Add(Refresh);
            Controls.Add(btnCreateTransaction);
            Controls.Add(btnFilter);
            Controls.Add(btnSearch);
            Controls.Add(label2);
            Controls.Add(comboBoxPaymentStatus);
            Controls.Add(label1);
            Controls.Add(txtSearch);
            Controls.Add(dataGridViewRentalTransactions);
            Name = "RentalTransactionsUser";
            Text = "RentalTransactionsUser";
            Load += RentalTransactionsUser_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewRentalTransactions).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Refresh;
        private Button btnCreateTransaction;
        private Button btnFilter;
        private Button btnSearch;
        private Label label2;
        private ComboBox comboBoxPaymentStatus;
        private Label label1;
        private TextBox txtSearch;
        private DataGridView dataGridViewRentalTransactions;
    }
}