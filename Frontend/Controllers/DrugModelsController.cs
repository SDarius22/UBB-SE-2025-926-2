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
    public class DrugModelsController : Controller
    {
        private readonly AppDbContext _context;

        public DrugModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DrugModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Drugs.ToListAsync());
        }

        // GET: DrugModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drugModel = await _context.Drugs
                .FirstOrDefaultAsync(m => m.DrugID == id);
            if (drugModel == null)
            {
                return NotFound();
            }

            return View(drugModel);
        }

        // GET: DrugModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DrugModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DrugID,Name,Administration,Specification,Supply")] DrugModel drugModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drugModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(drugModel);
        }

        // GET: DrugModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drugModel = await _context.Drugs.FindAsync(id);
            if (drugModel == null)
            {
                return NotFound();
            }
            return View(drugModel);
        }

        // POST: DrugModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DrugID,Name,Administration,Specification,Supply")] DrugModel drugModel)
        {
            if (id != drugModel.DrugID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drugModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrugModelExists(drugModel.DrugID))
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
            return View(drugModel);
        }

        // GET: DrugModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drugModel = await _context.Drugs
                .FirstOrDefaultAsync(m => m.DrugID == id);
            if (drugModel == null)
            {
                return NotFound();
            }

            return View(drugModel);
        }

        // POST: DrugModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drugModel = await _context.Drugs.FindAsync(id);
            if (drugModel != null)
            {
                _context.Drugs.Remove(drugModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrugModelExists(int id)
        {
            return _context.Drugs.Any(e => e.DrugID == id);
        }
    }
}
