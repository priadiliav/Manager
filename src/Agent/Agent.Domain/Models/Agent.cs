namespace Agent.Domain.Models;

public class Agent
{
  #region Long term properties
  public Guid Id { get; init; } = Guid.Empty;
  public string Secret { get; init; } = string.Empty;
  #endregion

  #region Short term properties
  public string Token { get; set; } = string.Empty;
  #endregion

  #region Functions
  /// <summary>
  /// Update the authentication token of the agent
  /// </summary>
  /// <param name="token"></param>
  public void UpdateToken(string token) => Token = token;
  #endregion
}
