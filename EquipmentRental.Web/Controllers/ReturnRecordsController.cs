using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquipmentRental.DataAccess.Models;

namespace EquipmentRental.Web.Controllers
{
    public class ReturnRecordsController : Controller
    {
        private readonly EquipmentRentalDbContext _context;

        public ReturnRecordsController(EquipmentRentalDbContext context)
        {
            _context = context;
        }

        // GET: ReturnRecords
        public async Task<IActionResult> Index()
        {
            var equipmentRentalDbContext = _context.ReturnRecords.Include(r => r.RentalTransaction);
            return View(await equipmentRentalDbContext.ToListAsync());
        }

        // GET: ReturnRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ReturnRecords == null)
            {
                return NotFound();
            }

            var returnRecord = await _context.ReturnRecords
                .Include(r => r.RentalTransaction)
                .FirstOrDefaultAsync(m => m.ReturnRecordId == id);
            if (returnRecord == null)
            {
                return NotFound();
            }

            return View(returnRecord);
        }

        // GET: ReturnRecords/Create
        public IActionResult Create()
        {
            ViewData["RentalTransactionId"] = new SelectList(_context.RentalTransactions, "RentalTransactionId", "PaymentStatus");
            return View();
        }

        // POST: ReturnRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReturnRecordId,RentalTransactionId,ActualReturnDate,ReturnCondition,LateReturnFee,AdditionalCharges,Notes")] ReturnRecord returnRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(returnRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RentalTransactionId"] = new SelectList(_context.RentalTransactions, "RentalTransactionId", "PaymentStatus", returnRecord.RentalTransactionId);
            return View(returnRecord);
        }

        // GET: ReturnRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ReturnRecords == null)
            {
                return NotFound();
            }

            var returnRecord = await _context.ReturnRecords.FindAsync(id);
            if (returnRecord == null)
            {
                return NotFound();
            }
            ViewData["RentalTransactionId"] = new SelectList(_context.RentalTransactions, "RentalTransactionId", "PaymentStatus", returnRecord.RentalTransactionId);
            return View(returnRecord);
        }

        // POST: ReturnRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReturnRecordId,RentalTransactionId,ActualReturnDate,ReturnCondition,LateReturnFee,AdditionalCharges,Notes")] ReturnRecord returnRecord)
        {
            if (id != returnRecord.ReturnRecordId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(returnRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReturnRecordExists(returnRecord.ReturnRecordId))
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
            ViewData["RentalTransactionId"] = new SelectList(_context.RentalTransactions, "RentalTransactionId", "PaymentStatus", returnRecord.RentalTransactionId);
            return View(returnRecord);
        }

        // GET: ReturnRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ReturnRecords == null)
            {
                return NotFound();
            }

            var returnRecord = await _context.ReturnRecords
                .Include(r => r.RentalTransaction)
                .FirstOrDefaultAsync(m => m.ReturnRecordId == id);
            if (returnRecord == null)
            {
                return NotFound();
            }

            return View(returnRecord);
        }

        // POST: ReturnRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ReturnRecords == null)
            {
                return Problem("Entity set 'EquipmentRentalDbContext.ReturnRecords'  is null.");
            }
            var returnRecord = await _context.ReturnRecords.FindAsync(id);
            if (returnRecord != null)
            {
                _context.ReturnRecords.Remove(returnRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnRecordExists(int id)
        {
          return (_context.ReturnRecords?.Any(e => e.ReturnRecordId == id)).GetValueOrDefault();
        }
    }
}
