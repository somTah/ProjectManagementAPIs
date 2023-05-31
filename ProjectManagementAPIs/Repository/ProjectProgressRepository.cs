using Microsoft.EntityFrameworkCore;
using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Repository
{
    public class ProjectProgressRepository : IProjectProgressRepository
    {
        private readonly DataContext _context;

        public ProjectProgressRepository(DataContext context)
        {
            _context = context;
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool CreateProjectProgress(ProjectProgressModel projectProgress)
        {
            projectProgress.ProjectProgressId = Guid.NewGuid().ToString();
            projectProgress.DateForm = DateTime.Now.ToString();
            projectProgress.CommentTeacher = null;
            projectProgress.DateTeacher = null;
            _context.Add(projectProgress);
            return Save();

        }

        public ProjectProgressModel? GetProjectProgress(string projectProgressId)
        {
            return _context.ProjectProgresses.Include(x => x.Project).Where(p => p.ProjectProgressId == projectProgressId).FirstOrDefault();
        }

        public ICollection<ProjectProgressModel> GetProjectProgresses()
        {
            return _context.ProjectProgresses.OrderBy(p => p.ProjectProgressId).ToList();

        }

        public ICollection<ProjectProgressModel> GetProjectProgressByProject(string projectId)
        {
            return _context.ProjectProgresses.Include(x => x.Project).Where(p => p.Project.ProjectId == projectId).ToList();
        }

        public bool ProjectProgressExists(string projectProgressId)
        {
            return _context.ProjectProgresses.Any(p => p.ProjectProgressId == projectProgressId);
        }

        public bool UpdateProjectProgress(string projectId, ProjectProgressModel projectProgress)
        {
            projectProgress.DateTeacher = DateTime.Now.ToString();
            _context.Update(projectProgress);
            return Save();
        }

        public bool DeleteProjectProgress(ProjectProgressModel projectProgress)
        {
            _context.Remove(projectProgress);
            return Save();
        }

        public bool DeleteProjectProgressByProject(string projectId)
        {
            _context.ProjectProgresses.RemoveRange(_context.ProjectProgresses.Where(x => x.ProjectId == projectId));
            return Save();
        }
    }
}
