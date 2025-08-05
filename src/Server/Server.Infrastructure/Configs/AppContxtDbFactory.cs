namespace Server.Infrastructure.Configs;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
	public AppDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
		optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=Manager;Username=postgres;Password=postgres");
        
		return new AppDbContext(optionsBuilder.Options);
	}
}