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
    public class DepartmentModelsController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DepartmentModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Departments.ToListAsync());
        }

        // GET: DepartmentModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentModel = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (departmentModel == null)
            {
                return NotFound();
            }

            return View(departmentModel);
        }

        // GET: DepartmentModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DepartmentModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,Name")] DepartmentModel departmentModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(departmentModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(departmentModel);
        }

        // GET: DepartmentModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentModel = await _context.Departments.FindAsync(id);
            if (departmentModel == null)
            {
                return NotFound();
            }
            return View(departmentModel);
        }

        // POST: DepartmentModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,Name")] DepartmentModel departmentModel)
        {
            if (id != departmentModel.DepartmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(departmentModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentModelExists(departmentModel.DepartmentID))
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
            return View(departmentModel);
        }

        // GET: DepartmentModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentModel = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentID == id);
            if (departmentModel == null)
            {
                return NotFound();
            }

            return View(departmentModel);
        }

        // POST: DepartmentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departmentModel = await _context.Departments.FindAsync(id);
            if (departmentModel != null)
            {
                _context.Departments.Remove(departmentModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentModelExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }
    }
}
