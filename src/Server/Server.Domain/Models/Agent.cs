using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class Agent : ITrackable
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public long ConfigurationId { get; set; }
	public virtual Configuration Configuration { get; set; } = null!;
	
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? ModifiedAt { get; set; }
	
	public void ModifyFrom(Agent agent)
	{
		if (agent is null) 
			throw new ArgumentNullException(nameof(agent));
		
		Name = agent.Name;
		ConfigurationId = agent.ConfigurationId;
		Configuration = agent.Configuration;
	}
}