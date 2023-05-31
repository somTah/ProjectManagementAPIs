using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Interfaces
{
    public interface IRoleRepository
    {
        ICollection<RoleModel> GetRoles();
        RoleModel GetRole(string roleId);
        RoleModel GetRoleByMemberUser(string memberUserId);
        ICollection<MemberUserModel> GetMemberUsersFromRole(string roleId);
        bool RoleExists(string roleId);

    }
}
