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
    public partial class DashboardForm : Form
    {
        private EquipmentRentalDbContext context;

        public DashboardForm()
        {
            InitializeComponent();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            context = new EquipmentRentalDbContext();
            LoadDashboardData();
            //SeedTestOverdueTransaction(); FOR TESTING PURPOSES DGVOVERDUERENTALS DOES WORK JUST NO DATA YET
        }

        private void LoadDashboardData()
        {
            LoadEquipmentSummary();
            LoadRentalStats();
            LoadOverdueRentals();
            LoadFinancialSummary();
        }

        private void LoadEquipmentSummary()
        {
            var summary = context.Equipment
                .Include(e => e.Category)
                .GroupBy(e => new { e.Category.CategoryName, e.AvailabilityStatus })
                .Select(g => new
                {
                    Category = g.Key.CategoryName,
                    Status = g.Key.AvailabilityStatus,
                    Count = g.Count()
                })
                .ToList();

            dgvEquipmentSummary.DataSource = summary;
        }

        private void LoadRentalStats()
        {
            var stats = context.RentalRequests
                .GroupBy(r => r.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToList();

            dgvRentalStats.DataSource = stats;
        }

        private void LoadOverdueRentals()
        {
            var today = DateTime.Now.Date;

            var overdue = context.RentalTransactions
                .Include(rt => rt.Equipment)
                .Include(rt => rt.Customer)
                .Where(rt => rt.RentalEndDate < today && rt.PaymentStatus == "Overdue")
                .Select(rt => new
                {
                    rt.RentalTransactionId,
                    Equipment = rt.Equipment.Name,
                    Customer = rt.Customer.FullName,
                    DueDate = rt.RentalEndDate,
                    DaysOverdue = EF.Functions.DateDiffDay(rt.RentalEndDate, today),
                    rt.PaymentStatus
                })
                .ToList();

            dgvOverdueRentals.DataSource = overdue;
        }

        private void LoadFinancialSummary()
        {
            // Total revenue from completed/paid rentals
            var totalRevenue = context.RentalTransactions
                .Where(rt => rt.PaymentStatus == "Paid")
                .Sum(rt => (decimal?)rt.RentalFee) ?? 0;

            lblTotalRevenue.Text = $"${totalRevenue:N2}";

            // Total overdue count
            var overdueCount = context.RentalTransactions
                .Count(rt => rt.RentalEndDate < DateTime.Now.Date && rt.PaymentStatus == "Overdue");

            lblOverdueCount.Text = overdueCount.ToString();
        }

        private void btnRefreshDashboard_Click(object sender, EventArgs e)
        {
            LoadDashboardData();
        }
        //private void SeedTestOverdueTransaction()
        //{
        //    if (!context.RentalTransactions.Any(r => r.PaymentStatus == "Overdue"))
        //    {
        //        var testRental = new RentalTransaction
        //        {
        //            RentalRequestId = 1,
        //            EquipmentId = 1,
        //            CustomerId = 1,
        //            RentalStartDate = DateTime.Now.AddDays(-10),
        //            RentalEndDate = DateTime.Now.AddDays(-5),
        //            RentalPeriod = 5,
        //            RentalFee = 100,
        //            Deposit = 50,
        //            PaymentStatus = "Overdue",
        //            CreatedAt = DateTime.Now
        //        };

        //        context.RentalTransactions.Add(testRental);
        //        context.SaveChanges();
        //    }
        //}


    }
}
