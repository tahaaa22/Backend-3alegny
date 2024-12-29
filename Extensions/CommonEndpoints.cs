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
        }).WithTags("Common").WithOpenApi(operation => new(operation)
        {
            Summary = "Get the top-rated hospitals",
            Description = "This endpoint returns the top-rated hospitals based on their ratings.",
            OperationId = "GetTopHospitals",
        });


        app.MapGet("/top-doctors", async (CommonLogic logic) =>
        {
            try
            {
                // Fetch the top-rated doctors (as records)
                var doctors = await logic.GetTopRatedDoctors();

                // Return the records directly
                return Results.Ok(new { Success = true, Data = doctors });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        }).WithTags("Common").WithOpenApi(operation => new(operation)
        {
            Summary = "Get the top-rated doctors",
            Description = "This endpoint returns the top-rated doctors based on their ratings.",
            OperationId = "GetTopDoctors",
        });



        app.MapGet("/top-pharmacies", async (CommonLogic logic) =>
        {
            try
            {
                // Fetch the top-rated pharmacies (as records)
                var pharmacies = await logic.GetTopRatedPharmacies();

                // Return the records directly
                return Results.Ok(new { Success = true, Data = pharmacies });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        }).WithTags("Common").WithOpenApi(operation => new(operation)
        {
            Summary = "Get the top-rated pharmacies",
            Description = "This endpoint returns the top-rated pharmacies based on their ratings.",
            OperationId = "GetTopPharmacies",
        });


        // Endpoint to get Covered Insurance
        app.MapGet("/all-insurances", async (CommonLogic logic) =>
        {
            try
            {
                var insurance = await logic.GetHospitalInsurances();
                return Results.Ok(new { Success = true, Data = insurance });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        }).WithTags("Common").WithOpenApi(operation => new(operation)
        {
            Summary = "Get the covered insurances",
            Description = "This endpoint returns the list of insurance companies covered by the system.",
            OperationId = "GetCoveredInsurance",
        });
    }
}
