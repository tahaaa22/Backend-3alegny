using _3alegny.Service_layer;
using Microsoft.AspNetCore.Identity.Data;
using _3alegny.Entities;
using _3alegny.RepoLayer;
using MongoDB.Bson;
using MongoDB.Driver;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {

        app.MapPost("/signup/", (Func<SignupRequest, AuthLogic, IResult>)((request, logic) =>
        {
            var result = logic.Signup(request).Result;
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Message);
        })).WithTags("Auth")
         .WithOpenApi(operation => new(operation)
         {
             Summary = "Sign up a new user",
             Description = "This endpoint allows users to create an account by providing their signup details.",
             OperationId = "UserSignup",
         });

        app.MapPost("/login/", (Func<LoginRequest, AuthLogic, IResult>)((request, logic) =>
        {
            var result = logic.Login(request).Result;
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Message);
        })).WithTags("Auth")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Login for all user types (hospital, pharmacy, patient)",
                Description = "This endpoint allows users (hospital, pharmacy, patient) to login",
                OperationId = "UserLogIn",
            });
    }

    public record SignupRequest(
       string Name,
       string UserName,
       string Password,
       DateTime DateOfBirth,
       string Gender,
       string Phone,
       string Email,
       string Street,
       string City,
       string state,
       string ZipCode
   );

    public record LoginRequest(string UserName, string Password);
}