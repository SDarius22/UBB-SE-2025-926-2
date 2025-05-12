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
    public class ProcedureModelsController : Controller
    {
        private readonly AppDbContext _context;

        public ProcedureModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ProcedureModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Procedures.ToListAsync());
        }

        // GET: ProcedureModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedureModel = await _context.Procedures
                .FirstOrDefaultAsync(m => m.ProcedureId == id);
            if (procedureModel == null)
            {
                return NotFound();
            }

            return View(procedureModel);
        }

        // GET: ProcedureModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProcedureModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProcedureId,ProcedureName,DepartmentId,ProcedureDuration")] ProcedureModel procedureModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(procedureModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(procedureModel);
        }

        // GET: ProcedureModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedureModel = await _context.Procedures.FindAsync(id);
            if (procedureModel == null)
            {
                return NotFound();
            }
            return View(procedureModel);
        }

        // POST: ProcedureModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProcedureId,ProcedureName,DepartmentId,ProcedureDuration")] ProcedureModel procedureModel)
        {
            if (id != procedureModel.ProcedureId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(procedureModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProcedureModelExists(procedureModel.ProcedureId))
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
            return View(procedureModel);
        }

        // GET: ProcedureModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var procedureModel = await _context.Procedures
                .FirstOrDefaultAsync(m => m.ProcedureId == id);
            if (procedureModel == null)
            {
                return NotFound();
            }

            return View(procedureModel);
        }

        // POST: ProcedureModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var procedureModel = await _context.Procedures.FindAsync(id);
            if (procedureModel != null)
            {
                _context.Procedures.Remove(procedureModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProcedureModelExists(int id)
        {
            return _context.Procedures.Any(e => e.ProcedureId == id);
        }
    }
}
