using System.ComponentModel.DataAnnotations;

namespace ProjectManagementAPIs.Models
{
    public class RefreshTokenModel
    {
        [Key]
        public string? RefreshTokenId { get; set; }
        public string? MemberUserEmail { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
