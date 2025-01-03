﻿using _3alegny.Entities;
using _3alegny.Service_layer;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        // Create new order
        app.MapPost("/order/create/{patientId}/{pharmacyId}", (Func<string, string, OrderRequest,OrdersLogic, IResult>)((pid, phid, request, logic) =>
        {
            var result = logic.CreateOrder(pid, phid, request).Result;
            return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result.Message);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Create new order",
            Description = "This endpoint allows patients to create a new order.",
            OperationId = "CreateOrder",
        });

        // Get patient orders
        app.MapGet("/order/get/{patientId}", (Func<string, OrdersLogic, IResult>)((pid, logic) =>
        {
            var result = logic.GetPatientOrders(pid).Result;
            return Results.Ok(result);
        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get patient orders",
            Description = "This endpoint allows patients to get their orders.",
            OperationId = "GetPatientOrders",
        });

        //Get all orders    
        app.MapGet("/order/all", (Func<OrdersLogic, IResult>)((logic) =>
        {
            var result = logic.GetAllOrders().Result;
            return Results.Ok(result);
        })).WithTags("Admin")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get all orders",
            Description = "This endpoint allows patients to get all orders.",
            OperationId = "GetAllOrders",
        });

        //Get orders of pharmacyID
        app.MapGet("/order/pharmacy/{pharmacyId}", (Func<string, OrdersLogic, IResult>)((phid, logic) =>
        {
            var result = logic.GetPharmacyOrders(phid).Result;
            return Results.Ok(result);
        })).WithTags("Pharmacy")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Get orders of pharmacy",
            Description = "This endpoint allows pharmacies to get their orders.",
            OperationId = "GetPharmacyOrders",
        });

        //Update Order Status
        app.MapPut("/order/status/{patientid}", (Func<string,string,string, OrdersLogic, IResult>)((pid,status,oid, logic) =>
        {
            var result = logic.UpdateOrderStatus(pid,status,oid).Result;
            return Results.Ok(result);
        })).WithTags("Pharmacy")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Update Order Status",
            Description = "This endpoint allows pharmacies to change order status",
            OperationId = "UpdateOrderStatus",
        });

        //Cancel Order
        app.MapDelete("/order/cancel/{patientid}", (Func<string,string, OrdersLogic, IResult>)((pid,oid, logic) =>
        {
            var result = logic.CancelOrder(pid,oid).Result;
            return Results.Ok(result);

        })).WithTags("Patient")
        .WithOpenApi(operation => new(operation)
        {
            Summary = "Cancel Order",
            Description = "This endpoint allows patients to cancel their orders.",
            OperationId = "CancelOrder",
        });
            
            
            
    }
}

public record OrderRequest
{
    public string PatientId { get; set; }
    public string PharmacyId { get; set; }
    public DateTime Created { get; set; }
    public string Drug { get; set; }
    public string DrugCategory { get; set; }
    public int DrugQuantity { get; set; }
    public string street { get; set; }
    public string city { get; set; }
    public string zipcode { get; set; }
    public string state { get; set; }
}