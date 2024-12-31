using _3alegny.Entities;
using _3alegny.Service_layer;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
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
        });

        //UPDATE Patient Profile
        app.MapPut("/patient/update/{id}", (Func<string, PatientUpdateRequest, PatientLogic, IResult>)((id, request, logic) =>
        {
            var result = logic.UpdatePatient(id, request).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update Patient Profile",
            Description = "This endpoint allows patients to update their profile.",
            OperationId = "UpdatePatient",
        });

        //Get Followup requests for a patient
        app.MapGet("/patient/followup/{id}", (Func<string, PatientLogic, IResult>)((id, logic) =>
        {
            var result = logic.GetFollowUp(id).Result;
            return Results.Ok(result);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get Followup requests",
            Description = "This endpoint allows patients to get their followup requests.",
            OperationId = "GetFollowup",
        });

        // select all avaliable hospitals depends on the filters
        app.MapPost("/patient/filterhospitals", async ([FromBody] HospitalFiltrationRequest<Hospital> request, [FromServices] PatientLogic logic) =>
        {
            var result = await logic.GetAvailableHospitals(request);
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        }).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
         {
             Summary = "Get List of All hospitals",
             Description = "this endpoint allow to get the list of all hospitals",
             OperationId = "GEThospitals",
         });

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


        app.MapGet("/patient/Hospitals", async ([FromServices] PatientLogic logic) =>
        {
            var result = await logic.GetAllHospitals();
            return Results.Ok(result);
        }).WithTags("Patient")
     .WithOpenApi(operation => new(operation)
     {
         Summary = "Get List of All Hospitals",
         Description = "this endpoint allow to get the list of all Hospitals",
         OperationId = "GETHospitals",
     });

        //get all hospitals
        app.MapGet("/patient/department/{DepartmentId}/TopDoctor", async (string DepartmentId,string HospitalId, [FromServices] PatientLogic logic) =>
        {
            var result = await logic.GetTopDoctor(DepartmentId, HospitalId);
            return Results.Ok(result);
        }).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get the top doctor in the current department",
            Description = "this endpoint allow to get the top doctor in the current department",
            OperationId = "GETtopDoctors",
        });
    }




    public record HospitalFiltrationRequest <T>
    (
        string PatientId = "",
        int price = 0,
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
        List<string> ImagingResults,
        List<string> LabResultsURL,
        string MedicalProcedures,
        string PrescriptionHistory,
        int Weight,
        int Height,
        int BMI
    );

    public record PatientUpdateRequest
    (
        string UserName,
        string Password,
        string imageUrl,
        string Phone,
        string Email,
        string Street,
        string City,
        string State,
        string ZipCode,
        string InsuranceName


    );

}