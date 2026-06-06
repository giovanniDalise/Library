using Library.BookService.Core.Application;
using Library.BookService.Core.Domain.Services;
using Library.BookService.Core.Ports;
using Library.BookService.Core.Ports.Authors;
using Library.BookService.Core.Ports.Books;
using Library.BookService.Core.Ports.Editors;
using Library.BookService.Infrastructure.Adapters;
using Library.BookService.Infrastructure.Adapters.Authors;
using Library.BookService.Infrastructure.Adapters.Books;
using Library.BookService.Infrastructure.Adapters.Editors;
using Library.BookService.Infrastructure.Persistence.EF;
using Library.BookService.Infrastructure.Persistence.EF.Mappers;
using Library.BookService.Security;
using Library.Logging.Abstractions;
using Library.Logging.NLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;


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

builder.Services.AddScoped<IAuthorRepositoryPort, AuthorRepositoryEF>();
builder.Services.AddScoped<IAuthorAppServicePort, AuthorAppService>();
builder.Services.AddScoped<IAuthorServicePort, AuthorService>();

// Aggiungi DbContext per la connessione a MySQL
var connectionString = builder.Configuration.GetConnectionString("BookDbConnection");
builder.Services.AddDbContext<BookDBContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Registrazione del BookMapper
builder.Services.AddScoped<BookEntityMapper>();  // Registrazione di BookMapper nel DI
builder.Services.AddScoped<EditorEntityMapper>();  // Registrazione di EditorMapper nel DI
builder.Services.AddScoped<AuthorEntityMapper>();  // Registrazione di AuthorMapper nel DI


builder.Services.AddControllers();

builder.Services.ConfigureSecurity(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Book Service API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Inserisci il token JWT nel formato: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
var imagesPath = builder.Configuration["Media:BasePath"] ?? throw new Exception("Media:BasePath not configured");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagesPath),
    RequestPath = "/images"
});
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();