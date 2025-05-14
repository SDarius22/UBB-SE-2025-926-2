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
    public class ScheduleModelsController : Controller
    {
        private readonly IScheduleApiService _scheduleService;

        public ScheduleModelsController(IScheduleApiService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: ScheduleModels
        public async Task<IActionResult> Index()
        {
            return View(await _scheduleService.GetSchedulesAsync());
        }

        private async Task<IActionResult> GetScheduleActionResult(int? id)
        {
            if (id == null) return NotFound();

            var model = await _scheduleService.GetScheduleAsync(id.Value);

            return model == null ? NotFound() : View(model);
        }

        // GET: ScheduleModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return await GetScheduleActionResult(id);
        }

        // GET: ScheduleModels/Create
        public IActionResult Create()
        {
            //ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId");
            //ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID");
            return View();
        }

        // POST: ScheduleModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,ShiftId,ScheduleId")] ScheduleModel schedule)
        {
            
            if (!ModelState.IsValid)
            {
                //ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", schedule.DoctorId);
                //ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID", schedule.ShiftId);
             
                return View(schedule);
            }

            bool response = await _scheduleService.AddScheduleAsync(schedule);

            if (!response)
            {
                //ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", schedule.DoctorId);
                //ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID", schedule.ShiftId);

                return View(schedule);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ScheduleModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", schedule.DoctorId);
            //ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID", schedule.ShiftId);
            
            // poate trebuie facut ceva extra cu alea de mai sus?

            return await GetScheduleActionResult(id);
        }

        // POST: ScheduleModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,ShiftId,ScheduleId")] ScheduleModel schedule)
        {
            if (id != schedule.ScheduleId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                //ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", schedule.DoctorId);
                //ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID", schedule.ShiftId);

                return View(schedule);
            }

            var response = await _scheduleService.UpdateScheduleAsync(id, schedule);

            if (!response)
            {
                //ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", schedule.DoctorId);
                //ViewData["ShiftId"] = new SelectList(_context.Shifts, "ShiftID", "ShiftID", schedule.ShiftId);

                return View(schedule);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ScheduleModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await GetScheduleActionResult(id);
        }

        // POST: ScheduleModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _scheduleService.DeleteScheduleAsync(id);

            // return daca da fail?
            return RedirectToAction(nameof(Index));
        }

    }
}
