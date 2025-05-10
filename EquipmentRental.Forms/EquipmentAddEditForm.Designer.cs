namespace EquipmentRental.Forms
{
    partial class EquipmentAddEditForm
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
            label3 = new Label();
            label4 = new Label();
            txtName = new TextBox();
            txtDescription = new TextBox();
            txtRentalPrice = new TextBox();
            label5 = new Label();
            label6 = new Label();
            cmbCategory = new ComboBox();
            cmbAvailability = new ComboBox();
            cmbCondition = new ComboBox();
            btnSaveEquipment = new Button();
            btnCancelEdit = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(85, 79);
            label1.Name = "label1";
            label1.Size = new Size(100, 30);
            label1.TabIndex = 0;
            label1.Text = "Equipment Name\n\n";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(85, 126);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 1;
            label2.Text = "Description";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(85, 207);
            label3.Name = "label3";
            label3.Size = new Size(58, 15);
            label3.TabIndex = 2;
            label3.Text = "Category ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(85, 167);
            label4.Name = "label4";
            label4.Size = new Size(69, 30);
            label4.TabIndex = 3;
            label4.Text = "Rental Price\n\n";
            // 
            // txtName
            // 
            txtName.Location = new Point(224, 76);
            txtName.Name = "txtName";
            txtName.Size = new Size(121, 23);
            txtName.TabIndex = 4;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(224, 118);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(121, 23);
            txtDescription.TabIndex = 6;
            // 
            // txtRentalPrice
            // 
            txtRentalPrice.Location = new Point(224, 164);
            txtRentalPrice.Name = "txtRentalPrice";
            txtRentalPrice.Size = new Size(121, 23);
            txtRentalPrice.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(85, 250);
            label5.Name = "label5";
            label5.Size = new Size(100, 15);
            label5.TabIndex = 8;
            label5.Text = "Availability Status";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(85, 297);
            label6.Name = "label6";
            label6.Size = new Size(98, 15);
            label6.TabIndex = 9;
            label6.Text = "Condition Status ";
            // 
            // cmbCategory
            // 
            cmbCategory.FormattingEnabled = true;
            cmbCategory.Location = new Point(224, 199);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(121, 23);
            cmbCategory.TabIndex = 10;
            // 
            // cmbAvailability
            // 
            cmbAvailability.FormattingEnabled = true;
            cmbAvailability.Location = new Point(224, 242);
            cmbAvailability.Name = "cmbAvailability";
            cmbAvailability.Size = new Size(121, 23);
            cmbAvailability.TabIndex = 11;
            // 
            // cmbCondition
            // 
            cmbCondition.FormattingEnabled = true;
            cmbCondition.Location = new Point(224, 289);
            cmbCondition.Name = "cmbCondition";
            cmbCondition.Size = new Size(121, 23);
            cmbCondition.TabIndex = 12;
            // 
            // btnSaveEquipment
            // 
            btnSaveEquipment.Location = new Point(110, 362);
            btnSaveEquipment.Name = "btnSaveEquipment";
            btnSaveEquipment.Size = new Size(75, 23);
            btnSaveEquipment.TabIndex = 13;
            btnSaveEquipment.Text = "Save";
            btnSaveEquipment.UseVisualStyleBackColor = true;
            btnSaveEquipment.Click += btnSaveEquipment_Click;
            // 
            // btnCancelEdit
            // 
            btnCancelEdit.Location = new Point(239, 362);
            btnCancelEdit.Name = "btnCancelEdit";
            btnCancelEdit.Size = new Size(75, 23);
            btnCancelEdit.TabIndex = 14;
            btnCancelEdit.Text = "Cancel";
            btnCancelEdit.UseVisualStyleBackColor = true;
            btnCancelEdit.Click += btnCancelEdit_Click;
            // 
            // EquipmentAddEditForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(444, 450);
            Controls.Add(btnCancelEdit);
            Controls.Add(btnSaveEquipment);
            Controls.Add(cmbCondition);
            Controls.Add(cmbAvailability);
            Controls.Add(cmbCategory);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(txtRentalPrice);
            Controls.Add(txtDescription);
            Controls.Add(txtName);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "EquipmentAddEditForm";
            Text = "EquipmentEditForm";
            Load += EquipmentAddEditForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private TextBox txtName;
        private TextBox txtDescription;
        private TextBox txtRentalPrice;
        private Label label5;
        private Label label6;
        private ComboBox cmbCategory;
        private ComboBox cmbAvailability;
        private ComboBox cmbCondition;
        private Button btnSaveEquipment;
        private Button btnCancelEdit;
    }
}