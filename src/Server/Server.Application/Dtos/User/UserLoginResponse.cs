namespace Server.Application.Dtos.User;

public class UserLoginResponse
{
  public string Token { get; set; } = string.Empty;
  public string Role { get; set; } = string.Empty;
  public string Username { get; set; } = string.Empty;
}
