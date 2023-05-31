using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Interfaces
{
    public interface IAdviseeRepository
    {
        ICollection<AdviseeModel> GetAdvisees();
        ICollection<AdviseeModel> GetAdvisee(string projectId);
        bool CreateAdvisee(AdviseeModel advisee);
        bool DeleteAdvisee(string projectId);
        bool AdviseeExists(string projectId);
        bool Save();
    }
}
