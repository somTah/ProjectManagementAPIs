using Microsoft.EntityFrameworkCore;
using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly DataContext _context;

        public AppointmentRepository(DataContext context)
        {
            _context = context;
        }

        public bool AppointmentExists(string appointmentId)
        {
            return _context.Appointments.Any(a => a.AppointmentId == appointmentId);
        }

        public bool CreateAppointment(AppointmentModel appointment)
        {
            appointment.AppointmentId = Guid.NewGuid().ToString();
            //appointment.AppointmentDate = DateTime.Now.ToString("dd/MM/yyyy");
            //appointment.AppointmentDateFrom = DateTime.Now.ToString("HH:mm");
            //appointment.AppointmentDateTo = DateTime.Now.ToString("HH:mm");
            _context.Add(appointment);
            return Save();
        }

        public bool DeleteAppointment(AppointmentModel appointment)
        {
            _context.Remove(appointment);
            return Save();
        }

        public AppointmentModel? GetAppointment(string appointmentId)
        {
            return _context.Appointments
        .Include(x => x.AppointmentReserves)
            .ThenInclude(r => r.Project) // Include the Project navigation property
        .Where(a => a.AppointmentId == appointmentId)
        .FirstOrDefault();
        }

        public ICollection<AppointmentModel> GetAppointments()
        {
            return _context.Appointments.Include(x => x.AppointmentReserves).OrderBy(a => a.AppointmentId).ToList();
        }

        public ICollection<ProjectModel> GetProjectByAppointment(string appointmentId)
        {
            return _context.AppointmentReserves.Where(a => a.Appointment.AppointmentId == appointmentId).Select(p => p.Project).ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateAppointment(AppointmentModel appointment)
        {
            _context.Update(appointment);
            return Save();
        }
    }
}
