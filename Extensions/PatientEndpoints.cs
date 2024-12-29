using _3alegny.Entities;
using _3alegny.Service_layer;
using Microsoft.AspNetCore.Mvc;
using static AdminEndpoints;
using static PatientEndpoints;
public static class PatientEndpoints
{
    public static void MapPatientEndpoints(this WebApplication app)
    {
        // create New PHR for the patient
        app.MapPost("/patient/newphr", (Func<phrRequest, PatientLogic, IResult>)((request, logic) =>
        {
            var result = logic.PostPHR(request).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Post a new PHR",
            Description = "This endpoint allows patients to post a new PHR.",
            OperationId = "PostPHR",
        }
        );
        // update patient PHR using patient ID
        app.MapPost("/patient/updatephr/{id}", (Func<string, phrRequest, PatientLogic, IResult>)((id, request, logic) =>
        {
            var result = logic.UpdatePHR(id, request).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update PHR",
            Description = "This endpoint allows patients to update their PHR.",
            OperationId = "UpdatePHR",
        }
        );

        // get patient PHR using patient ID
        app.MapGet("/patient/getphr/{id}", (Func<string, PatientLogic, IResult>)((id, logic) =>
        {
            var result = logic.GetPHR(id).Result;
            return Results.Ok(result);
        }))
        .WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get PHR",
            Description = "This endpoint allows patients to get their PHR.",
            OperationId = "GetPHR",
        }
        );

        // Get a patient by specific ID
        app.MapGet("/patient/{id}", (Func<string,PatientLogic, IResult>)((id, logic) =>
        {
            var result = logic.GetPatientById(id).Result;
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get patient with ID",
            Description = "this endpoint allow to get the patient details using ID",
            OperationId = "GETPatient",
        }
        );
        // select all avaliable hospitals depends on the filters
        app.MapPost("/patient/hospitals", async ([FromBody] HospitalFiltrationRequest<Hospital> request, [FromServices] PatientLogic logic) =>
        {
            var result = await logic.GetAvailableHospitals(request); 
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        }).WithTags("Patient");

        app.MapGet("/patient/pharmacies", async ([FromServices] PatientLogic logic) =>
        {
            var result = await logic.GetAllPharmacies();
            return Results.Ok(result);
        }).WithTags("Patient")
      .WithOpenApi(operation => new(operation)
      {
          Summary = "Get List of All pahrmacies",
          Description = "this endpoint allow to get the list of all pharmacies",
          OperationId = "GETpharmacies",
      });
    }


    public record HospitalFiltrationRequest <T>
    (
        string PatientId = "",
        string price = "",
        string street = "",
        string department = "",
        string rating = ""
        );

    public record phrRequest(
        string Allergies,
        string ChronicIllness,
        string Diagnosis,
        string Medication,
        string FamilyHistory,
        string ImagingResults,
        string LabResults,
        string MedicalProcedures,
        string PrescriptionHistory
    );

}