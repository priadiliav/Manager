namespace Server.Domain.Abstractions;

public interface ITrackable
{
	DateTimeOffset CreatedAt { get; set; }
	DateTimeOffset? ModifiedAt { get; set; }
}
