﻿namespace EquipmentRental.Forms
{
    partial class UserHomePage
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
            button1 = new Button();
            button2 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Black", 36F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(84, 34);
            label1.Name = "label1";
            label1.Size = new Size(597, 128);
            label1.TabIndex = 0;
            label1.Text = "Home Page";
            // 
            // button1
            // 
            button1.Location = new Point(234, 210);
            button1.Name = "button1";
            button1.Size = new Size(291, 46);
            button1.TabIndex = 1;
            button1.Text = "Rental Requests";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(234, 281);
            button2.Name = "button2";
            button2.Size = new Size(291, 46);
            button2.TabIndex = 5;
            button2.Text = "Rental Transactions";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // UserHomePage
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 659);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Name = "UserHomePage";
            Text = "UserHomePage";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button1;
        private Button button2;
    }
}