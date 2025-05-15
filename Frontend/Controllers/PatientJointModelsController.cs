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
    public class PatientJointModelsController : Controller
    {
        private readonly AppDbContext _context;

        public PatientJointModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PatientJointModels
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.PatientJoints.Include(p => p.User);

            return View(await appDbContext.ToListAsync());
        }

        // GET: PatientJointModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientJointModel = await _context.PatientJoints
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patientJointModel == null)
            {
                return NotFound();
            }

            return View(patientJointModel);
        }

        // GET: PatientJointModels/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role");
            return View();
        }

        // POST: PatientJointModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,PatientId,PatientName,BloodType,EmergencyContact,Allergies,Weight,Height")] PatientJointModel patientJointModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patientJointModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", patientJointModel.UserId);
            return View(patientJointModel);
        }

        // GET: PatientJointModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientJointModel = await _context.PatientJoints.FindAsync(id);
            if (patientJointModel == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", patientJointModel.UserId);
            return View(patientJointModel);
        }

        // POST: PatientJointModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,PatientId,PatientName,BloodType,EmergencyContact,Allergies,Weight,Height")] PatientJointModel patientJointModel)
        {
            if (id != patientJointModel.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patientJointModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientJointModelExists(patientJointModel.PatientId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", patientJointModel.UserId);
            return View(patientJointModel);
        }

        // GET: PatientJointModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patientJointModel = await _context.PatientJoints
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patientJointModel == null)
            {
                return NotFound();
            }

            return View(patientJointModel);
        }

        // POST: PatientJointModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patientJointModel = await _context.PatientJoints.FindAsync(id);
            if (patientJointModel != null)
            {
                _context.PatientJoints.Remove(patientJointModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientJointModelExists(int id)
        {
            return _context.PatientJoints.Any(e => e.PatientId == id);
        }
    }
}
