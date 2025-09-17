using Microsoft.EntityFrameworkCore;
using Server.Domain.Abstractions;
using Server.Domain.Models;

namespace Server.Infrastructure.Configs;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
		public DbSet<Domain.Models.Agent> Agents { get; set; }
		public DbSet<Domain.Models.Configuration> Configurations { get; set; }
		public DbSet<Domain.Models.Policy> Policies { get; set; }
		public DbSet<Domain.Models.Process> Processes { get; set; }
    public DbSet<Domain.Models.User> Users { get; set; }

    public DbSet<Domain.Models.AgentHardware> Hardware { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

			#region Policy in Configuration m:m
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

			#region Process in Configuration m:m
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

      #region Hardware in Agent 1:1
      modelBuilder.Entity<AgentHardware>()
          .HasOne(h => h.Agent)
          .WithOne(a => a.Hardware)
          .HasForeignKey<AgentHardware>(h => h.AgentId)
          .OnDelete(DeleteBehavior.Cascade)
          .IsRequired();
      #endregion
		}

		public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
		{
			var entries = ChangeTracker.Entries()
					.Where(e => e is { Entity: ITrackable, State: EntityState.Added or EntityState.Modified });

			var now = DateTimeOffset.UtcNow;

			foreach (var entry in entries)
      {
        var entity = (ITrackable)entry.Entity;
        switch (entry.State)
        {
          case EntityState.Added:
            entity.CreatedAt = now;
            entity.ModifiedAt = now;
            break;
          case EntityState.Modified:
            entity.ModifiedAt = now;
            break;
          case EntityState.Detached:
            break;
          case EntityState.Unchanged:
            break;
          case EntityState.Deleted:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

			return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}
}
