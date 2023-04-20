using Microsoft.EntityFrameworkCore;
using ModularArchitecture.Identity.EntityFramework;

namespace ModularArchitecture.Identity.TwoFactorRegistration
{
    public class EntityRegistrar : IIdentityEntityRegistrar
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CellToken>().ToTable("CellTokens", "identity");
        }
    }
}