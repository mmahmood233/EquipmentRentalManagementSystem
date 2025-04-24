namespace EquipmentRental.Forms
{
    partial class TransactionReturnDetailsForm
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
            lblTransactionDetails = new Label();
            lblReturnDetails = new Label();
            label1 = new Label();
            label2 = new Label();
            btnClose = new Button();
            SuspendLayout();
            // 
            // lblTransactionDetails
            // 
            lblTransactionDetails.AutoSize = true;
            lblTransactionDetails.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblTransactionDetails.Location = new Point(32, 135);
            lblTransactionDetails.Name = "lblTransactionDetails";
            lblTransactionDetails.Size = new Size(105, 45);
            lblTransactionDetails.TabIndex = 0;
            lblTransactionDetails.Text = "label1";
            // 
            // lblReturnDetails
            // 
            lblReturnDetails.AutoSize = true;
            lblReturnDetails.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            lblReturnDetails.Location = new Point(32, 400);
            lblReturnDetails.Name = "lblReturnDetails";
            lblReturnDetails.Size = new Size(105, 45);
            lblReturnDetails.TabIndex = 1;
            lblReturnDetails.Text = "label1";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(210, 62);
            label1.Name = "label1";
            label1.Size = new Size(0, 32);
            label1.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Black", 16.125F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(162, 62);
            label2.Name = "label2";
            label2.Size = new Size(431, 59);
            label2.TabIndex = 3;
            label2.Text = "Transaction Details";
            // 
            // btnClose
            // 
            btnClose.Location = new Point(603, 614);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(150, 46);
            btnClose.TabIndex = 4;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // TransactionReturnDetailsForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 688);
            Controls.Add(btnClose);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lblReturnDetails);
            Controls.Add(lblTransactionDetails);
            Name = "TransactionReturnDetailsForm";
            Text = "TransactionReturnDetailsForm";
            Load += TransactionReturnDetailsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTransactionDetails;
        private Label lblReturnDetails;
        private Label label1;
        private Label label2;
        private Button btnClose;
    }
}