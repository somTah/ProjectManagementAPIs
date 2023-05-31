using Microsoft.EntityFrameworkCore;
using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Repository
{
    public class MemberUserRepository : IMemberUserRepository
    {
        private readonly DataContext _context;

        public MemberUserRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateMemberUser(MemberUserModel memberUser)
        {
            memberUser.MemberUserId = Guid.NewGuid().ToString();
            _context.Add(memberUser);
            return Save();
        }

        public bool DeleteMemberUser(MemberUserModel memberUser)
        {
            _context.Remove(memberUser);
            return Save();
        }

        public MemberUserModel? GetMemberUser(string memberUserId)
        {
            return _context.MemberUsers.Where(m => m.MemberUserId == memberUserId).FirstOrDefault();
        }

        public MemberUserModel? GetMemberUserByEmail(string memberUserEmail)
        {
            return _context.MemberUsers.FirstOrDefault(e => e.MemberUserEmail == memberUserEmail);
        }

        public MemberUserModel? GetMemberUserByFirstname(string firstname)
        {
            return _context.MemberUsers.FirstOrDefault(f => f.Firstname == firstname);
        }

        public MemberUserModel? GetMemberUserByLastname(string lastname)
        {
            return _context.MemberUsers.FirstOrDefault(l => l.Lastname == lastname);
        }

        public ICollection<MemberUserModel> GetMemberUserOfProject(string projectId)
        {
            return _context.Advisees.Where(w => w.Project.ProjectId == projectId).Select(m => m.MemberUser).ToList();
        }

        public ICollection<MemberUserModel> GetMemberUsers()
        {
            //return _context.MemberUsers.ToList();
            return _context.MemberUsers.Include(x => x.Role).ToList();
        }

        public ICollection<ProjectModel> GetProjectByMemberUser(string memberUserId, string roleId)
        {
            if (roleId == "PM03")
            {
                return _context.Advisees.Include(x => x.Project.Advisers).ThenInclude(x => x.MemberUser).Where(x => x.MemberUserId == memberUserId).Select(x => x.Project).ToList();
            }
            else if (roleId == "PM01")
            {
                return _context.Advisers.Include(x => x.Project.Advisers).ThenInclude(x => x.MemberUser).Where(x => x.MemberUserId == memberUserId).Select(x => x.Project).ToList();
            }
            else if (roleId == "PM02")
            {
                return _context.Advisers.Include(x => x.Project.Advisers).ThenInclude(x => x.MemberUser).Where(x => x.MemberUserId == memberUserId).Select(x => x.Project).ToList();
            }
            return null;
        }

        public bool MemberUserExists(string memberUserId)
        {
            return _context.MemberUsers.Any(m => m.MemberUserId == memberUserId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateMemberUser(MemberUserModel memberUser)
        {
            _context.Update(memberUser);
            return Save();
        }

        public bool UpdateRoleMemberUser(string roleId, MemberUserModel memberUser)
        {
            _context.Update(memberUser);
            return Save();
        }
    }
}
