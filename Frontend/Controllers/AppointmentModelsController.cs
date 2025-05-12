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
    public class AppointmentModelsController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AppointmentModels
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AppointmentModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
        }

        // GET: AppointmentModels/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId");
            ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId");
            return View();
        }

        // POST: AppointmentModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,DoctorId,PatientId,DateAndTime,Finished,ProcedureId")] AppointmentModel appointmentModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointmentModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", appointmentModel.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId", appointmentModel.PatientId);
            return View(appointmentModel);
        }

        // GET: AppointmentModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.Appointments.FindAsync(id);
            if (appointmentModel == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", appointmentModel.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId", appointmentModel.PatientId);
            return View(appointmentModel);
        }

        // POST: AppointmentModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,DoctorId,PatientId,DateAndTime,Finished,ProcedureId")] AppointmentModel appointmentModel)
        {
            if (id != appointmentModel.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointmentModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentModelExists(appointmentModel.AppointmentId))
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
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", appointmentModel.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId", appointmentModel.PatientId);
            return View(appointmentModel);
        }

        // GET: AppointmentModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentModel = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointmentModel == null)
            {
                return NotFound();
            }

            return View(appointmentModel);
        }

        // POST: AppointmentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointmentModel = await _context.Appointments.FindAsync(id);
            if (appointmentModel != null)
            {
                _context.Appointments.Remove(appointmentModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentModelExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
