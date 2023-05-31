using ProjectManagementAPIs.Data;
using ProjectManagementAPIs.Interfaces;
using ProjectManagementAPIs.Models;

namespace ProjectManagementAPIs.Repository
{
    public class AdviserRepository : IAdviserRepository
    {
        private readonly DataContext _context;

        public AdviserRepository(DataContext context)
        {
            _context = context;
        }

        public bool AdviserExists(string projectId)
        {
            return _context.Advisers.Any(p => p.ProjectId == projectId);
        }

        public bool CreateAdviser(AdviserModel adviser)
        {
            _context.Add(adviser);
            return Save();
        }

        public bool DeleteAdviser(string projectId)
        {
            _context.Advisers.RemoveRange(_context.Advisers.Where(x => x.ProjectId == projectId));
            return Save();
        }

        public ICollection<AdviserModel> GetAdviser(string projectId)
        {
            return _context.Advisers.Where(a => a.ProjectId == projectId).ToList();
        }

        public ICollection<AdviserModel> GetAdvisers()
        {
            return _context.Advisers.ToList();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
