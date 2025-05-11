namespace EquipmentRental.Forms
{
    partial class ReturnRecordAddEditForm
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
            cmbRentalTransaction = new ComboBox();
            cmbReturnCondition = new ComboBox();
            txtAdditionalCharges = new TextBox();
            txtLateFee = new TextBox();
            btnSaveReturn = new Button();
            btnCancelReturn = new Button();
            dtReturnDate = new DateTimePicker();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            SuspendLayout();
            // 
            // cmbRentalTransaction
            // 
            cmbRentalTransaction.FormattingEnabled = true;
            cmbRentalTransaction.Location = new Point(174, 96);
            cmbRentalTransaction.Name = "cmbRentalTransaction";
            cmbRentalTransaction.Size = new Size(200, 23);
            cmbRentalTransaction.TabIndex = 0;
            // 
            // cmbReturnCondition
            // 
            cmbReturnCondition.FormattingEnabled = true;
            cmbReturnCondition.Location = new Point(174, 191);
            cmbReturnCondition.Name = "cmbReturnCondition";
            cmbReturnCondition.Size = new Size(200, 23);
            cmbReturnCondition.TabIndex = 1;
            // 
            // txtAdditionalCharges
            // 
            txtAdditionalCharges.Location = new Point(174, 288);
            txtAdditionalCharges.Name = "txtAdditionalCharges";
            txtAdditionalCharges.Size = new Size(200, 23);
            txtAdditionalCharges.TabIndex = 2;
            // 
            // txtLateFee
            // 
            txtLateFee.Location = new Point(174, 244);
            txtLateFee.Name = "txtLateFee";
            txtLateFee.Size = new Size(200, 23);
            txtLateFee.TabIndex = 3;
            // 
            // btnSaveReturn
            // 
            btnSaveReturn.Location = new Point(33, 344);
            btnSaveReturn.Name = "btnSaveReturn";
            btnSaveReturn.Size = new Size(121, 23);
            btnSaveReturn.TabIndex = 4;
            btnSaveReturn.Text = "Save";
            btnSaveReturn.UseVisualStyleBackColor = true;
            btnSaveReturn.Click += btnSaveReturn_Click;
            // 
            // btnCancelReturn
            // 
            btnCancelReturn.Location = new Point(222, 344);
            btnCancelReturn.Name = "btnCancelReturn";
            btnCancelReturn.Size = new Size(121, 23);
            btnCancelReturn.TabIndex = 5;
            btnCancelReturn.Text = "Cancel";
            btnCancelReturn.UseVisualStyleBackColor = true;
            btnCancelReturn.Click += btnCancelReturn_Click;
            // 
            // dtReturnDate
            // 
            dtReturnDate.Location = new Point(174, 139);
            dtReturnDate.Name = "dtReturnDate";
            dtReturnDate.Size = new Size(200, 23);
            dtReturnDate.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(33, 96);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 7;
            label1.Text = "Rental:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(33, 145);
            label2.Name = "label2";
            label2.Size = new Size(69, 15);
            label2.TabIndex = 8;
            label2.Text = "ReturnDate:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(33, 194);
            label3.Name = "label3";
            label3.Size = new Size(63, 15);
            label3.TabIndex = 9;
            label3.Text = "Condition:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(33, 244);
            label4.Name = "label4";
            label4.Size = new Size(50, 15);
            label4.TabIndex = 10;
            label4.Text = "LateFee:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(33, 296);
            label5.Name = "label5";
            label5.Size = new Size(108, 15);
            label5.TabIndex = 11;
            label5.Text = "AdditionalCharges:";
            // 
            // ReturnRecordAddEditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(496, 450);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dtReturnDate);
            Controls.Add(btnCancelReturn);
            Controls.Add(btnSaveReturn);
            Controls.Add(txtLateFee);
            Controls.Add(txtAdditionalCharges);
            Controls.Add(cmbReturnCondition);
            Controls.Add(cmbRentalTransaction);
            Name = "ReturnRecordAddEditForm";
            Text = "ReturnRecordAddEditForm";
            Load += ReturnRecordAddEditForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbRentalTransaction;
        private ComboBox cmbReturnCondition;
        private TextBox txtAdditionalCharges;
        private TextBox txtLateFee;
        private Button btnSaveReturn;
        private Button btnCancelReturn;
        private DateTimePicker dtReturnDate;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}