using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Frontend.DbContext;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class ShiftModelsController : Controller
    {
        private readonly AppDbContext _context;

        public ShiftModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ShiftModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Shifts.ToListAsync());
        }

        // GET: ShiftModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftModel = await _context.Shifts
                .FirstOrDefaultAsync(m => m.ShiftID == id);
            if (shiftModel == null)
            {
                return NotFound();
            }

            return View(shiftModel);
        }

        // GET: ShiftModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShiftModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShiftID,Date,StartTime,EndTime")] ShiftModel shiftModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shiftModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shiftModel);
        }

        // GET: ShiftModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftModel = await _context.Shifts.FindAsync(id);
            if (shiftModel == null)
            {
                return NotFound();
            }
            return View(shiftModel);
        }

        // POST: ShiftModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShiftID,Date,StartTime,EndTime")] ShiftModel shiftModel)
        {
            if (id != shiftModel.ShiftID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shiftModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShiftModelExists(shiftModel.ShiftID))
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
            return View(shiftModel);
        }

        // GET: ShiftModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shiftModel = await _context.Shifts
                .FirstOrDefaultAsync(m => m.ShiftID == id);
            if (shiftModel == null)
            {
                return NotFound();
            }

            return View(shiftModel);
        }

        // POST: ShiftModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shiftModel = await _context.Shifts.FindAsync(id);
            if (shiftModel != null)
            {
                _context.Shifts.Remove(shiftModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShiftModelExists(int id)
        {
            return _context.Shifts.Any(e => e.ShiftID == id);
        }
    }
}
