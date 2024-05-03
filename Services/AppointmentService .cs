using DogGrooming_Server.Data;
using DogGrooming_Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DogGrooming_Server.Services
{
    public class AppointmentService
    {
        private readonly ServerDBContext _dbContext;

        public AppointmentService(ServerDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // Create Appointment
        public async Task<Appointment> CreateAppointment(Appointment newAppointment)
        {
            _dbContext.Appointment.Add(newAppointment);
            await _dbContext.SaveChangesAsync();
            return newAppointment;
        }

        // Edit Appointment
        public async Task EditAppointment(Appointment editedAppointment)
        {
            _dbContext.Entry(editedAppointment).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        // Remove Appointment
        public async Task RemoveAppointment(int appointmentId)
        {
            var appointmentToRemove = await _dbContext.Appointment.FindAsync(appointmentId);
            if (appointmentToRemove != null)
            {
                _dbContext.Appointment.Remove(appointmentToRemove);
                await _dbContext.SaveChangesAsync();
            }
        }

        // Get all Appointments
        public async Task<List<Appointment>> GetAllAppointments()
        {
            return await _dbContext.Appointment.Include(a => a.User).ToListAsync();
        }

        // Get Appointments by User Id
        public async Task<List<Appointment>> GetAppointmentsByUserId(int userId)
        {
            return await _dbContext.Appointment.Where(a => a.UserId == userId).ToListAsync();
        }

        // Get all Appointments with AppointmentDate bigger than now
        public async Task<List<Appointment>> GetFutureAppointments()
        {
            DateTime now = DateTime.Now;
            return await _dbContext.Appointment.Where(a => a.AppointmentDate > now).ToListAsync();
        }

        // Get Appointment by Id
        public async Task<Appointment> GetAppointmentById(int appointmentId)
        {
            return await _dbContext.Appointment.FindAsync(appointmentId);
        }
    }
}
