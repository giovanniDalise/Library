using Library.IdentityService.Core.Domain.Services;
using Library.IdentityService.Core.Ports;
using Library.IdentityService.Infrastructure.Adapters;
using Library.IdentityService.Infrastructure.Adapters.Jwt;
using Library.IdentityService.Infrastructure.Adapters.Repository;
using Library.IdentityService.Infrastructure.Adapters.Security;
using Library.Logging.Abstractions;
using Library.Logging.NLog;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi NLogAdapter come ILoggerPort
builder.Services.AddSingleton<ILoggerPort>(_ => new NLogAdapter("UserService"));
builder.Services.AddSingleton<ILoggerPort>(_ => new NLogAdapter("AuthenticationService"));

var configuration = builder.Configuration;

// Registrazione dei servizi necessari per il BookService
builder.Services.AddScoped<IUserRepositoryPort, UserRepositoryAdapter>();  // Registrazione dell'interfaccia e dell'implementazione
builder.Services.AddScoped<IUserServicePort, UserService>();  // Registrazione del BookService
builder.Services.AddScoped<IPasswordHasherPort, BCryptPasswordHasherAdapter>(); // Assicurati di avere questa implementazione disponibile
builder.Services.AddScoped<IPasswordVerifierPort, BCryptPasswordVerifierAdapter>();
builder.Services.AddScoped<IAuthenticationRepositoryPort, AuthRepositoryAdapter>();
builder.Services.AddScoped<IJwtPort>(provider =>
{
    var jwtExpirationMs = configuration.GetValue<double>("JwtSettings:ExpirationMilliseconds");
    var logger = provider.GetRequiredService<ILoggerPort>(); // recupero il logger dal DI
    return new JwtAdapter(configuration, jwtExpirationMs, logger);
});
builder.Services.AddScoped<IAuthenticationServicePort, AuthenticationService>();

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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Usa CORS prima di UseRouting
app.UseCors("AllowAllOrigins");

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
