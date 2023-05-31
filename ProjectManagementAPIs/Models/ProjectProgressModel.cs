using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPIs.Models
{
    public class ProjectProgressModel
    {
        [Key]
        public string? ProjectProgressId { get; set; }
        public string? DateForm { get; set; }
        public int NumberProgress { get; set; }
        public string? SummaryProgress { get; set; }
        public string? SolutionToImprove { get; set; }
        public string? GoalOfWork { get; set; }
        public int WorkProgress { get; set; }
        public string? CommentTeacher { get; set; }
        public string? DateTeacher { get; set; }
        public string? ProjectId { get; set; }
        public ProjectModel? Project { get; set; } //one

    }
}
