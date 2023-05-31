using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Interfaces
{
    public interface IAdviserRepository
    {
        ICollection<AdviserModel> GetAdvisers();
        ICollection<AdviserModel> GetAdviser(string projectId);
        bool CreateAdviser(AdviserModel adviser);
        bool DeleteAdviser(string projectId);
        bool AdviserExists(string projectId);
        bool Save();

    }
}
