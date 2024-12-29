using _3alegny.Service_layer;
using Microsoft.AspNetCore.Mvc;
using _3alegny.Entities;
using Sprache;


public static class PharmacyEndpoints
{
    public static void MapPharmacyEndpoints(this WebApplication app)
    {
        // Getting Pharmacy by ID
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
        });

        // Adding Drug to Pharmacy
        app.MapPost("/Pharmacy/{pharmacyId}/AddDrugs", async ([FromServices] PharmacyLogic logic, string pharmacyId, Drugs drug) =>
        {
            var result = await logic.AddDrug(pharmacyId, drug);
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        }).WithTags("Pharmacy")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Add Drug to Pharmacy",   
            Description = "this endpoint allow to add a new drug to the pharmacy",
            OperationId = "POSTDrug",
        });

        // Updating Drug Quantity Add/Remove
        app.MapPut("/Pharmacy/{pharmacyId}/UpdateDrugs/", async ([FromServices] PharmacyLogic logic, string pharmacyId,string drugName, int quantity, bool flag) =>
        {
            var result = await logic.UpdateDrugQuantity(pharmacyId,drugName, quantity, flag);
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        }).WithTags("Pharmacy")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update Drug Quantity",
            Description = "this endpoint allow to update the quantity of a drug",
            OperationId = "PUTDrug",
        });

        // Getting List of All Drugs
        app.MapGet("/Pharmacy/Drugs", async ([FromServices] PharmacyLogic logic) =>
        {
            var result = await logic.GetAllDrugs();
            return Results.Ok(result);
        }).WithTags("Pharmacy")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get List of All Drugs",
            Description = "this endpoint allow to get the list of all drugs",
            OperationId = "GETDrugs",
        });
    }
}
