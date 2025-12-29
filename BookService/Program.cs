using Library.BookService.Core.Domain.Services;
using Library.BookService.Core.Ports;
using Library.BookService.Infrastructure.Adapters;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.BookService.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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

// Configura i servizi di sicurezza (JWT, CORS, etc.)
builder.Services.ConfigureSecurity(builder.Configuration);

// Swagger configuration (optional, for API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurazione middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        @"C:\LibraryMKNW"
    ),
    RequestPath = "/images"
});


// Configura routing
app.UseRouting();

// Usa CORS prima di UseRouting
app.UseCors("AllowAll");

// Abilita il middleware JWT
app.UseMiddleware<JwtMiddleware>();

// Abilita l'autenticazione e l'autorizzazione
app.UseAuthentication();
app.UseAuthorization();



app.MapControllers();  // Assicurati che i controller vengano mappati correttamente

app.Run();
