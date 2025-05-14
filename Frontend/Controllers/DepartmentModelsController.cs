using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Frontend.DbContext;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class DepartmentModelsController : Controller
    {
        private readonly IDepartmentsApiService _departmentsService;

        public DepartmentModelsController(IDepartmentsApiService departmentsService)
        {
            _departmentsService = departmentsService;
        }

        // GET: DepartmentModels
        public async Task<IActionResult> Index()
        {
            return View(await _departmentsService.GetDepartmentsAsync());
        }

        // GET: DepartmentModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return await GetDepartmentActionResult(id);
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
        public async Task<IActionResult> Create([Bind("DepartmentID,Name")] DepartmentModel department)
        {
            if (!ModelState.IsValid)
            {
                return View(department);
            }
            
            bool response = await _departmentsService.AddDepartmentAsync(department);

            return response ? RedirectToAction(nameof(Index)) : View(department);

        }

        // GET: DepartmentModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await GetDepartmentActionResult(id);
        }

        // POST: DepartmentModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentID,Name")] DepartmentModel department)
        {
            if (id != department.DepartmentID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(department);
            }

            var response = await _departmentsService.UpdateDepartmentAsync(id, department);

            return response ? RedirectToAction(nameof(Index)) : View(department);
        }

        // GET: DepartmentModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await GetDepartmentActionResult(id);
        }

        // POST: DepartmentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _departmentsService.DeleteDepartmentAsync(id);

            // return daca da fail?
            return RedirectToAction(nameof(Index));

        }

        private async Task<IActionResult> GetDepartmentActionResult(int? id)
        {
            if (id == null) return NotFound();

            var model = await _departmentsService.GetDepartmentAsync(id.Value);

            return model == null ? NotFound() : View(model);
        }

    }
}
