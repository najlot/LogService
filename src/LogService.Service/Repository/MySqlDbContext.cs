using Microsoft.EntityFrameworkCore;
using LogService.Service.Configuration;
using LogService.Service.Model;

namespace LogService.Service.Repository
{
	public class MySqlDbContext : DbContext
	{
		private readonly MySqlConfiguration _configuration;

		public MySqlDbContext(MySqlConfiguration configuration)
		{
			_configuration = configuration;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = $"server={_configuration.Host};" +
				$"port={_configuration.Port};" +
				$"database={_configuration.Database};" +
				$"uid={_configuration.User};" +
				$"password={_configuration.Password}";

			var serverVersion = ServerVersion.AutoDetect(connectionString);
			optionsBuilder.UseMySql(connectionString, serverVersion);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<UserModel>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.OwnsOne(e => e.Settings);
			});
			modelBuilder.Entity<LogMessageModel>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.OwnsMany(e => e.Arguments, e => { e.HasKey(e => e.Id); e.ToTable("LogMessage_Arguments"); });
			});
		}

		public DbSet<UserModel> Users { get; set; }
		public DbSet<LogMessageModel> LogMessages { get; set; }
	}
}