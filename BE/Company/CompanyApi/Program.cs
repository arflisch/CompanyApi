using Application;
using Application.Services;
using Database;
using Domain;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers().AddDapr();
builder.Services.AddDaprClient();
builder.Services.AddTransient<ICreateCompanyCommand, CreateCompanyCommand>();
builder.Services.AddTransient<IUpdateCompanyCommand, UpdateCompanyCommand>();
builder.Services.AddTransient<IDeleteCompanyCommand, DeleteCompanyCommand>();
builder.Services.AddTransient<IPatchCompanyCommand, PatchCompanyCommand>();
builder.Services.AddTransient<IGetCompaniesCommand, GetCompaniesCommand>();
builder.Services.AddTransient<IGetCompanyByIdCommand, GetCompanyByIdCommand>();
builder.Services.AddSingleton<Application.Metrics.CompanyMetrics>();

// Register Dapr cache service
builder.Services.AddSingleton<IDaprCacheService, DaprCacheService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CompanyDtoValidator>();

builder.Services.AddDbContext<dbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// Register the repository
builder.Services.AddScoped<ICompanyRepository<Company>, CompanyRepository>();

builder.Services.AddOpenApiDocument(option =>
{
    option.DocumentName = "facade";
    option.ApiGroupNames = ["facade"];
    option.PostProcess = postProcess =>
    {
        postProcess.Info.Title = "Facade contracts are only used by Guis and no perinity is provided.";
    };
});


var app = builder.Build();

try
{
    Log.Information("Starting web host");

    app.MapDefaultEndpoints();

    if (app.Environment.IsDevelopment())
    {
        app.UseOpenApi(configure =>
        {
            configure.DocumentName = "facade";
        })
        .UseSwaggerUi(configure =>
        {
            configure.Path = "/swagger/facade";
            configure.DocumentPath = configure.Path + "/swagger.json";
            configure.TagsSorter = "alpha";
            configure.OperationsSorter = "alpha";
        });
    }
    app.UseCloudEvents();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.MapSubscribeHandler();

    

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
