using _3alegny.Service_layer;

public static class AppointmentEndpoints
{
    // Schedule an appointment
    public static void MapAppointmentEndpoints(this WebApplication app)
    {
        // Schedule an appointment
        app.MapPost("/appointment/schedule/{id}", (Func<string, string, AppointmentRequest, AppointmentLogic, IResult>)((pid, HospitalID, request, logic) =>
        {
            var result = logic.ScheduleAppointment(pid, HospitalID, request).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Schedule an appointment",
            Description = "This endpoint allows patients to schedule an appointment with a doctor.",
            OperationId = "ScheduleAppointment",
        });

        // Get a patient's appointments using patient ID
        app.MapGet("/appointment/get/{id}", (Func<string, AppointmentLogic, IResult>)((id, logic) =>
        {
            var result = logic.GetAppointments(id).Result;
            return Results.Ok(result);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get Appointments",
            Description = "This endpoint allows patients to get their appointments.",
            OperationId = "GetAppointments",
        });
        // Get all appointments
        app.MapGet("/appointment/all", (Func<AppointmentLogic, IResult>)((logic) =>
        {
            var result = logic.GetAllAppointments().Result;
            return Results.Ok(result);
        })).WithTags("Admin")
        .WithOpenApi(operation => new(
            operation)
        {
            Summary = "Get all appointments",
            Description = "This endpoint allows patients to get all appointments.",
            OperationId = "GetAllAppointments",
        });

        // Cancel an appointment
        app.MapDelete("/appointment/cancel/{id}", (Func<string, AppointmentLogic, string, IResult>)((id, logic, time) =>
        {
            var result = logic.CancelAppointment(id,time).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Cancel an appointment",
            Description = "This endpoint allows patients to cancel an appointment.",
            OperationId = "CancelAppointment",
        });

        // Update an appointment
        app.MapPut("/appointment/update/{id}", (Func<string, AppointmentRequest,string, AppointmentLogic, IResult>)((id, request,time, logic) =>
        {
            var result = logic.ChangeAppointment(id, time,request).Result;
            return result.IsSuccess ? Results.Ok(result.Message) : Results.BadRequest(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(
            operation)
        {
            Summary = "Update an appointment",
            Description = "This endpoint allows patients to update an appointment.",
            OperationId = "UpdateAppointment",
        });

        // Get all appointments for a specific doctor
        app.MapGet("/appointment/doctor/{id}", (Func<string, AppointmentLogic, IResult>)((id, logic) =>
        {
            var result = logic.GetDoctorAppointments(id).Result;
            return Results.Ok(result);
        })).WithTags("Admin")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get doctor appointments",
            Description = "This endpoint allows patients to get all appointments for a specific doctor.",
            OperationId = "GetDoctorAppointments",
        });

        // Get all appointments for a specific hospital
        app.MapGet("/appointment/hospital/{id}", (Func<string, AppointmentLogic, IResult>)((hid, logic) =>
        {
            var result = logic.GetHospitalAppointments(hid).Result;
            return Results.Ok(result);
        })).WithTags("Hospital")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get hospital appointments",
            Description = "This endpoint allows patients to get all appointments for a specific hospital.",
            OperationId = "GetHospitalAppointments",
        });

        // Get all appointments for a specific department
        app.MapGet("/appointment/department/{id}", (Func<string, AppointmentLogic, IResult>)((id, logic) =>
        {
            var result = logic.GetDepartmentAppointments(id).Result;
            return Results.Ok(result);
        })).WithTags("Admin")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get department appointments",
            Description = "This endpoint allows patients to get all appointments for a specific department.",
            OperationId = "GetDepartmentAppointments",
        });

        
    }

    public class AppointmentRequest
    {
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string DoctorName { get; set; }
        public string Department { get; set; }
        public string HospitalName { get; set; }
        public string PHRId { get; set; }


    }
}
