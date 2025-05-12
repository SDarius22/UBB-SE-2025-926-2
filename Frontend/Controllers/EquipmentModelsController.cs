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
    public class EquipmentModelsController : Controller
    {
        private readonly AppDbContext _context;

        public EquipmentModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: EquipmentModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Equipments.ToListAsync());
        }

        // GET: EquipmentModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentModel = await _context.Equipments
                .FirstOrDefaultAsync(m => m.EquipmentID == id);
            if (equipmentModel == null)
            {
                return NotFound();
            }

            return View(equipmentModel);
        }

        // GET: EquipmentModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EquipmentModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipmentID,Name,Type,Specification,Stock")] EquipmentModel equipmentModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipmentModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(equipmentModel);
        }

        // GET: EquipmentModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentModel = await _context.Equipments.FindAsync(id);
            if (equipmentModel == null)
            {
                return NotFound();
            }
            return View(equipmentModel);
        }

        // POST: EquipmentModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipmentID,Name,Type,Specification,Stock")] EquipmentModel equipmentModel)
        {
            if (id != equipmentModel.EquipmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipmentModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentModelExists(equipmentModel.EquipmentID))
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
            return View(equipmentModel);
        }

        // GET: EquipmentModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipmentModel = await _context.Equipments
                .FirstOrDefaultAsync(m => m.EquipmentID == id);
            if (equipmentModel == null)
            {
                return NotFound();
            }

            return View(equipmentModel);
        }

        // POST: EquipmentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipmentModel = await _context.Equipments.FindAsync(id);
            if (equipmentModel != null)
            {
                _context.Equipments.Remove(equipmentModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentModelExists(int id)
        {
            return _context.Equipments.Any(e => e.EquipmentID == id);
        }
    }
}
