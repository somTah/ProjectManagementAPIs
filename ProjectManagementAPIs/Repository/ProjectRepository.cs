using Microsoft.EntityFrameworkCore;
using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DataContext _context;

        public ProjectRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<ProjectModel> GetProjects()
        {
            return _context.Projects.Include(x => x.Advisers).ThenInclude(x => x.MemberUser).ToList();

        }

        public ProjectModel GetProject(string projectId)
        {
            return _context.Projects.Include(x => x.Advisees).ThenInclude(x => x.MemberUser)
                .Include(x => x.Advisers).ThenInclude(x => x.MemberUser).Where(x => x.ProjectId == projectId).FirstOrDefault();

        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;

        }

        public bool ProjectExists(string projectId)
        {
            return _context.Projects.Any(p => p.ProjectId == projectId);
        }

        public ICollection<MemberUserModel> GetAdviseeByProject(string projectId)
        {
            return _context.Advisees.Where(u => u.ProjectId == projectId).Select(c => c.MemberUser).ToList();
        }

        public bool CreateProject(string adviserId, ProjectModel project)
        {
            var adviserExist = _context.MemberUsers.Where(x => x.MemberUserId == adviserId).FirstOrDefault();

            project.ProjectId = Guid.NewGuid().ToString();
            project.ProjectYear = (DateTime.Now.Year + 543).ToString();

            _context.Add(project);

            var adviser = new AdviserModel()
            {
                MemberUser = adviserExist,
                Project = project
            };
            _context.Add(adviser);

            return Save();
        }

        public bool UpdateProject(ProjectModel project)
        {
            _context.Update(project);
            return Save();
        }

        public ICollection<MemberUserModel> GetAdviserByProject(string projectId)
        {
            return _context.Advisers.Where(u => u.ProjectId == projectId).Select(c => c.MemberUser).ToList();
        }

        public bool DeleteProject(ProjectModel project)
        {
            _context.Projects.Remove(project);
            return Save();
        }
    }
}
