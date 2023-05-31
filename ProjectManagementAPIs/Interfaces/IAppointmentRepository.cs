using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Interfaces
{
    public interface IAppointmentRepository
    {
        ICollection<AppointmentModel> GetAppointments();
        AppointmentModel? GetAppointment(string appointmentId);
        ICollection<ProjectModel> GetProjectByAppointment(string appointmentId);
        bool AppointmentExists(string appointmentId);
        bool CreateAppointment(AppointmentModel appointment);
        bool UpdateAppointment(AppointmentModel appointment);
        bool DeleteAppointment(AppointmentModel appointment);
        bool Save();

    }
}
