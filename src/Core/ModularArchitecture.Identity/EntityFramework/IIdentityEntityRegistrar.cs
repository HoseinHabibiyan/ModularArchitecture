using Microsoft.EntityFrameworkCore;

namespace ModularArchitecture.Identity.EntityFramework
{
    public interface IIdentityEntityRegistrar
    {
        void RegisterEntities(ModelBuilder modelBuilder);
    }
}