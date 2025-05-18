namespace EquipmentRental.Forms
{
    partial class RentalTransactionsAdmin
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
            dataGridViewRentalTransactions = new DataGridView();
            btnFilter = new Button();
            btnSearch = new Button();
            label2 = new Label();
            comboBoxPaymentStatus = new ComboBox();
            label1 = new Label();
            txtSearch = new TextBox();
            btnCreateTransaction = new Button();
            button2 = new Button();
            Refresh = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridViewRentalTransactions).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewRentalTransactions
            // 
            dataGridViewRentalTransactions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewRentalTransactions.Location = new Point(27, 234);
            dataGridViewRentalTransactions.Name = "dataGridViewRentalTransactions";
            dataGridViewRentalTransactions.RowHeadersWidth = 82;
            dataGridViewRentalTransactions.RowTemplate.Height = 41;
            dataGridViewRentalTransactions.Size = new Size(1496, 300);
            dataGridViewRentalTransactions.TabIndex = 0;
            dataGridViewRentalTransactions.CellContentClick += dataGridViewRentalTransactions_CellContentClick;
            // 
            // btnFilter
            // 
            btnFilter.Location = new Point(422, 43);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(150, 46);
            btnFilter.TabIndex = 12;
            btnFilter.Text = "Submit";
            btnFilter.UseVisualStyleBackColor = true;
            btnFilter.Click += btnFilter_Click;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(422, 121);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(150, 46);
            btnSearch.TabIndex = 11;
            btnSearch.Text = "Submit";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(32, 47);
            label2.Name = "label2";
            label2.Size = new Size(67, 32);
            label2.TabIndex = 10;
            label2.Text = "Filter";
            // 
            // comboBoxPaymentStatus
            // 
            comboBoxPaymentStatus.FormattingEnabled = true;
            comboBoxPaymentStatus.Location = new Point(123, 47);
            comboBoxPaymentStatus.Name = "comboBoxPaymentStatus";
            comboBoxPaymentStatus.Size = new Size(267, 40);
            comboBoxPaymentStatus.TabIndex = 9;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(32, 128);
            label1.Name = "label1";
            label1.Size = new Size(85, 32);
            label1.TabIndex = 8;
            label1.Text = "Search";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(123, 128);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(267, 39);
            txtSearch.TabIndex = 7;
            // 
            // btnCreateTransaction
            // 
            btnCreateTransaction.Location = new Point(1192, 43);
            btnCreateTransaction.Name = "btnCreateTransaction";
            btnCreateTransaction.Size = new Size(299, 46);
            btnCreateTransaction.TabIndex = 13;
            btnCreateTransaction.Text = "Create Transaction ";
            btnCreateTransaction.UseVisualStyleBackColor = true;
            btnCreateTransaction.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(1192, 114);
            button2.Name = "button2";
            button2.Size = new Size(299, 46);
            button2.TabIndex = 14;
            button2.Text = "Update Transaction ";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Refresh
            // 
            Refresh.Location = new Point(1192, 182);
            Refresh.Name = "Refresh";
            Refresh.Size = new Size(299, 46);
            Refresh.TabIndex = 15;
            Refresh.Text = "Refersh";
            Refresh.UseVisualStyleBackColor = true;
            Refresh.Click += Refresh_Click;
            // 
            // RentalTransactionsAdmin
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1554, 592);
            Controls.Add(Refresh);
            Controls.Add(button2);
            Controls.Add(btnCreateTransaction);
            Controls.Add(btnFilter);
            Controls.Add(btnSearch);
            Controls.Add(label2);
            Controls.Add(comboBoxPaymentStatus);
            Controls.Add(label1);
            Controls.Add(txtSearch);
            Controls.Add(dataGridViewRentalTransactions);
            Name = "RentalTransactionsAdmin";
            Text = "RentalTransactionsAdmin";
            Load += RentalTransactionsAdmin_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewRentalTransactions).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridViewRentalTransactions;
        private Button btnFilter;
        private Button btnSearch;
        private Label label2;
        private ComboBox comboBoxPaymentStatus;
        private Label label1;
        private TextBox txtSearch;
        private Button btnCreateTransaction;
        private Button button2;
        private Button Refresh;
    }
}