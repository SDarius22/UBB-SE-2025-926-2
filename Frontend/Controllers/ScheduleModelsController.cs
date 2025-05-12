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
    public class ScheduleModelsController : Controller
    {
        private readonly AppDbContext _context;

        public ScheduleModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: ScheduleModels
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Schedules.Include(s => s.Doctor).Include(s => s.Shift);
            return View(await appDbContext.ToListAsync());
        }

        // GET: ScheduleModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleModel = await _context.Schedules
                .Include(s => s.Doctor)
                .Include(s => s.Shift)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (scheduleModel == null)
            {
                return NotFound();
            }

            return View(scheduleModel);
        }

        // GET: ScheduleModels/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId");
            ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID");
            return View();
        }

        // POST: ScheduleModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,ShiftId,ScheduleId")] ScheduleModel scheduleModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scheduleModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", scheduleModel.DoctorId);
            ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID", scheduleModel.ShiftId);
            return View(scheduleModel);
        }

        // GET: ScheduleModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleModel = await _context.Schedules.FindAsync(id);
            if (scheduleModel == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", scheduleModel.DoctorId);
            ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID", scheduleModel.ShiftId);
            return View(scheduleModel);
        }

        // POST: ScheduleModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,ShiftId,ScheduleId")] ScheduleModel scheduleModel)
        {
            if (id != scheduleModel.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scheduleModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleModelExists(scheduleModel.ScheduleId))
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
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", scheduleModel.DoctorId);
            ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID", scheduleModel.ShiftId);
            return View(scheduleModel);
        }

        // GET: ScheduleModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scheduleModel = await _context.Schedules
                .Include(s => s.Doctor)
                .Include(s => s.Shift)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (scheduleModel == null)
            {
                return NotFound();
            }

            return View(scheduleModel);
        }

        // POST: ScheduleModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scheduleModel = await _context.Schedules.FindAsync(id);
            if (scheduleModel != null)
            {
                _context.Schedules.Remove(scheduleModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleModelExists(int id)
        {
            return _context.Schedules.Any(e => e.ScheduleId == id);
        }
    }
}
