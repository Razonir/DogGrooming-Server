using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DogGrooming_Server.Data.Models;
using DogGrooming_Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogGrooming_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;
        private readonly UserService _userService;

        public AppointmentController(AppointmentService appointmentService, UserService userService)
        {
            _appointmentService = appointmentService;
            _userService = userService;

        }

        // POST: api/Appointment
        // Create Appointment
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Appointment>> CreateAppointment(Appointment newAppointment)
        {
            var user = await GetUserFromToken();
            newAppointment.UserId = user.UserId;
            var createdAppointment = await _appointmentService.CreateAppointment(newAppointment);
            return Ok(createdAppointment);
        }

        // PUT: api/Appointment/5
        // Edit Appointment
        [HttpPut("{AppointmentId}")]
        [Authorize]
        public async Task<IActionResult> EditAppointment(int AppointmentId, Appointment editedAppointment)
        {
            var user = await GetUserFromToken();

            if (user.Role != "Admin" && user.UserId != editedAppointment.UserId)
            {
                return Unauthorized("משתמש יכול לערוך רק את הרשומה של עצמו");
            }
            if (AppointmentId != editedAppointment.AppointmentId)
            {
                return BadRequest();
            }

            await _appointmentService.EditAppointment(editedAppointment);

            return NoContent();
        }

        // DELETE: api/Appointment/5
        // Remove Appointment
        [HttpDelete("{AppointmentId}")]
        [Authorize]
        public async Task<IActionResult> RemoveAppointment(int AppointmentId)
        {
            var user = await GetUserFromToken();
            var appointment = await _appointmentService.GetAppointmentById(AppointmentId);
            if (user.Role !="Admin" && user.UserId != appointment.UserId)
            {
                return Unauthorized("משתמש יכול למחוק רק את הרשומה של עצמו");
            }
            await _appointmentService.RemoveAppointment(AppointmentId);

            return NoContent();
        }

        // GET: api/Appointment
        // Get all Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments()
        {
            var appointments = await _appointmentService.GetAllAppointments();
            return Ok(appointments);
        }

        // GET: api/Appointment/user
        // Get Appointments by User JWT
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByUserId()
        {
            var user = await GetUserFromToken();

            var appointments = await _appointmentService.GetAppointmentsByUserId(user.UserId);
            return Ok(appointments);
        }

        // GET: api/Appointment/future
        // Get all Appointments with AppointmentDate bigger than now
        [HttpGet("future")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetFutureAppointments()
        {
            var futureAppointments = await _appointmentService.GetFutureAppointments();
            return Ok(futureAppointments);
        }

        // GET: api/Appointment/5
        // Get Appointmet by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _appointmentService.GetAppointmentById(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }

        private async Task<User> GetUserFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("Invalid JWT token.");
            }
            var userId = Int32.Parse(userIdClaim.Value);
            var user = await _userService.GetUserById(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            return user;
        }
    }
}
