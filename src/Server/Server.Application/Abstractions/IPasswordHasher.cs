namespace Server.Application.Abstractions;

public interface IPasswordHasher
{
  bool IsPasswordValid(string password, byte[] storedHash, byte[] storedSalt);
  (byte[] hash, byte[] salt) CreatePasswordHash(string password);
}
