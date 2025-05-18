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
            // 
            // txtUserSearch
            // 
            txtUserSearch.Location = new Point(12, 31);
            txtUserSearch.Name = "txtUserSearch";
            txtUserSearch.Size = new Size(100, 23);
            txtUserSearch.TabIndex = 2;
            // 
            // txtActionSearch
            // 
            txtActionSearch.Location = new Point(128, 31);
            txtActionSearch.Name = "txtActionSearch";
            txtActionSearch.Size = new Size(100, 23);
            txtActionSearch.TabIndex = 3;
            // 
            // btnSearchLogs
            // 
            btnSearchLogs.Location = new Point(514, 26);
            btnSearchLogs.Name = "btnSearchLogs";
            btnSearchLogs.Size = new Size(119, 28);
            btnSearchLogs.TabIndex = 4;
            btnSearchLogs.Text = "Filter";
            btnSearchLogs.UseVisualStyleBackColor = true;
            // 
            // dtLogStart
            // 
            dtLogStart.Location = new Point(251, 31);
            dtLogStart.Name = "dtLogStart";
            dtLogStart.Size = new Size(227, 23);
            dtLogStart.TabIndex = 5;
            // 
            // cmbSourceFilter
            // 
            cmbSourceFilter.FormattingEnabled = true;
            cmbSourceFilter.Location = new Point(12, 91);
            cmbSourceFilter.Name = "cmbSourceFilter";
            cmbSourceFilter.Size = new Size(121, 23);
            cmbSourceFilter.TabIndex = 6;
            // 
            // dtLogEnd
            // 
            dtLogEnd.Location = new Point(251, 88);
            dtLogEnd.Name = "dtLogEnd";
            dtLogEnd.Size = new Size(227, 23);
            dtLogEnd.TabIndex = 7;
            // 
            // LogsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
    }
}