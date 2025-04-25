namespace EquipmentRental.Forms
{
    partial class CreateTransaction
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
            label1 = new Label();
            label2 = new Label();
            txtRentalRequestId = new TextBox();
            txtEquipmentId = new TextBox();
            label3 = new Label();
            txtCustomerId = new TextBox();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            txtRentalFee = new TextBox();
            label7 = new Label();
            txtDeposit = new TextBox();
            label8 = new Label();
            btnSave = new Button();
            datePickerStartDate = new DateTimePicker();
            datePickerEndDate = new DateTimePicker();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Black", 19.875F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(95, 62);
            label1.Name = "label1";
            label1.Size = new Size(520, 71);
            label1.TabIndex = 0;
            label1.Text = "Create Transaction";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(37, 197);
            label2.Name = "label2";
            label2.Size = new Size(209, 32);
            label2.TabIndex = 1;
            label2.Text = "Rental Request ID ";
            // 
            // txtRentalRequestId
            // 
            txtRentalRequestId.Location = new Point(252, 190);
            txtRentalRequestId.Name = "txtRentalRequestId";
            txtRentalRequestId.Size = new Size(305, 39);
            txtRentalRequestId.TabIndex = 2;
            // 
            // txtEquipmentId
            // 
            txtEquipmentId.Location = new Point(252, 235);
            txtEquipmentId.Name = "txtEquipmentId";
            txtEquipmentId.Size = new Size(305, 39);
            txtEquipmentId.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(86, 238);
            label3.Name = "label3";
            label3.Size = new Size(160, 32);
            label3.TabIndex = 3;
            label3.Text = "Equipment ID";
            // 
            // txtCustomerId
            // 
            txtCustomerId.Location = new Point(252, 280);
            txtCustomerId.Name = "txtCustomerId";
            txtCustomerId.Size = new Size(305, 39);
            txtCustomerId.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(99, 280);
            label4.Name = "label4";
            label4.Size = new Size(147, 32);
            label4.TabIndex = 5;
            label4.Text = "Customer ID";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(54, 332);
            label5.Name = "label5";
            label5.Size = new Size(192, 32);
            label5.TabIndex = 7;
            label5.Text = "Rental Start Date";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(62, 377);
            label6.Name = "label6";
            label6.Size = new Size(184, 32);
            label6.TabIndex = 9;
            label6.Text = "Rental End Date";
            // 
            // txtRentalFee
            // 
            txtRentalFee.Location = new Point(252, 415);
            txtRentalFee.Name = "txtRentalFee";
            txtRentalFee.Size = new Size(305, 39);
            txtRentalFee.TabIndex = 12;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(121, 422);
            label7.Name = "label7";
            label7.Size = new Size(125, 32);
            label7.TabIndex = 11;
            label7.Text = "Rental Fee";
            label7.TextAlign = ContentAlignment.TopCenter;
            // 
            // txtDeposit
            // 
            txtDeposit.Location = new Point(252, 460);
            txtDeposit.Name = "txtDeposit";
            txtDeposit.Size = new Size(305, 39);
            txtDeposit.TabIndex = 14;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(143, 463);
            label8.Name = "label8";
            label8.Size = new Size(103, 32);
            label8.TabIndex = 13;
            label8.Text = "Deposit ";
            // 
            // btnSave
            // 
            btnSave.Location = new Point(300, 544);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(150, 46);
            btnSave.TabIndex = 15;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // datePickerStartDate
            // 
            datePickerStartDate.Location = new Point(252, 327);
            datePickerStartDate.Name = "datePickerStartDate";
            datePickerStartDate.Size = new Size(400, 39);
            datePickerStartDate.TabIndex = 16;
            // 
            // datePickerEndDate
            // 
            datePickerEndDate.Location = new Point(252, 372);
            datePickerEndDate.Name = "datePickerEndDate";
            datePickerEndDate.Size = new Size(400, 39);
            datePickerEndDate.TabIndex = 17;
            // 
            // CreateTransaction
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(734, 677);
            Controls.Add(datePickerEndDate);
            Controls.Add(datePickerStartDate);
            Controls.Add(btnSave);
            Controls.Add(txtDeposit);
            Controls.Add(label8);
            Controls.Add(txtRentalFee);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(txtCustomerId);
            Controls.Add(label4);
            Controls.Add(txtEquipmentId);
            Controls.Add(label3);
            Controls.Add(txtRentalRequestId);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "CreateTransaction";
            Text = "CreateTransaction";
            Load += CreateTransaction_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox txtRentalRequestId;
        private TextBox txtEquipmentId;
        private Label label3;
        private TextBox txtCustomerId;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox txtRentalFee;
        private Label label7;
        private TextBox txtDeposit;
        private Label label8;
        private Button btnSave;
        private DateTimePicker datePickerStartDate;
        private DateTimePicker datePickerEndDate;
    }
}