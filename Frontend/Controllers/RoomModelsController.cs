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
    public class RoomModelsController : Controller
    {
        private readonly AppDbContext _context;

        public RoomModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: RoomModels
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Rooms.Include(r => r.Department).Include(r => r.Equipment);
            return View(await appDbContext.ToListAsync());
        }

        // GET: RoomModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomModel = await _context.Rooms
                .Include(r => r.Department)
                .Include(r => r.Equipment)
                .FirstOrDefaultAsync(m => m.RoomID == id);
            if (roomModel == null)
            {
                return NotFound();
            }

            return View(roomModel);
        }

        // GET: RoomModels/Create
        public IActionResult Create()
        {
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
            ViewData["EquipmentID"] = new SelectList(_context.Equipments, "EquipmentID", "EquipmentID");
            return View();
        }

        // POST: RoomModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomID,Capacity,DepartmentID,EquipmentID")] RoomModel roomModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roomModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", roomModel.DepartmentID);
            ViewData["EquipmentID"] = new SelectList(_context.Equipments, "EquipmentID", "EquipmentID", roomModel.EquipmentID);
            return View(roomModel);
        }

        // GET: RoomModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomModel = await _context.Rooms.FindAsync(id);
            if (roomModel == null)
            {
                return NotFound();
            }
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", roomModel.DepartmentID);
            ViewData["EquipmentID"] = new SelectList(_context.Equipments, "EquipmentID", "EquipmentID", roomModel.EquipmentID);
            return View(roomModel);
        }

        // POST: RoomModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomID,Capacity,DepartmentID,EquipmentID")] RoomModel roomModel)
        {
            if (id != roomModel.RoomID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomModelExists(roomModel.RoomID))
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
            ViewData["DepartmentID"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", roomModel.DepartmentID);
            ViewData["EquipmentID"] = new SelectList(_context.Equipments, "EquipmentID", "EquipmentID", roomModel.EquipmentID);
            return View(roomModel);
        }

        // GET: RoomModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomModel = await _context.Rooms
                .Include(r => r.Department)
                .Include(r => r.Equipment)
                .FirstOrDefaultAsync(m => m.RoomID == id);
            if (roomModel == null)
            {
                return NotFound();
            }

            return View(roomModel);
        }

        // POST: RoomModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomModel = await _context.Rooms.FindAsync(id);
            if (roomModel != null)
            {
                _context.Rooms.Remove(roomModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomModelExists(int id)
        {
            return _context.Rooms.Any(e => e.RoomID == id);
        }
    }
}
