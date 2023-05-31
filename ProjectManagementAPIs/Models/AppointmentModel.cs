using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPIs.Models
{
    public class AppointmentModel
    {
        [Key]
        public string? AppointmentId { get; set; }
        public string? AppointmentTitle { get; set; }
        public string? AppointmentDate { get; set; }
        public string? AppointmentDateFrom { get; set; }
        public string? AppointmentDateTo { get; set; }
        public string? AppointmentLocation { get; set; }
        public ICollection<AppointmentReserveModel>? AppointmentReserves { get; set; } //Many

    }
}
