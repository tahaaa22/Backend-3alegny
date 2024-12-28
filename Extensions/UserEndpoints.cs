using _3alegny.Service_layer;
using Microsoft.AspNetCore.Identity.Data;
using _3alegny.Entities;
using _3alegny.RepoLayer;
using MongoDB.Bson;
using MongoDB.Driver;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {

        app.MapPost("users/signup", (Func<SignupRequest, UserLogic, IResult>)((request, logic) =>
        {
            var result = logic.Signup(request).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("User")
         .WithOpenApi(operation => new(operation)
         {
             Summary = "Sign up a new user",
             Description = "This endpoint allows users to create an account by providing their signup details.",
             OperationId = "UserSignup",
         });

        app.MapPost("users/login", (Func<LoginRequest, UserLogic, IResult>)((request, logic) =>
        {
            var result = logic.Login(request).Result;
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Message);
        })).WithTags("User")
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