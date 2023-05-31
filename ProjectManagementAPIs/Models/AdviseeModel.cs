namespace ProjectManagementAPIs.Models
{
    public class AdviseeModel
    {
        public string? MemberUserId { get; set; }
        public string? ProjectId { get; set; }
        public MemberUserModel? MemberUser { get; set; } //one
        public ProjectModel? Project { get; set; } //one
    }
}
