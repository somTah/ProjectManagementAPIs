using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Interfaces
{
    public interface IAppointmentReserveRepository
    {
        ICollection<AppointmentReserveModel> GetAppointmentReserves();
        ICollection<AppointmentReserveModel> GetAppointmentReserve(string projectId);
        bool CreateAppointmentReserve(AppointmentReserveModel appointmentReserve);
        bool DeleteAppointmentReserve(string projectId);
        bool DeleteAppointmentReserveByAppointment(string appointmentId);
        bool AppointmentReserveExists(string projectId);
        bool AppointmentReserveExistsByAppointment(string appointmentId);
        bool Save();
    }
}
