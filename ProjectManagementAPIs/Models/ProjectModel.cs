using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPIs.Models
{
    public class ProjectModel
    {
        [Key]
        public string? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDetail { get; set; }
        public string? ProjectYear { get; set; }
        public string? ProjectContact { get; set; }
        public ICollection<AdviseeModel>? Advisees { get; set; } // Many
        public ICollection<AdviserModel>? Advisers { get; set; } // Many
        public ICollection<ProjectProgressModel>? ProjectProgresses { get; set; } //Many
        public ICollection<AppointmentReserveModel>? AppointmentReserves { get; set; }


    }
}
