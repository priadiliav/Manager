namespace Server.Application.Dtos.User;

public class UserLoginRequest
{
  public string Username { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}
