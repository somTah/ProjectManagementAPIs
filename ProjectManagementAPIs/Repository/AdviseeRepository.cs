using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Repository
{
    public class AdviseeRepository : IAdviseeRepository
    {
        private readonly DataContext _context;

        public AdviseeRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateAdvisee(AdviseeModel advisee)
        {
            _context.Add(advisee);
            return Save();
        }

        public ICollection<AdviseeModel> GetAdvisee(string projectId)
        {
            return _context.Advisees.Where(u => u.ProjectId == projectId).ToList();
        }

        public ICollection<AdviseeModel> GetAdvisees()
        {
            return _context.Advisees.ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;

        }

        public bool AdviseeExists(string projectId)
        {
            return _context.Advisees.Any(p => p.ProjectId == projectId);
        }

        public bool DeleteAdvisee(string projectId)
        {
            _context.Advisees.RemoveRange(_context.Advisees.Where(x => x.ProjectId == projectId));
            return Save();
        }
    }
}
