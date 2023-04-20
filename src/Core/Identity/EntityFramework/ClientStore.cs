using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModularArchitecture.Identity;

namespace  ModularArchitecture.Identity.Core
{
    public class ClientStore
    {
        private readonly DbSet<Client> _clients;
        private readonly IdentityContext _context;

        public ClientStore(IdentityContext context)
        {
            _context = context;
            _clients = context.Set<Client>();
        }

        public Client FindClient(string clientId)
        {
            var client = _clients.Find(clientId);
            return client;
        }

        public List<Client> GetAllClients()
        {
            var clients = _clients.ToList();
            return clients;
        }
    }
}