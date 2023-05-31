using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPIs.Models
{
    public class MemberUserModel
    {
        [Key]
        public string? MemberUserId { get; set; }
        public string? MemberUserEmail { get; set; }
        public string? passwordHash { get; set; }
        public string? passwordSalt { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? RoleId { get; set; }
        public RoleModel? Role { get; set; } // one
        public ICollection<AdviseeModel>? Advisees { get; set; } //Many
        public ICollection<AdviserModel>? Advisers { get; set; } //Many
    }
}
