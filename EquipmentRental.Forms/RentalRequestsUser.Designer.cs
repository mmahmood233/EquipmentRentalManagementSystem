namespace EquipmentRental.Forms
{
    partial class RentalRequestsUser
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
            button1 = new Button();
            btnSearchSubmit = new Button();
            btnFilterSubmit = new Button();
            dataGridViewRentalRequests = new DataGridView();
            label2 = new Label();
            comboBoxStatusFilter = new ComboBox();
            label1 = new Label();
            txtSearch = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewRentalRequests).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(1587, 578);
            button1.Name = "button1";
            button1.Size = new Size(193, 46);
            button1.TabIndex = 21;
            button1.Text = "View Details";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // btnSearchSubmit
            // 
            btnSearchSubmit.Location = new Point(429, 129);
            btnSearchSubmit.Name = "btnSearchSubmit";
            btnSearchSubmit.Size = new Size(150, 46);
            btnSearchSubmit.TabIndex = 18;
            btnSearchSubmit.Text = "Submit";
            btnSearchSubmit.UseVisualStyleBackColor = true;
            btnSearchSubmit.Click += btnSearchSubmit_Click;
            // 
            // btnFilterSubmit
            // 
            btnFilterSubmit.Location = new Point(429, 48);
            btnFilterSubmit.Name = "btnFilterSubmit";
            btnFilterSubmit.Size = new Size(150, 46);
            btnFilterSubmit.TabIndex = 17;
            btnFilterSubmit.Text = "Submit";
            btnFilterSubmit.UseVisualStyleBackColor = true;
            btnFilterSubmit.Click += btnFilterSubmit_Click;
            // 
            // dataGridViewRentalRequests
            // 
            dataGridViewRentalRequests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewRentalRequests.Location = new Point(68, 206);
            dataGridViewRentalRequests.Name = "dataGridViewRentalRequests";
            dataGridViewRentalRequests.RowHeadersWidth = 82;
            dataGridViewRentalRequests.RowTemplate.Height = 41;
            dataGridViewRentalRequests.Size = new Size(1754, 355);
            dataGridViewRentalRequests.TabIndex = 16;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(35, 48);
            label2.Name = "label2";
            label2.Size = new Size(67, 32);
            label2.TabIndex = 15;
            label2.Text = "Filter";
            // 
            // comboBoxStatusFilter
            // 
            comboBoxStatusFilter.FormattingEnabled = true;
            comboBoxStatusFilter.Location = new Point(126, 48);
            comboBoxStatusFilter.Name = "comboBoxStatusFilter";
            comboBoxStatusFilter.Size = new Size(267, 40);
            comboBoxStatusFilter.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(35, 129);
            label1.Name = "label1";
            label1.Size = new Size(85, 32);
            label1.TabIndex = 13;
            label1.Text = "Search";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(126, 129);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(267, 39);
            txtSearch.TabIndex = 12;
            // 
            // RentalRequestsUser
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1868, 646);
            Controls.Add(button1);
            Controls.Add(btnSearchSubmit);
            Controls.Add(btnFilterSubmit);
            Controls.Add(dataGridViewRentalRequests);
            Controls.Add(label2);
            Controls.Add(comboBoxStatusFilter);
            Controls.Add(label1);
            Controls.Add(txtSearch);
            Name = "RentalRequestsUser";
            Text = "RentalRequestsUser";
            Load += RentalRequestsUser_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewRentalRequests).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button1;
        private Button btnSearchSubmit;
        private Button btnFilterSubmit;
        private DataGridView dataGridViewRentalRequests;
        private Label label2;
        private ComboBox comboBoxStatusFilter;
        private Label label1;
        private TextBox txtSearch;
    }
}