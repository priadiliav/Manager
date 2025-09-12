using Common.Messages.Agent.Login;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Dtos.User;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class AuthEndpoints
{
  public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
  {
    var group = app.MapGroup("/api/auth")
      .WithTags("Authentication");

    group.MapPost("/user/login",
      async (UserLoginRequest loginRequest, IAuthService authService) =>
      {
        var response = await authService.LoginUserAsync(loginRequest);
        return response is not null
          ? Results.Ok(response)
          : Results.Unauthorized();
      })
      .WithName("LoginUser");

    group.MapPost("/user/register",
      async (UserRegisterRequest registerRequest, IAuthService authService) =>
      {
        var response = await authService.RegisterUserAsync(registerRequest);
        return response is not null
          ? Results.Ok(response)
          : Results.BadRequest("User already exists or registration failed.");
      })
      .WithName("RegisterUser");

    group.MapPost("/agent/login",
      async ([FromBody] AgentLoginRequestMessage request, IAuthService authService) =>
      {
        var loginResponse = await authService.LoginAgentAsync(request);
        return loginResponse is null
            ? Results.Unauthorized()
            : Results.Ok(loginResponse);
      })
      .WithName("LoginAgent");
  }
}
