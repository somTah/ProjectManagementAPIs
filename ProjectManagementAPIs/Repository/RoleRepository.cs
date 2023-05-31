using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DataContext _context;

        public RoleRepository(DataContext context)
        {
            _context = context;
        }
        public ICollection<MemberUserModel> GetMemberUsersFromRole(string roleId)
        {
            return _context.MemberUsers.Where(m => m.Role.RoleId == roleId).ToList();
        }

        public RoleModel GetRole(string roleId)
        {
            return _context.Roles.Where(r => r.RoleId == roleId).FirstOrDefault();
        }

        public RoleModel GetRoleByMemberUser(string memberUserId)
        {
            return _context.MemberUsers.Where(m => m.MemberUserId == memberUserId).Select(r => r.Role).FirstOrDefault();
        }

        public ICollection<RoleModel> GetRoles()
        {
            return _context.Roles.OrderBy(r => r.RoleId).ToList();
        }

        public bool RoleExists(string roleId)
        {
            return _context.Roles.Any(r => r.RoleId == roleId);
        }
    }
}
