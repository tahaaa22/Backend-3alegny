using _3alegny.Service_layer;
using _3alegny.Entities;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;


public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        // Admin - Get all users 
        app.MapGet("/admin/users", (Func<AdminLogic, IResult>)(logic =>
        {
            var result = logic.GetAllUsers().Result;
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        })).WithTags("Admin");

        // Admin - Get a user by specific ID
        app.MapGet("/admin/user/{id}", (Func<string, AdminLogic, IResult>)((id, logic) =>
        {
            var result = logic.GetUserById(id).Result;
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        })).WithTags("Admin");

        // Admin - Delete a user
        app.MapDelete("/admin/user/{id}", (Func<string, AdminLogic, IResult>)((id, logic) =>
        {
            var result = logic.DeleteUser(id).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.NotFound(result.Message);
        })).WithTags("Admin");

        app.MapPost("/admin/create/hospital", async ([FromBody] HospitalRequest request, [FromServices] AdminLogic logic) =>
        {
            var hospital = await logic.CreateHopital(request);

            return hospital.IsSuccess ? Results.Ok(hospital.Message) : Results.BadRequest(hospital.Message);
        }).WithTags("Admin")
          .WithOpenApi(operation => new(operation)
          {
                Summary = "Create a Hospital",
                Description = "Receives hospital data from the frontend and creates a new hospital.",
                OperationId = "CreateHospital"
          });

    }



    public record HospitalRequest(
     string Name,
     string UserName,
     string Role, 
     string Password,
     ContactInfo contactInfo,
     Address Address,
     DateTime CreatedAt,
     DateTime UpdatedAt
    );

}

