using _3alegny.Service_layer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

public static class CommonEndpoints
{
    public static void MapCommonEndpoints(this WebApplication app)
    {
        // Use dependency injection to inject the CommonLogic service
        app.MapGet("/top-hospitals", async (CommonLogic logic) =>
        {
            // Call the method to get the top-rated hospitals
            try
            {
                var hospitals = await logic.GetTopRatedHospitals();
                return Results.Ok(new { Success = true, Data = hospitals });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        }).WithTags("Hospitals").WithOpenApi(operation => new(operation)
        {
            Summary = "Get the top-rated hospitals",
            Description = "This endpoint returns the top-rated hospitals based on their ratings.",
            OperationId = "GetTopHospitals",
        });


        // Endpoint to get the top-rated doctors
        app.MapGet("/top-doctors", async (CommonLogic logic) =>
        {

            try
            {
                var doctors = await logic.GetTopRatedDoctors();
                return Results.Ok(new { Success = true, Data = doctors });
            }

            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        }).WithTags("Doctors").WithTags("Doctors").WithOpenApi(operation => new(operation)
        {
            Summary = "Get the top-rated doctors",
            Description = "This endpoint returns the top-rated doctors based on their ratings.",
            OperationId = "GetTopDoctors",
        });
    }
}
