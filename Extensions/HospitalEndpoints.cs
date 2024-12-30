using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using _3alegny.Service_layer;
using _3alegny.Entities;
using Microsoft.AspNetCore.Mvc;
using static _3alegny.Service_layer.HospitalLogic;

public static class HospitalEndpoints
{
    public static void MapHospitalEndpoints(this WebApplication app)
    {
        // POST endpoint to add a new department
        app.MapPost("/Hospital/add-department", async ([FromServices] HospitalLogic logic, string hospitalId, string departmentName) =>
        {
            try
            {
                var result = await logic.AddDepartment(hospitalId, departmentName);
                return Results.Ok(new { Success = true, Message = result });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        })
        .WithTags("Hospital")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Add a new department",
            Description = "Adds a new department to a hospital based on its ID.",
            OperationId = "AddDepartment"
        });

        app.MapPost("/Hospital/add-doctor", async ([FromServices] HospitalLogic logic, [FromBody] DoctorResponse doctor) =>
        {
            try
            {
                var response = await logic.AddDoctor(doctor);
                return Results.Ok(response);
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        })
.WithTags("Hospital")
.WithOpenApi(operation => new(operation)
{
    Summary = "Add a new doctor",
    Description = "Adds a new doctor to a hospital.",
    OperationId = "AddDoctor"
});




//        app.MapPut("/Hospital/upsert-doctor/{doctorId}", async ([FromServices] HospitalLogic logic, string doctorId, [FromBody] record UpdatedDoctor) =>
//        {
//            try
//            {
//                var response = await logic.UpdateDoctorById(doctorId, UpdatedDoctor);
//                return Results.Ok(response);
//            }
//            catch (Exception e)
//            {
//                return Results.BadRequest(new { Success = false, Message = e.Message });
//            }
//        })
//.WithTags("Hospital")
//.WithOpenApi(operation => new(operation)
//{
//    Summary = "Get and update doctor by ID",
//    Description = "Fetches a doctor's details based on their ID and allows updating those details.",
//    OperationId = "UpsertDoctorById"
//});



        // DELETE endpoint to remove a doctor by ID
        app.MapDelete("/Hospital/delete-doctor/{doctorId}", async ([FromServices] HospitalLogic logic, string doctorId) =>
        {
            try
            {
                var result = await logic.DeleteDoctorById(doctorId);
                return Results.Ok(new { Success = true, Message = result });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        })
        .WithTags("Hospital")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Delete a doctor by ID",
            Description = "Removes a doctor from the hospital's doctor list by their ID.",
            OperationId = "DeleteDoctorById"
        });

        app.MapPost("/Hospital/post-ehr", async ([FromServices]  HospitalLogic logic, [FromBody] EHR ehr) =>
        {
            try
            {
                var result = await logic.CreateEHR(ehr);
                return Results.Ok(new { Success = true, Message = result, PatientId = ehr.PatientId });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        })
        .WithTags("Hospital")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Create a new EHR for a patient",
            Description = "Adds a new EHR record for a patient if they don't already have one.",
            OperationId = "CreateEHR"
        });

        //FIXME: hospital do not have auth to get request EHR

        app.MapPut("/Hospital/update-doctor/{doctorId}", async ([FromServices] HospitalLogic logic, string doctorId, [FromBody] UpdateDoctorRequest updatedDoctor) =>
        {
            try
            {
                var response = await logic.UpdateDoctorById(doctorId, updatedDoctor);
                return Results.Ok(response);
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        })
.WithTags("Hospital")
.WithOpenApi(operation => new(operation)
{
    Summary = "Update doctor details",
    Description = "Updates a doctor's details based on their ID.",
    OperationId = "UpdateDoctorById"
});


        // PUT endpoint to update an EHR by ID
        app.MapPut("Hospital/update-ehr/{ehrId}", async ([FromServices] HospitalLogic logic, string ehrId, [FromBody] EHR updatedEHR) =>
        {
            try
            {
                var result = await logic.UpdateEHRById(ehrId, updatedEHR);
                return Results.Ok(new { Success = true, Message = result });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        })
             .WithTags("Hospital")
             .WithOpenApi(operation => new(operation)
             {
                 Summary = "Update EHR by ID",
                 Description = "Updates an EHR document by its ID.",
                 OperationId = "UpdateEHRById"
             });



        app.MapGet("/Hospital/{id}", async ([FromServices] HospitalLogic logic, string id) =>
        {
            var result = await logic.GetHospitalById(id);
            return result.IsSuccess ? Results.Ok(result.Data) : Results.NotFound(result.Message);
        }).WithTags("Hospital")
          .WithOpenApi(operation => new(operation)
          {
              Summary = "Get hospital with ID",
              Description = "this endpoint allow to get the hospital details using ID",
              OperationId = "GETHospital",
          }
          );

        app.MapPost("/Hospital/followup",(Func<string,FollowupRequest,HospitalLogic,IResult>)((pname,request,logic) =>
        {
            var result = logic.AddFollowUp(pname, request).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("Hospital")
            .WithOpenApi(operation => new(operation)
            {
                Summary = "Add a follow-up",
                Description = "This endpoint allows hospitals to add a follow-up for a patient.",
                OperationId = "AddFollowUp",
            });



        //app.MapPut("/Hospital/update/{hospitalId}", async ([FromServices] HospitalLogic logic, string hospitalId, [FromBody] Hospital updatedHospital) =>
        //{
        //    try
        //    {
        //        var result = await logic.UpdateHospitalById(hospitalId, updatedHospital);
        //        return Results.Ok(result);
        //    }
        //    catch (Exception e)
        //    {
        //        return Results.BadRequest(new { Success = false, Message = e.Message });
        //    }
        //})
        //    .WithTags("Hospital")
        //    .WithOpenApi(operation => new(operation)
        //    {
        //        Summary = "Update hospital by ID",
        //        Description = "Updates a hospital's information based on its ID and returns the updated data.",
        //        OperationId = "UpdateHospitalById"
        //    });

        app.MapPost("/Hospital/create-bill", async ([FromServices] HospitalLogic logic, [FromBody] HospitalBilling bill) =>
        {
            try
            {
                // Create the bill using the logic method
                var result = await logic.CreateHospitalBill(bill);
                return Results.Ok(new { Success = true, Message = "Bill created successfully", BillId = result });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        })
   .WithTags("Hospital")
   .WithOpenApi(operation => new(operation)
   {
       Summary = "Create a new hospital bill",
       Description = "Creates a billing record for a patient, including doctor details, appointment fee, and insurance information.",
       OperationId = "CreateHospitalBill"
   });

        app.MapGet("/Hospital/get-ehr/{ehrId}", async ([FromServices] HospitalLogic logic, string ehrId) =>
        {
            try
            {
                var ehr = await logic.GetEHRById(ehrId); // This should work if only one GetEHRById exists
                return Results.Ok(new { Success = true, Message = "EHR found", EHR = ehr });
            }
            catch (Exception e)
            {
                return Results.BadRequest(new { Success = false, Message = e.Message });
            }
        })
.WithTags("Hospital")
.WithOpenApi(operation => new(operation)
{
    Summary = "Get EHR by ID",
    Description = "Fetches an Electronic Health Record (EHR) document by its ID.",
    OperationId = "GetEHRById"
});





    }


    public record HospitalRequest
        (

         
         );


    public record FollowupRequest
    {
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Date { get; set; }
        public string Notes { get; set; }
        public string Department { get; set; }
    }

    public record DoctorResponse(
    string? Id,
    string Name,
    string Specialty,
    string HospitalId,
    string License,
    string Description,
    string city,
    string state,
    string zipcode,
    string street,
    int AppointmentFee,
    string ImageUrl,
    List<DateTime>? AvailableSlots);

    public record UpdateDoctorRequest
    {
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string License { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Street { get; set; }
        public string? Reviews { get; set; }
        public Double Rating { get; set; }
        public int AppointmentFee { get; set; }
        public string ImageUrl { get; set; }
        public List<DateTime> AvailableSlots { get; set; }
        public List<DateTime> RegisteredSlots { get; set; }
    }


}

