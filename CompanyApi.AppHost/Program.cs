using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

// Add Redis - Dapr l'utilisera comme state store
var redis = builder.AddRedis("redis")
    .WithRedisCommander(); // Interface web pour visualiser Redis

var postgres = builder.AddConnectionString("DefaultConnection");

// Add your API project
var companyApi = builder.AddProject<Projects.CompanyApi>("companyapi")
    .WithReference(postgres);  

//var companyFrontend = builder.AddProject<Projects.CompanyFrontend>("companyfrontend");

// Configure Dapr sidecar (utilisera automatiquement Redis via statestore.yaml)
companyApi
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "company-api",
        AppPort = 7223,    
        ResourcesPaths = ["../BE/Dapr/Development"],
        LogLevel = "debug",
        AppProtocol = "https"
    });

var notificationService = builder.AddProject<Projects.NotificationService>("notificationservice")
    .WithDaprSidecar(new DaprSidecarOptions
    {
        AppId = "notification-service",
        AppPort = 7026,
        ResourcesPaths = ["../BE/Dapr/Development"],
        LogLevel = "debug",
        AppProtocol = "https"
    });

builder.Build().Run();
