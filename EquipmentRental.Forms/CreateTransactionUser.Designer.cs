namespace EquipmentRental.Forms
{
    partial class CreateTransactionUser
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
            datePickerEndDate = new DateTimePicker();
            datePickerStartDate = new DateTimePicker();
            btnSave = new Button();
            txtDeposit = new TextBox();
            label8 = new Label();
            txtRentalFee = new TextBox();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            txtCustomerId = new TextBox();
            label4 = new Label();
            txtEquipmentId = new TextBox();
            label3 = new Label();
            txtRentalRequestId = new TextBox();
            label2 = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // datePickerEndDate
            // 
            datePickerEndDate.Location = new Point(265, 368);
            datePickerEndDate.Name = "datePickerEndDate";
            datePickerEndDate.Size = new Size(400, 39);
            datePickerEndDate.TabIndex = 33;
            // 
            // datePickerStartDate
            // 
            datePickerStartDate.Location = new Point(265, 323);
            datePickerStartDate.Name = "datePickerStartDate";
            datePickerStartDate.Size = new Size(400, 39);
            datePickerStartDate.TabIndex = 32;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(313, 540);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(150, 46);
            btnSave.TabIndex = 31;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // txtDeposit
            // 
            txtDeposit.Location = new Point(265, 456);
            txtDeposit.Name = "txtDeposit";
            txtDeposit.Size = new Size(305, 39);
            txtDeposit.TabIndex = 30;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(156, 459);
            label8.Name = "label8";
            label8.Size = new Size(103, 32);
            label8.TabIndex = 29;
            label8.Text = "Deposit ";
            // 
            // txtRentalFee
            // 
            txtRentalFee.Location = new Point(265, 411);
            txtRentalFee.Name = "txtRentalFee";
            txtRentalFee.Size = new Size(305, 39);
            txtRentalFee.TabIndex = 28;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(134, 418);
            label7.Name = "label7";
            label7.Size = new Size(125, 32);
            label7.TabIndex = 27;
            label7.Text = "Rental Fee";
            label7.TextAlign = ContentAlignment.TopCenter;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(75, 373);
            label6.Name = "label6";
            label6.Size = new Size(184, 32);
            label6.TabIndex = 26;
            label6.Text = "Rental End Date";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(67, 328);
            label5.Name = "label5";
            label5.Size = new Size(192, 32);
            label5.TabIndex = 25;
            label5.Text = "Rental Start Date";
            // 
            // txtCustomerId
            // 
            txtCustomerId.Location = new Point(265, 276);
            txtCustomerId.Name = "txtCustomerId";
            txtCustomerId.ReadOnly = true;
            txtCustomerId.Size = new Size(305, 39);
            txtCustomerId.TabIndex = 24;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(112, 276);
            label4.Name = "label4";
            label4.Size = new Size(147, 32);
            label4.TabIndex = 23;
            label4.Text = "Customer ID";
            // 
            // txtEquipmentId
            // 
            txtEquipmentId.Location = new Point(265, 231);
            txtEquipmentId.Name = "txtEquipmentId";
            txtEquipmentId.Size = new Size(305, 39);
            txtEquipmentId.TabIndex = 22;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(99, 234);
            label3.Name = "label3";
            label3.Size = new Size(160, 32);
            label3.TabIndex = 21;
            label3.Text = "Equipment ID";
            // 
            // txtRentalRequestId
            // 
            txtRentalRequestId.Location = new Point(265, 186);
            txtRentalRequestId.Name = "txtRentalRequestId";
            txtRentalRequestId.Size = new Size(305, 39);
            txtRentalRequestId.TabIndex = 20;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(50, 193);
            label2.Name = "label2";
            label2.Size = new Size(209, 32);
            label2.TabIndex = 19;
            label2.Text = "Rental Request ID ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Black", 19.875F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(108, 58);
            label1.Name = "label1";
            label1.Size = new Size(520, 71);
            label1.TabIndex = 18;
            label1.Text = "Create Transaction";
            // 
            // CreateTransactionUser
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 694);
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
            Name = "CreateTransactionUser";
            Text = "CreateTransactionUser";
            Load += CreateTransactionUser_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DateTimePicker datePickerEndDate;
        private DateTimePicker datePickerStartDate;
        private Button btnSave;
        private TextBox txtDeposit;
        private Label label8;
        private TextBox txtRentalFee;
        private Label label7;
        private Label label6;
        private Label label5;
        private TextBox txtCustomerId;
        private Label label4;
        private TextBox txtEquipmentId;
        private Label label3;
        private TextBox txtRentalRequestId;
        private Label label2;
        private Label label1;
    }
}