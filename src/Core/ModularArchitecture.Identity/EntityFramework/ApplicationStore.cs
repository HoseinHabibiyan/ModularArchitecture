using Microsoft.EntityFrameworkCore;
using ModularArchitecture.Identity.Core.Models;

namespace ModularArchitecture.Identity.EntityFramework
{
    public class ApplicationStore
    {
        private readonly DbSet<Application> _applications;
        private readonly IdentityContext _context;

        public ApplicationStore(IdentityContext context)
        {
            _context = context;
            _applications = context.Set<Application>();
        }

        public Application GetByName(string name)
        {
            return _applications.FirstOrDefault(x => x.Name == name);
        }
    }
}