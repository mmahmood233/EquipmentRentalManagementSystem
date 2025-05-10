using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EquipmentRental.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.Forms
{
    public partial class LogsForm : Form
    {
        private EquipmentRentalDbContext context;

        public LogsForm()
        {
            InitializeComponent();
        }

        private void LogsForm_Load(object sender, EventArgs e)
        {
            context = new EquipmentRentalDbContext();

            // Set default date range to the past 7 days
            dtLogStart.Value = DateTime.Now.Date.AddDays(-7);
            dtLogEnd.Value = DateTime.Now.Date;

            cmbSourceFilter.Items.AddRange(new string[] { "Web", "Desktop" });
            cmbSourceFilter.SelectedIndex = -1;

            LoadLogs(); // Initial load
        }

        private void LoadLogs(string userKeyword = "", string actionKeyword = "", string source = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = context.Logs
                .Include(l => l.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(userKeyword))
            {
                query = query.Where(l => l.User != null && l.User.FullName.Contains(userKeyword));
            }

            if (!string.IsNullOrWhiteSpace(actionKeyword))
            {
                query = query.Where(l => l.Action.Contains(actionKeyword));
            }

            if (!string.IsNullOrWhiteSpace(source))
            {
                query = query.Where(l => l.Source == source);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= startDate.Value && l.Timestamp <= endDate.Value);
            }

            var logs = query
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new
                {
                    l.LogId,
                    Timestamp = l.Timestamp.ToString("g"),
                    User = l.User != null ? l.User.FullName : "System",
                    l.Action,
                    l.Source,
                    l.Exception
                })
                .ToList();

            dgvLogs.DataSource = logs;
        }

        private void btnSearchLogs_Click(object sender, EventArgs e)
        {
            string userSearch = txtUserSearch.Text.Trim();
            string actionSearch = txtActionSearch.Text.Trim();
            string sourceFilter = cmbSourceFilter.SelectedItem?.ToString();
            DateTime start = dtLogStart.Value.Date;
            DateTime end = dtLogEnd.Value.Date.AddDays(1).AddSeconds(-1); // include full day

            LoadLogs(userSearch, actionSearch, sourceFilter, start, end);
        }

        private void btnRefreshLogs_Click(object sender, EventArgs e)
        {
            txtUserSearch.Clear();
            txtActionSearch.Clear();
            cmbSourceFilter.SelectedIndex = -1;

            dtLogStart.Value = DateTime.Now.Date.AddDays(-7);
            dtLogEnd.Value = DateTime.Now.Date;

            LoadLogs();
        }
    }
}
