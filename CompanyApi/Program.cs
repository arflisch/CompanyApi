using Application;
using Database;
using Domain;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddTransient<ICreateCompanyCommand, CreateCompanyCommand>();
builder.Services.AddTransient<IUpdateCompanyCommand, UpdateCompanyCommand>();
builder.Services.AddTransient<IDeleteCompanyCommand, DeleteCompanyCommand>();
builder.Services.AddTransient<IPatchCompanyCommand, PatchCompanyCommand>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CompanyDtoValidator>();

builder.Services.AddDbContext<dbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
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
