using Microsoft.AspNetCore.Identity.Data;
using _3alegny.Entities;
using _3alegny.RepoLayer;
using MongoDB.Bson;
using MongoDB.Driver;
using _3alegny.Service_layer;
public static class PatientEndpoints
{
    public static void MapPatientEndpoints(this WebApplication app)
    {
        app.MapPost("/patient/newphr/{patientid}", (Func<string,phrRequest, PatientLogic, IResult>)((pid,request, logic) =>
        {
            var result = logic.PostPHR(pid,request).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Post a new PHR",
            Description = "This endpoint allows patients to post a new PHR.",
            OperationId = "PostPHR",
        }
        );

        app.MapPut("/patient/updatephr/{id}", (Func<string, phrRequest, PatientLogic, IResult>)((pid, request, logic) =>
        {
            var result = logic.UpdatePHR(pid, request).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update PHR",
            Description = "This endpoint allows patients to update their PHR.",
            OperationId = "UpdatePHR",
        }
        );

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
    }
    public record phrRequest(
        string Allergies,
        string ChronicIllness,
        string Diagnosis,
        string Medication,
        string FamilyHistory,
        string ImagingResults,
        string LabResultsURL,
        string MedicalProcedures,
        string PrescriptionHistory,
        int Weight,
        int Height,
        int BMI
    );

}