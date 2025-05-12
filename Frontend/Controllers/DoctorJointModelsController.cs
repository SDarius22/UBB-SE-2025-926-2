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
    public class DoctorJointModelsController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorJointModelsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DoctorJointModels
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.DoctorJoints.Include(d => d.Department).Include(d => d.User);
            return View(await appDbContext.ToListAsync());
        }

        // GET: DoctorJointModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorJointModel = await _context.DoctorJoints
                .Include(d => d.Department)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctorJointModel == null)
            {
                return NotFound();
            }

            return View(doctorJointModel);
        }

        // GET: DoctorJointModels/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role");
            return View();
        }

        // POST: DoctorJointModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,UserId,DepartmentId,Rating,LicenseNumber")] DoctorJointModel doctorJointModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctorJointModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", doctorJointModel.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", doctorJointModel.UserId);
            return View(doctorJointModel);
        }

        // GET: DoctorJointModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorJointModel = await _context.DoctorJoints.FindAsync(id);
            if (doctorJointModel == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", doctorJointModel.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", doctorJointModel.UserId);
            return View(doctorJointModel);
        }

        // POST: DoctorJointModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,UserId,DepartmentId,Rating,LicenseNumber")] DoctorJointModel doctorJointModel)
        {
            if (id != doctorJointModel.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorJointModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorJointModelExists(doctorJointModel.DoctorId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", doctorJointModel.DepartmentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", doctorJointModel.UserId);
            return View(doctorJointModel);
        }

        // GET: DoctorJointModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctorJointModel = await _context.DoctorJoints
                .Include(d => d.Department)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctorJointModel == null)
            {
                return NotFound();
            }

            return View(doctorJointModel);
        }

        // POST: DoctorJointModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctorJointModel = await _context.DoctorJoints.FindAsync(id);
            if (doctorJointModel != null)
            {
                _context.DoctorJoints.Remove(doctorJointModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorJointModelExists(int id)
        {
            return _context.DoctorJoints.Any(e => e.DoctorId == id);
        }
    }
}
