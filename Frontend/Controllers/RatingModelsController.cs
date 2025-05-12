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
    public class RatingModelsController : Controller
    {
        private readonly AppDbContext _context;

        public RatingModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: RatingModels
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Ratings.Include(r => r.MedicalRecord);
            return View(await appDbContext.ToListAsync());
        }

        // GET: RatingModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ratingModel = await _context.Ratings
                .Include(r => r.MedicalRecord)
                .FirstOrDefaultAsync(m => m.RatingId == id);
            if (ratingModel == null)
            {
                return NotFound();
            }

            return View(ratingModel);
        }

        // GET: RatingModels/Create
        public IActionResult Create()
        {
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecords, "MedicalRecordId", "MedicalRecordId");
            return View();
        }

        // POST: RatingModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RatingId,MedicalRecordId,NumberStars,Motivation")] RatingModel ratingModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ratingModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecords, "MedicalRecordId", "MedicalRecordId", ratingModel.MedicalRecordId);
            return View(ratingModel);
        }

        // GET: RatingModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ratingModel = await _context.Ratings.FindAsync(id);
            if (ratingModel == null)
            {
                return NotFound();
            }
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecords, "MedicalRecordId", "MedicalRecordId", ratingModel.MedicalRecordId);
            return View(ratingModel);
        }

        // POST: RatingModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RatingId,MedicalRecordId,NumberStars,Motivation")] RatingModel ratingModel)
        {
            if (id != ratingModel.RatingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ratingModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RatingModelExists(ratingModel.RatingId))
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
            ViewData["MedicalRecordId"] = new SelectList(_context.MedicalRecords, "MedicalRecordId", "MedicalRecordId", ratingModel.MedicalRecordId);
            return View(ratingModel);
        }

        // GET: RatingModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ratingModel = await _context.Ratings
                .Include(r => r.MedicalRecord)
                .FirstOrDefaultAsync(m => m.RatingId == id);
            if (ratingModel == null)
            {
                return NotFound();
            }

            return View(ratingModel);
        }

        // POST: RatingModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ratingModel = await _context.Ratings.FindAsync(id);
            if (ratingModel != null)
            {
                _context.Ratings.Remove(ratingModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RatingModelExists(int id)
        {
            return _context.Ratings.Any(e => e.RatingId == id);
        }
    }
}
