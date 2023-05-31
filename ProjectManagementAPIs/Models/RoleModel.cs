using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPIs.Models
{
    public class RoleModel
    {
        [Key]
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public ICollection<MemberUserModel>? MemberUsers { get; set; } //Many

    }
}
