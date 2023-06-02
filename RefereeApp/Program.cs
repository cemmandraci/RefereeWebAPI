using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using RefereeApp.Abstractions;
using RefereeApp.Concretes;
using RefereeApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("RefereeDbConnection"));
});

builder.Services.AddControllers();

builder.Services.AddFluentValidation(x =>
{
    x.RegisterValidatorsFromAssemblyContaining<Program>(lifetime: ServiceLifetime.Singleton);
    x.DisableDataAnnotationsValidation = true;
    
});

builder.Services.AddScoped<IRefereeLevelService, RefereeLevelService>();
builder.Services.AddScoped<IRefereeService, RefereeService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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