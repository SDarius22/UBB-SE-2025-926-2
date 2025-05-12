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
    public class MedicalRecordModelsController : Controller
    {
        private readonly AppDbContext _context;

        public MedicalRecordModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MedicalRecordModels
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.MedicalRecords.Include(m => m.Doctor).Include(m => m.Patient).Include(m => m.Procedure);
            return View(await appDbContext.ToListAsync());
        }

        // GET: MedicalRecordModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecordModel = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .Include(m => m.Procedure)
                .FirstOrDefaultAsync(m => m.MedicalRecordId == id);
            if (medicalRecordModel == null)
            {
                return NotFound();
            }

            return View(medicalRecordModel);
        }

        // GET: MedicalRecordModels/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId");
            ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId");
            ViewData["ProcedureId"] = new SelectList(_context.Procedures, "ProcedureId", "ProcedureId");
            return View();
        }

        // POST: MedicalRecordModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicalRecordId,PatientId,DoctorId,DateAndTime,ProcedureId,Conclusion")] MedicalRecordModel medicalRecordModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicalRecordModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", medicalRecordModel.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId", medicalRecordModel.PatientId);
            ViewData["ProcedureId"] = new SelectList(_context.Procedures, "ProcedureId", "ProcedureId", medicalRecordModel.ProcedureId);
            return View(medicalRecordModel);
        }

        // GET: MedicalRecordModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecordModel = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecordModel == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", medicalRecordModel.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId", medicalRecordModel.PatientId);
            ViewData["ProcedureId"] = new SelectList(_context.Procedures, "ProcedureId", "ProcedureId", medicalRecordModel.ProcedureId);
            return View(medicalRecordModel);
        }

        // POST: MedicalRecordModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicalRecordId,PatientId,DoctorId,DateAndTime,ProcedureId,Conclusion")] MedicalRecordModel medicalRecordModel)
        {
            if (id != medicalRecordModel.MedicalRecordId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalRecordModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalRecordModelExists(medicalRecordModel.MedicalRecordId))
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
            ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", medicalRecordModel.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId", medicalRecordModel.PatientId);
            ViewData["ProcedureId"] = new SelectList(_context.Procedures, "ProcedureId", "ProcedureId", medicalRecordModel.ProcedureId);
            return View(medicalRecordModel);
        }

        // GET: MedicalRecordModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecordModel = await _context.MedicalRecords
                .Include(m => m.Doctor)
                .Include(m => m.Patient)
                .Include(m => m.Procedure)
                .FirstOrDefaultAsync(m => m.MedicalRecordId == id);
            if (medicalRecordModel == null)
            {
                return NotFound();
            }

            return View(medicalRecordModel);
        }

        // POST: MedicalRecordModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalRecordModel = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecordModel != null)
            {
                _context.MedicalRecords.Remove(medicalRecordModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalRecordModelExists(int id)
        {
            return _context.MedicalRecords.Any(e => e.MedicalRecordId == id);
        }
    }
}
