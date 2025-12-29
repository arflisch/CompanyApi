using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

// Add your API project and wire up the connection string from the Postgres resource
var companyApi = builder.AddProject<Projects.CompanyApi>("companyapi")
    .WithEnvironment("ConnectionStrings__DefaultConnection", "Host=localhost;Port=5433;Database=company_db;Username=cae_user;Password=cae");

var companyFrontend = builder.AddProject<Projects.CompanyFrontend>("companyfrontend");

companyApi
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "company-api",
        AppPort = 7223,    
        ResourcesPaths = ["../BE/Dapr/Development"],
        LogLevel = "debug",
        AppProtocol = "https"
    });

builder.Build().Run();
