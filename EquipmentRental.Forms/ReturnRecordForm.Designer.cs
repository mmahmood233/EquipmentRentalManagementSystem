namespace EquipmentRental.Forms
{
    partial class ReturnRecordForm
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
            dgvReturnRecords = new DataGridView();
            txtSearchCustomer = new TextBox();
            label1 = new Label();
            txtSearchEquipment = new TextBox();
            btnSearchReturns = new Button();
            label2 = new Label();
            btnRefreshReturns = new Button();
            btnAddReturn = new Button();
            btnEditReturn = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvReturnRecords).BeginInit();
            SuspendLayout();
            // 
            // dgvReturnRecords
            // 
            dgvReturnRecords.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReturnRecords.Location = new Point(12, 131);
            dgvReturnRecords.Name = "dgvReturnRecords";
            dgvReturnRecords.RowTemplate.Height = 25;
            dgvReturnRecords.Size = new Size(776, 229);
            dgvReturnRecords.TabIndex = 0;
            // 
            // txtSearchCustomer
            // 
            txtSearchCustomer.Location = new Point(94, 46);
            txtSearchCustomer.Name = "txtSearchCustomer";
            txtSearchCustomer.Size = new Size(100, 23);
            txtSearchCustomer.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 46);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 2;
            label1.Text = "Customer:";
            // 
            // txtSearchEquipment
            // 
            txtSearchEquipment.Location = new Point(94, 93);
            txtSearchEquipment.Name = "txtSearchEquipment";
            txtSearchEquipment.Size = new Size(100, 23);
            txtSearchEquipment.TabIndex = 3;
            // 
            // btnSearchReturns
            // 
            btnSearchReturns.Location = new Point(254, 92);
            btnSearchReturns.Name = "btnSearchReturns";
            btnSearchReturns.Size = new Size(105, 23);
            btnSearchReturns.TabIndex = 4;
            btnSearchReturns.Text = "Search/Filter";
            btnSearchReturns.UseVisualStyleBackColor = true;
            btnSearchReturns.Click += btnSearchReturns_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 93);
            label2.Name = "label2";
            label2.Size = new Size(68, 15);
            label2.TabIndex = 5;
            label2.Text = "Equipment:";
            // 
            // btnRefreshReturns
            // 
            btnRefreshReturns.Location = new Point(383, 92);
            btnRefreshReturns.Name = "btnRefreshReturns";
            btnRefreshReturns.Size = new Size(105, 23);
            btnRefreshReturns.TabIndex = 6;
            btnRefreshReturns.Text = "Refresh";
            btnRefreshReturns.UseVisualStyleBackColor = true;
            btnRefreshReturns.Click += btnRefreshReturns_Click;
            // 
            // btnAddReturn
            // 
            btnAddReturn.Location = new Point(12, 385);
            btnAddReturn.Name = "btnAddReturn";
            btnAddReturn.Size = new Size(105, 23);
            btnAddReturn.TabIndex = 7;
            btnAddReturn.Text = "Add";
            btnAddReturn.UseVisualStyleBackColor = true;
            btnAddReturn.Click += btnAddReturn_Click;
            // 
            // btnEditReturn
            // 
            btnEditReturn.Location = new Point(142, 385);
            btnEditReturn.Name = "btnEditReturn";
            btnEditReturn.Size = new Size(105, 23);
            btnEditReturn.TabIndex = 8;
            btnEditReturn.Text = "Edit";
            btnEditReturn.UseVisualStyleBackColor = true;
            btnEditReturn.Click += btnEditReturn_Click;
            // 
            // ReturnRecordForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btnEditReturn);
            Controls.Add(btnAddReturn);
            Controls.Add(btnRefreshReturns);
            Controls.Add(label2);
            Controls.Add(btnSearchReturns);
            Controls.Add(txtSearchEquipment);
            Controls.Add(label1);
            Controls.Add(txtSearchCustomer);
            Controls.Add(dgvReturnRecords);
            Name = "ReturnRecordForm";
            Text = "ReturnRecordForm";
            Load += ReturnRecordForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvReturnRecords).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvReturnRecords;
        private TextBox txtSearchCustomer;
        private Label label1;
        private TextBox txtSearchEquipment;
        private Button btnSearchReturns;
        private Label label2;
        private Button btnRefreshReturns;
        private Button btnAddReturn;
        private Button btnEditReturn;
    }
}