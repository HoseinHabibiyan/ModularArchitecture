namespace  ModularArchitecture.Identity.Core
{
    public class IdentityContextOptions
    {
        /// <summary>
        /// The connection string that is used to connect to the physical storage.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The assembly name where migrations are maintained for this context.
        /// </summary>
        public string MigrationsAssembly { get; set; }

        public bool EnableSensitiveDataLogging { get; set; }

        /// <summary>
        /// Change column names in EntityRegistrar
        /// </summary>
        public bool SystematizePropertyNames { get; set; }
    }
}
