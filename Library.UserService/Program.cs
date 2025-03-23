using Library.UserService.Core.Domain.Services;
using Library.UserService.Core.Ports;
using Library.UserService.Infrastructure.Adapters;

var builder = WebApplication.CreateBuilder(args);

// Registrazione dei servizi necessari per il BookService
builder.Services.AddScoped<UserRepositoryPort, UserRepositoryAdapter>();  // Registrazione dell'interfaccia e dell'implementazione
builder.Services.AddScoped<UserServicePort, UserService>();  // Registrazione del BookService

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Usa CORS prima di UseRouting
app.UseCors("AllowAll");

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
