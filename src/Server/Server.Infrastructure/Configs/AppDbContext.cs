using Microsoft.EntityFrameworkCore;
using Server.Domain.Models;

namespace Server.Infrastructure.Configs;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
		public DbSet<Domain.Models.Agent> Agents { get; set; }
		public DbSet<Domain.Models.Configuration> Configurations { get; set; }
		public DbSet<Domain.Models.Policy> Policies { get; set; }
		public DbSet<Domain.Models.Process> Processes { get; set; }
		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		
			#region Policy in Configuration
			modelBuilder.Entity<PolicyInConfiguration>()
					.HasKey(pc => new { pc.ConfigurationId, pc.PolicyId });

			modelBuilder.Entity<PolicyInConfiguration>()
					.HasOne(pc => pc.Configuration)
					.WithMany(c => c.Policies)
					.HasForeignKey(pc => pc.ConfigurationId);

			modelBuilder.Entity<PolicyInConfiguration>()
					.HasOne(pc => pc.Policy)
					.WithMany(p => p.Configurations)
					.HasForeignKey(pc => pc.PolicyId);
			#endregion
        
			#region Process in Configuration
			modelBuilder.Entity<ProcessInConfiguration>()
					.HasKey(pc => new { pc.ConfigurationId, pc.ProcessId });

			modelBuilder.Entity<ProcessInConfiguration>()
					.HasOne(pc => pc.Configuration)
					.WithMany(c => c.Processes)
					.HasForeignKey(pc => pc.ConfigurationId);

			modelBuilder.Entity<ProcessInConfiguration>()
					.HasOne(pc => pc.Process)
					.WithMany(p => p.Configurations)
					.HasForeignKey(pc => pc.ProcessId);
			#endregion
		}
}