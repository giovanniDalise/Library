using Library.BookService.Core.Application;
using Library.BookService.Core.Domain.Services;
using Library.BookService.Core.Ports;
using Library.BookService.Core.Ports.Books;
using Library.BookService.Core.Ports.Editors;
using Library.BookService.Infrastructure.Adapters;
using Library.BookService.Infrastructure.Adapters.Books;
using Library.BookService.Infrastructure.Adapters.Editors;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.BookService.Security;
using Library.Logging.Abstractions;
using Library.Logging.NLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);

// Aggiungi NLogAdapter come ILoggerPort
builder.Services.AddSingleton<ILoggerPort>(_ => new NLogAdapter("BookService"));


// Registrazione dei servizi necessari per il BookService
builder.Services.AddScoped<IBookRepositoryPort, BookRepositoryEF>();
builder.Services.AddScoped<IBookAppServicePort, BookAppService>();
builder.Services.AddScoped<IBookServicePort, BookService>();

builder.Services.AddScoped<IMediaStoragePort, FileSystemMediaStorageAdapter>();

builder.Services.AddScoped<IEditorRepositoryPort, EditorRepositoryEF>();
builder.Services.AddScoped<IEditorAppServicePort, EditorAppService>();
builder.Services.AddScoped<IEditorServicePort, EditorService>();

// Aggiungi DbContext per la connessione a MySQL
var connectionString = builder.Configuration.GetConnectionString("BookDbConnection");
builder.Services.AddDbContext<BookDBContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registrazione del BookMapper
builder.Services.AddScoped<BookEntityMapper>();  // Registrazione di BookMapper nel DI
builder.Services.AddScoped<EditorEntityMapper>();  // Registrazione di EditorMapper nel DI


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

var imagesPath = builder.Configuration["Media:BasePath"]
    ?? throw new Exception("Media:BasePath not configured");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
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
