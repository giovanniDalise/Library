using Library.AuthenticationService.Core.Ports;
using Library.AuthenticationService.Core.Domain.Services;
using Library.AuthenticationService.Infrastructure.Adapters.Jwt;
using Library.AuthenticationService.Infrastructure.Adapters.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Library.BookService.Core.Ports;
using Library.AuthenticationService.Infrastructure.Adapters.Security;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Registrazione dei servizi
builder.Services.AddScoped<IPasswordVerifierPort, BCryptPasswordVerifierAdapter>();
builder.Services.AddScoped<AuthenticationRepositoryPort, AuthRepositoryAdapter>();
builder.Services.AddScoped<JwtPort>(provider =>
{
    var jwtExpirationMs = configuration.GetValue<double>("JwtSettings:ExpirationMilliseconds");
    return new JwtAdapter(configuration, jwtExpirationMs);
});
builder.Services.AddScoped<AuthenticationServicePort, AuthenticationService>();

// Configurazione CORS per permettere tutte le richieste (debug)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Aggiunta dei controller
builder.Services.AddControllers().AddApplicationPart(typeof(Library.AuthenticationService.Infrastructure.Adapters.Controller.AuthController).Assembly);

// Configurazione Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurazione middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Abilitazione CORS
app.UseCors("AllowAllOrigins");

// Routing e autorizzazione
app.UseRouting();
app.UseAuthorization();

// Mapping dei controller
app.MapControllers();

app.Run();