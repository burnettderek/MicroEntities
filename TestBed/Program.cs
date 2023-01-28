using Microsoft.Extensions.Configuration;
using FluentValidation;
using MicroEntities.Application;
using MicroEntities.Data.SqlServer;
using MicroEntities.Utils;
using MicroEntities.Validation;
using TestBed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var connectionString = config.GetValue<string>("DatabaseConnectionString");
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
									.SetMinimumLevel(LogLevel.Trace)
									.AddDebug()
									.AddConsole());

var customerSystem = new PublicEntitySystem<CustomerDto, Customer>();
customerSystem
.AddLayer(new BenchmarkingLayer<Customer>(loggerFactory))
.AddLayer(new FluentValidationLayer<Customer>(validator =>
{
	validator.RuleFor(employee => employee.FirstName).Length(1, 50).Matches("^[a-zA-Z0-9'., -]*$");
	validator.RuleFor(employee => employee.LastName).Length(1, 50).Matches("^[a-zA-Z0-9-]*$");
	validator.RuleFor(user => user.Balance).GreaterThanOrEqualTo(0);
}))
.AddLayer(new SqlServerSystemLayer<Customer>(loggerFactory, connectionString, SchemaMode.CodeFirst, "Customers"));

var orderSystem = new PublicEntitySystem<OrderDto, Order>();
orderSystem
.AddLayer(new BenchmarkingLayer<Order>(loggerFactory))
.AddLayer(new SqlServerSystemLayer<Order>(loggerFactory, connectionString, SchemaMode.CodeFirst, "Orders"));

var itemSystem = new PublicEntitySystem<ItemDto, Item>();
itemSystem
.AddLayer(new BenchmarkingLayer<Item>(loggerFactory))
.AddLayer(new SqlServerSystemLayer<Item>(loggerFactory, connectionString, SchemaMode.CodeFirst, "Items"));

/*for(int i = 0; i < 10000; i++)
{
	var balance = (decimal)(new Random().NextDouble()) * 100M;
	var customer = new CustomerDto() { FirstName = Guid.NewGuid().ToString(), LastName = Guid.NewGuid().ToString(), Balance = balance };
	await customerSystem.Create(customer);
}*/


builder.Services.AddSingleton(orderSystem);
builder.Services.AddSingleton(customerSystem);
builder.Services.AddSingleton(itemSystem);

builder.Services.AddControllers();
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
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
