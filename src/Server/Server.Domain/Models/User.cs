using Server.Domain.Abstractions;

namespace Server.Domain.Models;

public class User : ITrackable
{
  public long Id { get; set; }
  public string Username { get; set; } = default!;
  public string Role { get; set; } = default!;
  public byte[] PasswordHash { get; set; } = default!;
  public byte[] PasswordSalt { get; set; } = default!;

  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? ModifiedAt { get; set; }
}
