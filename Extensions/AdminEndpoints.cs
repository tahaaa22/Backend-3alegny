using _3alegny.Service_layer;
using _3alegny.Entities;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharpCompress.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;


public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        // Admin - Get all users 
        app.MapGet("/admin/users", (Func<AdminLogic, IResult>)(logic =>
        {
            var result = logic.GetAllUsers().Result;
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        })).WithTags("admin");

        // Admin - Get a user by specific ID
        app.MapGet("/admin/user/{id}", (Func<string, AdminLogic, IResult>)((id, logic) =>
        {
            var result = logic.GetUserById(id).Result;
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        })).WithTags("admin");

        // Admin - Delete a user
        app.MapDelete("/admin/user/{id}", (Func<string, AdminLogic, IResult>)((id, logic) =>
        {
            var result = logic.DeleteUser(id).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.NotFound(result.Message);
        })).WithTags("admin");

        //Admin - Receives data from the frontend and creates a new business. (pharmcies / hospital)
        app.MapPost("/admin/create/Business", async ([FromBody] User request, [FromServices] AdminLogic logic) =>
        {
            AdminResult<User> business = new AdminResult<User>{ IsSuccess = true, Data = request, Message = "initial creation "  };
            //if(request.Role == UserRole.Hospital)
            //     business = await logic.CreateBusiness<Hospital>(request, _context.Hospitals, UserRole.Hospital);
            //if(request.Role == UserRole.Hospital)
            //     business = await logic.CreateBusiness<Pharmacy>(request, _context.Pharmacies, UserRole.Pharmacy);


            return business.IsSuccess ? Results.Ok(business.Message) : Results.BadRequest(business.Message);
        }).WithTags("admin")
          .WithOpenApi(operation => new(operation)
          {
              Summary = "creates a new business. (pharmcies / hospital)",
              Description = "Receives Business data from the frontend and creates a new business. (pharmcies / hospital).",
              OperationId = "CreateBusiness"
          });

    }



    public record BusinessRequest(
     string Name,
     string UserName,
     UserRole Role,
     string Password,
     ContactInfo contactInfo,
     Address Address,
     DateTime CreatedAt,
     DateTime UpdatedAt
    );

}

