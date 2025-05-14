//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Frontend.ApiClients.Interface;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Frontend.DbContext;
//using Frontend.Models;

//// Nu vad sa trebuiasca
//namespace Frontend.Controllers
//{
//    public class DoctorJointModelsController : Controller
//    {
//        private readonly IDoctorApiService _doctorService;

//        public DoctorJointModelsController(IDoctorApiService doctorService)
//        {
//            _doctorService = doctorService;
//        }

//        // GET: DoctorJointModels
//        public async Task<IActionResult> Index()
//        {
//            return View(await _doctorService.GetDoctorsAsync());
//        }

//        private async Task<IActionResult> GetDoctor(int? id)
//        {
//            if (id == null) return NotFound();

//            var doctor = await _doctorService.GetDoctorAsync(id.Value);

//            return doctor == null ? NotFound() : View(doctor);
//        }

//        // GET: DoctorJointModels/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            return await GetDoctor(id);
//        }

//        // GET: DoctorJointModels/Create
//        public IActionResult Create()
//        {
//            //ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID");
//            //ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role");
//            return View();
//        }

//        // POST: DoctorJointModels/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("DoctorId,UserId,DepartmentId,Rating,LicenseNumber")] DoctorJointModel doctorJoint)
//        {
//            if (!ModelState.IsValid)
//            {
//                //ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", doctorJointModel.DepartmentId);
//                //ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", doctorJointModel.UserId);

//                return View(doctorJoint);
//            }

//            bool response = await _doctorService.AddDoctorAsync(doctorJoint);

//            //ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", doctorJointModel.DepartmentId);
//            //ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", doctorJointModel.UserId);

//            return response ? RedirectToAction(nameof(Index)) : View(doctorJoint);

//        }

//        // GET: DoctorJointModels/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            return await GetDoctor(id);
//        }

//        // POST: DoctorJointModels/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,UserId,DepartmentId,Rating,LicenseNumber")] DoctorJointModel doctorJoint)
//        {
//            if (id != doctorJoint.D)
//            {
//                return NotFound();
//            }

//            if (!ModelState.IsValid)
//            {
//                //ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", doctorJointModel.DepartmentId);
//                //ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", doctorJointModel.UserId);

//                return View(department);
//            }

//            var response = await _departmentsService.UpdateDepartmentAsync(id, department);

//            return response ? RedirectToAction(nameof(Index)) : View(department);

//            //if (id != doctorJointModel.DoctorId)
//            //{
//            //    return NotFound();
//            //}

//            //if (ModelState.IsValid)
//            //{
//            //    try
//            //    {
//            //        _context.Update(doctorJointModel);
//            //        await _context.SaveChangesAsync();
//            //    }
//            //    catch (DbUpdateConcurrencyException)
//            //    {
//            //        if (!DoctorJointModelExists(doctorJointModel.DoctorId))
//            //        {
//            //            return NotFound();
//            //        }
//            //        else
//            //        {
//            //            throw;
//            //        }
//            //    }
//            //    return RedirectToAction(nameof(Index));
//            //}
//            //ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentID", "DepartmentID", doctorJointModel.DepartmentId);
//            //ViewData["UserId"] = new SelectList(_context.Users, "UserID", "Role", doctorJointModel.UserId);
//            //return View(doctorJointModel);
//        }

//        // GET: DoctorJointModels/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            return await GetDoctor(id);
//        }

//        // POST: DoctorJointModels/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var doctorJointModel = await _context.DoctorJoints.FindAsync(id);
//            if (doctorJointModel != null)
//            {
//                _context.DoctorJoints.Remove(doctorJointModel);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }
        
//    }
//}
