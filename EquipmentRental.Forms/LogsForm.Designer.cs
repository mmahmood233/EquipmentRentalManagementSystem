namespace EquipmentRental.Forms
{
    partial class LogsForm
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
            dgvLogs = new DataGridView();
            btnRefreshLogs = new Button();
            txtUserSearch = new TextBox();
            txtActionSearch = new TextBox();
            btnSearchLogs = new Button();
            dtLogStart = new DateTimePicker();
            cmbSourceFilter = new ComboBox();
            dtLogEnd = new DateTimePicker();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvLogs).BeginInit();
            SuspendLayout();
            // 
            // dgvLogs
            // 
            dgvLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLogs.Location = new Point(12, 134);
            dgvLogs.Name = "dgvLogs";
            dgvLogs.RowTemplate.Height = 25;
            dgvLogs.Size = new Size(776, 304);
            dgvLogs.TabIndex = 0;
            // 
            // btnRefreshLogs
            // 
            btnRefreshLogs.Location = new Point(651, 26);
            btnRefreshLogs.Name = "btnRefreshLogs";
            btnRefreshLogs.Size = new Size(119, 28);
            btnRefreshLogs.TabIndex = 1;
            btnRefreshLogs.Text = "Refresh";
            btnRefreshLogs.UseVisualStyleBackColor = true;
            btnRefreshLogs.Click += btnRefreshLogs_Click;
            // 
            // txtUserSearch
            // 
            txtUserSearch.Location = new Point(134, 26);
            txtUserSearch.Name = "txtUserSearch";
            txtUserSearch.Size = new Size(121, 23);
            txtUserSearch.TabIndex = 2;
            // 
            // txtActionSearch
            // 
            txtActionSearch.Location = new Point(134, 62);
            txtActionSearch.Name = "txtActionSearch";
            txtActionSearch.Size = new Size(121, 23);
            txtActionSearch.TabIndex = 3;
            // 
            // btnSearchLogs
            // 
            btnSearchLogs.Location = new Point(651, 86);
            btnSearchLogs.Name = "btnSearchLogs";
            btnSearchLogs.Size = new Size(119, 28);
            btnSearchLogs.TabIndex = 4;
            btnSearchLogs.Text = "Search/Filter";
            btnSearchLogs.UseVisualStyleBackColor = true;
            btnSearchLogs.Click += btnSearchLogs_Click;
            // 
            // dtLogStart
            // 
            dtLogStart.Location = new Point(373, 26);
            dtLogStart.Name = "dtLogStart";
            dtLogStart.Size = new Size(199, 23);
            dtLogStart.TabIndex = 5;
            // 
            // cmbSourceFilter
            // 
            cmbSourceFilter.FormattingEnabled = true;
            cmbSourceFilter.Location = new Point(134, 94);
            cmbSourceFilter.Name = "cmbSourceFilter";
            cmbSourceFilter.Size = new Size(121, 23);
            cmbSourceFilter.TabIndex = 6;
            // 
            // dtLogEnd
            // 
            dtLogEnd.Location = new Point(373, 94);
            dtLogEnd.Name = "dtLogEnd";
            dtLogEnd.Size = new Size(199, 23);
            dtLogEnd.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 29);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 8;
            label1.Text = "UserType:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 65);
            label2.Name = "label2";
            label2.Size = new Size(69, 15);
            label2.TabIndex = 9;
            label2.Text = "ActionType:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 97);
            label3.Name = "label3";
            label3.Size = new Size(70, 15);
            label3.TabIndex = 10;
            label3.Text = "SourceType:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(290, 29);
            label4.Name = "label4";
            label4.Size = new Size(75, 15);
            label4.TabIndex = 11;
            label4.Text = "StartingDate:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(290, 97);
            label5.Name = "label5";
            label5.Size = new Size(71, 15);
            label5.TabIndex = 12;
            label5.Text = "EndingDate:";
            // 
            // LogsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dtLogEnd);
            Controls.Add(cmbSourceFilter);
            Controls.Add(dtLogStart);
            Controls.Add(btnSearchLogs);
            Controls.Add(txtActionSearch);
            Controls.Add(txtUserSearch);
            Controls.Add(btnRefreshLogs);
            Controls.Add(dgvLogs);
            Name = "LogsForm";
            Text = "LogsForm";
            Load += LogsForm_Load;
            ((System.ComponentModel.ISupportInitialize)dgvLogs).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvLogs;
        private Button btnRefreshLogs;
        private TextBox txtUserSearch;
        private TextBox txtActionSearch;
        private Button btnSearchLogs;
        private DateTimePicker dtLogStart;
        private ComboBox cmbSourceFilter;
        private DateTimePicker dtLogEnd;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}