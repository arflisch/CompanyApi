using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

// Ensure you have the correct namespace or library that provides the 'AddPostgres' extension method.
// If 'AddPostgres' is part of an external library, ensure the library is installed and the namespace is included.

var builder = DistributedApplication.CreateBuilder(args);



// Add your API project and wire up the connection string from the Postgres resource
builder.AddProject<Projects.CompanyApi>("companyapi")
    .WithEnvironment("ConnectionStrings__DefaultConnection", "Host=localhost;Port=5433;Database=company_db;Username=cae_user;Password=cae");

builder.Build().Run();
