using _3alegny.Extensions;
using _3alegny.RepoLayer;
using _3alegny.Service_layer;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);

// Load .env file
//Env.Load();

//Configure connection string from environment variable
//var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DBCon");
//if (string.IsNullOrEmpty(connectionString))
//{
//    //throw new Exception("Connection string not found. Ensure the .env file is correctly configured and placed in the root directory.");

//    //Add connection string to the applications configuration system
//    builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
//{ {"ConnectionStrings:DBCon", connectionString }
//});
//}

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "3alegny phase 1",
        Version = "v1"
    });

    options.DocumentFilter<SwaggerTagDescriptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();

//connect resourse conc to handle frontend requests
builder.Services.AddCors();
builder.WebHost.CaptureStartupErrors(true)
               .UseSetting("detailedErrors", "true");


// Configure MongoDB
//var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
//var dbContext = new MongoDbContext(mongoDbSettings.ConnectionString, mongoDbSettings.DatabaseName);

// Dependency Injection
builder.Services.AddSingleton(dbContext);
builder.Services.AddScoped<UserLogic>();
builder.Services.AddScoped<AdminLogic>();
builder.Services.AddScoped<PatientLogic>();
builder.Services.AddScoped<CommonLogic>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Allows all origins
              .AllowAnyMethod() // Allows all HTTP methods
              .AllowAnyHeader(); // Allows all headers
    });
});

var app = builder.Build();
//app.UseCors(builder =>
//    builder.AddPolicy
//    builder.WithOrigins("http://localhost:3000") // FIXME: add your local host lel frontend
//           .AllowAnyMethod()
//           .AllowAnyHeader());


app.UseCors("AllowAll");

app.MapUserEndpoints();
app.MapAdminEndpoints();
app.MapPatientEndpoints();
app.MapCommonEndpoints();
app.MapHospitalEndpoints();

app.UseSwagger();
app.UseSwaggerUI(options=>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    options.RoutePrefix = string.Empty;
});
app.Run();

record MongoDbSettings(string ConnectionString, string DatabaseName);

// Add a custom filter to define tag descriptions
public class SwaggerTagDescriptionFilter : Swashbuckle.AspNetCore.SwaggerGen.IDocumentFilter
{
    public void Apply(Microsoft.OpenApi.Models.OpenApiDocument swaggerDoc, Swashbuckle.AspNetCore.SwaggerGen.DocumentFilterContext context)
    {
        swaggerDoc.Tags = new List<Microsoft.OpenApi.Models.OpenApiTag>
        {
            new() { Name = "Admin", Description = "Operations related to the Admin" },
            new() { Name = "User", Description = "Operations related to Authentication" },
            new() { Name = "Patient", Description = "Operations related to the Patient" },
            new() {Name= "Doctors", Description = "Operations related to the Doctors" },
            new() {Name= "Hospitals", Description = "Operations related to the Hospitals" },
            new() {Name= "Pharmacies", Description = "Operations related to the Pharmacies" },
            new() {Name = "Insurance", Description = "Insurance related operations"},
            new() {Name= "Departments", Description = "Operations related to the departments"}
        };
    }
}