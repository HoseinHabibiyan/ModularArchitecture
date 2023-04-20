using Microsoft.EntityFrameworkCore;

namespace ModularArchitecture.Identity.Core
{
    public class EntityRegistrar : IIdentityEntityRegistrar
    {
        public void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CellToken>().ToTable("CellTokens", "identity");
        }
    }
}