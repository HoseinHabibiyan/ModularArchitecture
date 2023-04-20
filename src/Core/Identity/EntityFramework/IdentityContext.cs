using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace  ModularArchitecture.Identity.Core
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>, IIdentityContext
    {
        private readonly ILogger<IdentityContext> _logger;
        private readonly IOptions<IdentityContextOptions> _options;

        public IdentityContext(DbContextOptions dbContextOptions, ILogger<IdentityContext> logger,
            IOptions<IdentityContextOptions> options) : base(dbContextOptions)
        {
            _logger = logger;
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("identity");

            builder.Entity<ApplicationUser>().HasAlternateKey(x => x.Code);
            builder.Entity<ApplicationUser>().Property(x => x.Code).ValueGeneratedOnAdd();

            builder.Entity<ApplicationRole>().HasOne<Application>().WithMany().HasForeignKey(x => x.ApplicationId);
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole {Id = "1", Name = "SuperAdmin", NormalizedName = "SUPERADMIN"},
                new ApplicationRole {Id = "2", Name = "Admin", NormalizedName = "ADMIN"});

            builder.Entity<RefreshToken>().ToTable("RefreshTokens");
            builder.Entity<Application>().ToTable("Applications");
            builder.Entity<Client>().ToTable("Clients");

            this.RegisterEntities(builder, _logger);

            if (_options != null && _options.Value.SystematizePropertyNames)
            {
                var i = 0;
                foreach (var pb in builder.Model
                             .GetEntityTypes()
                             .SelectMany(t => t.GetProperties())
                             .Select(p => builder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name)))
                {
                    i++;

                    pb.HasColumnName($"SystemGenerated_{i}");
                }
            }
        }
    }
}
