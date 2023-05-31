using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Interfaces
{
    public interface IProjectRepository
    {
        ICollection<ProjectModel> GetProjects();
        ProjectModel GetProject(string projectId);
        ICollection<MemberUserModel> GetAdviseeByProject(string projectId);
        ICollection<MemberUserModel> GetAdviserByProject(string projectId);
        bool CreateProject(string adviserId, ProjectModel project);
        bool UpdateProject(ProjectModel project);
        bool DeleteProject(ProjectModel project);
        bool ProjectExists(string projectId);
        bool Save();
    }
}
