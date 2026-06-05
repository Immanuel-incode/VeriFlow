using Api.Repositories;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllers();
builder.Services.AddSingleton(new TransactionRepository(connectionString!));

var app = builder.Build();
app.MapControllers();
app.Run();