using Microsoft.EntityFrameworkCore;
using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Repository
{
    public class AppointmentReserveRepository : IAppointmentReserveRepository
    {
        private readonly DataContext _context;

        public AppointmentReserveRepository(DataContext context)
        {
            _context = context;
        }

        public bool AppointmentReserveExists(string projectId)
        {
            return _context.AppointmentReserves.Any(p => p.ProjectId == projectId);
        }

        public bool AppointmentReserveExistsByAppointment(string appointmentId)
        {
            return _context.AppointmentReserves.Any(p => p.AppointmentId == appointmentId);
        }

        public bool CreateAppointmentReserve(AppointmentReserveModel appointmentReserve)
        {
            _context.Add(appointmentReserve);
            return Save();
        }

        public bool DeleteAppointmentReserve(string projectId)
        {
            _context.AppointmentReserves.RemoveRange(_context.AppointmentReserves.Where(x => x.ProjectId == projectId));
            return Save();
        }

        public bool DeleteAppointmentReserveByAppointment(string appointmentId)
        {
            _context.AppointmentReserves.RemoveRange(_context.AppointmentReserves.Where(x => x.AppointmentId == appointmentId));
            return Save();
        }

        public ICollection<AppointmentReserveModel> GetAppointmentReserve(string projectId)
        {
            return _context.AppointmentReserves.Include(x => x.Project).Include(x => x.Appointment).Where(a => a.ProjectId == projectId).ToList();
        }

        public ICollection<AppointmentReserveModel> GetAppointmentReserves()
        {
            return _context.AppointmentReserves.ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
