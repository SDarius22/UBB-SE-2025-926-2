using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.ApiClients.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
using Frontend.DbContext;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class AppointmentModelsController : Controller
    {
        private readonly IDoctorApiService _doctorService;
        private readonly IAppointmentsApiService _appointmentsService;

        public AppointmentModelsController(IDoctorApiService doctorService, IAppointmentsApiService appointmentsService)
        {
            _doctorService = doctorService;
            _appointmentsService = appointmentsService;
        }

        // GET: AppointmentModels
        public async Task<IActionResult> Index()
        {
            //var appDbContext = _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient);
            //return View(await appDbContext.ToListAsync());

            var appointments = await _appointmentsService.GetAllAppointmentsAsync();

            return View(appointments.Select(a => a.ToModel()));
        }

        // GET: AppointmentModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentsService.GetAppointmentAsync(id.Value);

            return appointment == null ? NotFound() : View(appointment.ToModel());

        }

        // GET: AppointmentModels/Create
        public async Task<IActionResult> Create()
        {
            //ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId");
            //ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId");
            return View();
        }

        // POST: AppointmentModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,DoctorId,PatientId,DateAndTime,Finished,ProcedureId")] AppointmentModel appointmentModel)
        {
            if (!ModelState.IsValid)
            {
                //ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", appointmentModel.DoctorId);
                //ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId", appointmentModel.PatientId);
                //return View(appointmentModel);
            }

            bool response = await _appointmentsService.AddAppointmentAsync(appointmentModel);

            if (!response)
            {
                //ViewData["DoctorId"] = new SelectList(_context.DoctorJoints, "DoctorId", "DoctorId", appointmentModel.DoctorId);
                //ViewData["PatientId"] = new SelectList(_context.PatientJoints, "PatientId", "PatientId", appointmentModel.PatientId);
                //return View(appointmentModel);
            }

            return RedirectToAction(nameof(Index));

        }

        // GET: AppointmentModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return NotFound();
        }

        // POST: AppointmentModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,DoctorId,PatientId,DateAndTime,Finished,ProcedureId")] AppointmentModel appointmentModel)
        {
            return NotFound();
        }

        // GET: AppointmentModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentsService.GetAppointmentAsync(id.Value);

            return appointment == null ? NotFound() : View(appointment.ToModel());

        }

        // POST: AppointmentModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _appointmentsService.RemoveAppointmentAsync(id);
            
            // Do smth if we couldnt delete?
            return RedirectToAction(nameof(Index));
        }

    }
}
