using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using SWP_Final.Entities;
using SWP_Final.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddDbContext<RealEasteSWPContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("RealEasteSWP"));
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IUserRepositories, UserRepositories>();
builder.Services.AddScoped<IAgencyRepositories, AgencyRepositories>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");
app.UseStaticFiles();

app.MapControllers();

app.Run();
