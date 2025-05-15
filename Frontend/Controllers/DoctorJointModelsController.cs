using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class DoctorJointModelsController : Controller
    {
        private readonly IDoctorApiService _doctorService;

        public DoctorJointModelsController(IDoctorApiService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET: DoctorJointModels
        public async Task<IActionResult> Index()
        {
            var doctors = await _doctorService.GetDoctorsAsync();
            return View(doctors);
        }

        private async Task<IActionResult> GetDoctorViewById(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _doctorService.GetDoctorAsync(id.Value);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

        // GET: DoctorJointModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return await GetDoctorViewById(id);
        }

        // GET: DoctorJointModels/Create
        public IActionResult Create()
        {
            // If you need to populate dropdowns for DepartmentId and UserId,
            // you would typically fetch this data via _doctorService or another relevant service
            // and pass it to the view, e.g., via ViewData or a ViewModel.
            // Example (assuming your service has these methods):
            // ViewData["DepartmentId"] = new SelectList(await _doctorService.GetDepartmentsAsync(), "DepartmentID", "Name");
            // ViewData["UserId"] = new SelectList(await _doctorService.GetUsersAsync(), "UserID", "FullName");
            return View();
        }

        // POST: DoctorJointModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,UserId,DepartmentId,Rating,LicenseNumber")] DoctorJointModel doctorJoint)
        {
            if (ModelState.IsValid)
            {
                bool response = await _doctorService.AddDoctorAsync(doctorJoint);
                if (response)
                {
                    return RedirectToAction(nameof(Index));
                }
                // If AddDoctorAsync returns false, it indicates a failure,
                // so we should add a model error or handle it appropriately.
                ModelState.AddModelError(string.Empty, "Failed to create doctor. Please try again.");
            }

            // Repopulate dropdowns if necessary on validation failure
            // ViewData["DepartmentId"] = new SelectList(await _doctorService.GetDepartmentsAsync(), "DepartmentID", "Name", doctorJoint.DepartmentId);
            // ViewData["UserId"] = new SelectList(await _doctorService.GetUsersAsync(), "UserID", "FullName", doctorJoint.UserId);
            return View(doctorJoint);
        }

        // GET: DoctorJointModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Similar to Create, populate dropdowns if needed
            // ViewData["DepartmentId"] = new SelectList(await _doctorService.GetDepartmentsAsync(), "DepartmentID", "Name");
            // ViewData["UserId"] = new SelectList(await _doctorService.GetUsersAsync(), "UserID", "FullName");
            return await GetDoctorViewById(id);
        }

        // POST: DoctorJointModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,UserId,DepartmentId,Rating,LicenseNumber")] DoctorJointModel doctorJoint)
        {
            if (id != doctorJoint.DoctorId) // Corrected property name
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Assuming IDoctorApiService has an UpdateDoctorAsync method
                bool response = await _doctorService.UpdateDoctorAsync(id, doctorJoint); 
                if (response)
                {
                    return RedirectToAction(nameof(Index));
                }
                // If UpdateDoctorAsync returns false, handle the error
                ModelState.AddModelError(string.Empty, "Failed to update doctor. Please try again.");
            }

            // Repopulate dropdowns if necessary on validation failure
            // ViewData["DepartmentId"] = new SelectList(await _doctorService.GetDepartmentsAsync(), "DepartmentID", "Name", doctorJoint.DepartmentId);
            // ViewData["UserId"] = new SelectList(await _doctorService.GetUsersAsync(), "UserID", "FullName", doctorJoint.UserId);
            return View(doctorJoint); // Corrected variable name
        }

        // GET: DoctorJointModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await GetDoctorViewById(id);
        }

        // POST: DoctorJointModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Assuming IDoctorApiService has a DeleteDoctorAsync method
            bool success = await _doctorService.DeleteDoctorAsync(id); 
            if (!success)
            {
                // If deletion fails, you might want to return to the delete view with an error
                // or redirect to an error page. For now, redirecting to Index.
                // Consider fetching the doctor again to show the delete confirmation page with an error.
                TempData["ErrorMessage"] = "Failed to delete doctor. Please try again.";
                return RedirectToAction(nameof(Delete), new { id = id }); 
            }
            return RedirectToAction(nameof(Index));
        }
        
        // Helper method to check if a doctor exists, might be useful if not using API for everything
        // private async Task<bool> DoctorJointModelExists(int id)
        // {
        //     var doctor = await _doctorService.GetDoctorAsync(id);
        //     return doctor != null;
        // }
    }
}
