using Microsoft.AspNetCore.Mvc;
using Server.Application.Dtos.User;
using Server.Application.Services;

namespace Server.Api.Endpoints;

public static class UserEndpoints
{
  public static void MapUserEndpoints(this IEndpointRouteBuilder app)
  {
    app.MapPost("/api/users/login", async ([FromBody]UserLoginRequest loginRequest, IUserService userService) =>
    {
      var response = await userService.LoginAsync(loginRequest);
      return response is not null
          ? Results.Ok(response)
          : Results.Unauthorized();
    })
    .WithName("Login User")
    .WithTags("Users");

    app.MapPost("/api/users/register", async ([FromBody]UserRegisterRequest registerRequest, IUserService userService) =>
    {
      var response = await userService.RegisterAsync(registerRequest);
      return response is not null
          ? Results.Ok(response)
          : Results.BadRequest("User already exists or registration failed.");
    })
    .WithName("Register User")
    .WithTags("Users");
  }
}
