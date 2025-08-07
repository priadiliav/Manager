using Server.Application.Abstractions;

namespace Server.Infrastructure.Utils;

public class PasswordHasher : IPasswordHasher
{
  public bool IsPasswordValid(string password, byte[] storedHash, byte[] storedSalt)
  {
    using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
    var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    return computedHash.SequenceEqual(storedHash);
  }

  public (byte[] hash, byte[] salt) CreatePasswordHash(string password)
  {
    using var hmac = new System.Security.Cryptography.HMACSHA512();
    var salt = hmac.Key;
    var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    return (hash, salt);
  }
}
