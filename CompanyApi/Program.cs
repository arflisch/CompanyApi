using Database;
using Domain;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<dbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5433;Database=company_db;Username=cae_user;Password=cae");
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

// Register the repository
builder.Services.AddScoped<ICompanyRepository<Company>, CompanyContext>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
