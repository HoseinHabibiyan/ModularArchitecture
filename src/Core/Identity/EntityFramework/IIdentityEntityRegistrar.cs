using Microsoft.EntityFrameworkCore;

namespace  ModularArchitecture.Identity.Core
{
    public interface IIdentityEntityRegistrar
    {
        void RegisterEntities(ModelBuilder modelBuilder);
    }
}