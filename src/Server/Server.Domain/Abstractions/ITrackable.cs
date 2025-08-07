namespace Server.Domain.Abstractions;

//todo: IEntity<long>
public interface ITrackable
{
	DateTimeOffset CreatedAt { get; set; }
	DateTimeOffset? ModifiedAt { get; set; }
}
