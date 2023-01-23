using AutoMapper;
using FluentValidation;
using MicroEntities.Application;
using MicroEntities.Data.SqlServer;
using MicroEntities.Utils;
using MicroEntities.Validation;
using Microsoft.Extensions.DependencyInjection;
using TestBed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
									.SetMinimumLevel(LogLevel.Trace)
									.AddDebug()
									.AddConsole());

var employeeSystem = new PublicEntitySystem<EmployeeDto, Employee>();
	employeeSystem
	.AddLayer(new BenchmarkingLayer<Employee>(loggerFactory))
	.AddLayer(new FluentValidationLayer<Employee>(validator => 
	{ 
		validator.RuleFor(employee => employee.FirstName).Length(1, 50).Matches("^[a-zA-Z'., -]*$"); 
		validator.RuleFor(employee => employee.LastName).Length(1, 50).Matches("^[a - zA - Z'., -]*$");
	}))
	.AddLayer(new SqlServerSystemLayer<Employee>("server=WSL3CNZZRG3;database=TestBed;Trusted_Connection=SSPI"));

var userSystem = new PublicEntitySystem<UserDto, User>();
userSystem
.AddLayer(new BenchmarkingLayer<User>(loggerFactory))
.AddLayer(new FluentValidationLayer<User>(validator =>
{
	validator.RuleFor(employee => employee.UserName).Length(1, 50).Matches("^[a-zA-Z'., -]*$");
	validator.RuleFor(employee => employee.Password).Length(1, 50).Matches("^[a-zA-Z0-9]*$");
	validator.RuleFor(user => user.Balance).GreaterThanOrEqualTo(0);
}))
.AddLayer(new SqlServerSystemLayer<User>("server=WSL3CNZZRG3;database=TestBed;Trusted_Connection=SSPI", "Users"));


builder.Services.AddSingleton(employeeSystem);
builder.Services.AddSingleton(userSystem);

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
