namespace ProjectManagementAPIs.Models
{
    public class AppointmentReserveModel
    {
        public string? AppointmentId { get; set; }
        public string? ProjectId { get; set; }
        public string? ReserveTime { get; set; }
        public AppointmentModel? Appointment { get; set; } //one
        public ProjectModel? Project { get; set; } //one
    }
}
