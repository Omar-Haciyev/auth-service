using TestProject.API.Middlewares;
using TestProject.API.Repositories.Interfaces;
using TestProject.API.Repositories.Classes;
using TestProject.API.Services.Interfaces;
using TestProject.API.Services.Classes;
using System.Data;
using TestProject.API.Helpers.Classes;
using TestProject.API.Helpers.Interfaces;
using WebExtensions.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(); 

var connectionString = builder.Configuration["ConnectionStrings:TestProjectDb"];

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}

builder.Services.AddScoped<ISecurityRepository>(_ => 
    new SecurityRepository(connectionString)
    {
        Command = new Command("TestProjectDb", "dbo", CommandType.StoredProcedure)
    });

builder.Services.AddScoped<ISecurityService, SecurityService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.MapControllers(); 

app.Run();