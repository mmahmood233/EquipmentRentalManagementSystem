using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
namespace EquipmentRental.Web.Controllers
{
    [Authorize(Roles = "Customer,Manager,Administrator")]
    public class RentalTransactionsController : Controller

    {
        private readonly EquipmentRentalDbContext _context;

        public RentalTransactionsController(EquipmentRentalDbContext context)
        {
            _context = context;
        }

        // GET: RentalTransactions
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity.Name;
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized();

            IQueryable<RentalTransaction> query = _context.RentalTransactions
                .Include(r => r.Customer)
                .Include(r => r.Equipment)
                .Include(r => r.RentalRequest);

            if (user.Role.RoleName == "Customer")
            {
                query = query.Where(r => r.Customer.Email == userEmail);
            }

            return View(await query.ToListAsync());
        }


        // GET: RentalTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RentalTransactions == null)
            {
                return NotFound();
            }

            var rentalTransaction = await _context.RentalTransactions
                .Include(r => r.Customer)
                .Include(r => r.Equipment)
                .Include(r => r.RentalRequest)
                .FirstOrDefaultAsync(m => m.RentalTransactionId == id);
            if (rentalTransaction == null)
            {
                return NotFound();
            }

            return View(rentalTransaction);
        }

        // GET: RentalTransactions/Create
        [Authorize(Roles = "Manager,Administrator")]
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Users, "UserId", "Email");
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "EquipmentId", "AvailabilityStatus");
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests, "RentalRequestId", "Status");
            return View();
        }

        // POST: RentalTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalTransactionId,RentalRequestId,EquipmentId,CustomerId,RentalStartDate,RentalEndDate,RentalPeriod,RentalFee,Deposit,PaymentStatus,CreatedAt")] RentalTransaction rentalTransaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rentalTransaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Users, "UserId", "Email", rentalTransaction.CustomerId);
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "EquipmentId", "AvailabilityStatus", rentalTransaction.EquipmentId);
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests, "RentalRequestId", "Status", rentalTransaction.RentalRequestId);
            return View(rentalTransaction);
        }
        [Authorize(Roles = "Manager,Administrator")]

        // GET: RentalTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RentalTransactions == null)
            {
                return NotFound();
            }

            var rentalTransaction = await _context.RentalTransactions.FindAsync(id);
            if (rentalTransaction == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Users, "UserId", "Email", rentalTransaction.CustomerId);
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "EquipmentId", "AvailabilityStatus", rentalTransaction.EquipmentId);
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests, "RentalRequestId", "Status", rentalTransaction.RentalRequestId);
            return View(rentalTransaction);
        }

        // POST: RentalTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalTransactionId,RentalRequestId,EquipmentId,CustomerId,RentalStartDate,RentalEndDate,RentalPeriod,RentalFee,Deposit,PaymentStatus,CreatedAt")] RentalTransaction rentalTransaction)
        {
            if (id != rentalTransaction.RentalTransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rentalTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalTransactionExists(rentalTransaction.RentalTransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Users, "UserId", "Email", rentalTransaction.CustomerId);
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "EquipmentId", "AvailabilityStatus", rentalTransaction.EquipmentId);
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests, "RentalRequestId", "Status", rentalTransaction.RentalRequestId);
            return View(rentalTransaction);
        }
        [Authorize(Roles = "Manager,Administrator")]

        // GET: RentalTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RentalTransactions == null)
            {
                return NotFound();
            }

            var rentalTransaction = await _context.RentalTransactions
                .Include(r => r.Customer)
                .Include(r => r.Equipment)
                .Include(r => r.RentalRequest)
                .FirstOrDefaultAsync(m => m.RentalTransactionId == id);
            if (rentalTransaction == null)
            {
                return NotFound();
            }

            return View(rentalTransaction);
        }

        // POST: RentalTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RentalTransactions == null)
            {
                return Problem("Entity set 'EquipmentRentalDbContext.RentalTransactions'  is null.");
            }
            var rentalTransaction = await _context.RentalTransactions.FindAsync(id);
            if (rentalTransaction != null)
            {
                _context.RentalTransactions.Remove(rentalTransaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalTransactionExists(int id)
        {
          return (_context.RentalTransactions?.Any(e => e.RentalTransactionId == id)).GetValueOrDefault();
        }
    }
}
