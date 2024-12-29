using _3alegny.Service_layer;
using Microsoft.AspNetCore.Mvc;
using Sprache;


public static class PharmacyEndpoints
{
    public static void MapPharmacyEndpoints(this WebApplication app)
    {

        app.MapGet("/Pharmacy/{id}", async ([FromServices] PharmacyLogic logic, string id) =>
        {
            var result = await logic.GetPharmacyById(id);
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        }).WithTags("Pharmacy")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Get Pharmacy with ID",
                Description = "this endpoint allow to get the Pharmacy details using ID",
                OperationId = "GETPharmacy",
            }
            );

        
    }
}
