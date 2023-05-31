using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Interfaces
{
    public interface IMemberUserRepository
    {
        ICollection<MemberUserModel> GetMemberUsers();
        MemberUserModel? GetMemberUser(string memberUserId);
        MemberUserModel? GetMemberUserByEmail(string memberUserEmail);
        MemberUserModel? GetMemberUserByFirstname(string firstname);
        MemberUserModel? GetMemberUserByLastname(string lastname);
        ICollection<MemberUserModel> GetMemberUserOfProject(string projectId);
        ICollection<ProjectModel> GetProjectByMemberUser(string memberUserId, string roleId);
        bool MemberUserExists(string memberUserId);
        bool CreateMemberUser(MemberUserModel memberUser);
        bool UpdateMemberUser(MemberUserModel memberUser);
        bool UpdateRoleMemberUser(string roleId, MemberUserModel memberUser);
        bool DeleteMemberUser(MemberUserModel memberUser);
        bool Save();

    }
}
