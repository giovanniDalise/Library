using Library.BookService.Core.Domain.Services;
using Library.BookService.Core.Ports;
using Library.BookService.Infrastructure.Adapters;
using Library.BookService.Infrastructure.Persistence;
using Library.BookService.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Registrazione dei servizi necessari per il BookService
builder.Services.AddScoped<BookRepositoryPort, BookRepositoryEF>();  // Registrazione dell'interfaccia e dell'implementazione
builder.Services.AddScoped<BookServicePort, BookService>();  // Registrazione del BookService

// Aggiungi DbContext per la connessione a MySQL
var connectionString = builder.Configuration.GetConnectionString("BookDbConnection");
builder.Services.AddDbContext<BookDBContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registrazione del BookMapper
builder.Services.AddScoped<BookMapper>();  // Registrazione di BookMapper nel DI

// Aggiungi i servizi per i controller
builder.Services.AddControllers();  // Aggiungi questa riga per registrare i controller

// Configura CORS: consenti tutte le origini (puoi restringerlo specificamente se necessario)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Swagger configuration (optional, for API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usa CORS prima di UseRouting
app.UseCors("AllowAllOrigins");  // Usa il middleware CORS qui

// Aggiungi il middleware per il routing e i controller
app.UseRouting();

app.MapControllers();  // Assicurati che i controller vengano mappati correttamente

app.Run();
