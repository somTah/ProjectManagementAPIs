using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Interfaces
{
    public interface IProjectProgressRepository
    {
        ICollection<ProjectProgressModel> GetProjectProgresses();
        ProjectProgressModel? GetProjectProgress(string projectProgressId);
        ICollection<ProjectProgressModel> GetProjectProgressByProject(string projectId);
        bool ProjectProgressExists(string projectProgressId);
        bool CreateProjectProgress(ProjectProgressModel projectProgress);
        bool UpdateProjectProgress(string projectId, ProjectProgressModel projectProgress);
        bool DeleteProjectProgress(ProjectProgressModel projectProgress);
        bool DeleteProjectProgressByProject(string projectId);
        bool Save();

    }
}
