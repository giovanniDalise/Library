using Library.IdentityService.Core.Application;
using Library.IdentityService.Core.Domain.Services;
using Library.IdentityService.Core.Ports;
using Library.IdentityService.Infrastructure.Adapters;
using Library.IdentityService.Infrastructure.Adapters.Events;
using Library.IdentityService.Infrastructure.Adapters.Jwt;
using Library.IdentityService.Infrastructure.Adapters.Repository;
using Library.IdentityService.Infrastructure.Adapters.Security;
using Library.Logging.Abstractions;
using Library.Logging.NLog;
using Library.Security;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Logger per classe: ogni classe chiede ILoggerPort<NomeClasse>
builder.Services.AddSingleton(typeof(ILoggerPort<>), typeof(NLogAdapter<>));

var configuration = builder.Configuration;

builder.Services.AddScoped<IUserRepositoryPort, UserRepositoryAdapter>();
builder.Services.AddScoped<IUserAppServicePort, UserAppService>();
builder.Services.AddScoped<IUserServicePort, UserService>();
builder.Services.AddScoped<IPasswordHasherPort, BCryptPasswordHasherAdapter>();
builder.Services.AddScoped<IPasswordVerifierPort, BCryptPasswordVerifierAdapter>();
builder.Services.AddScoped<IAuthenticationRepositoryPort, AuthRepositoryAdapter>();
builder.Services.AddScoped<IJwtPort>(provider =>
{
    var jwtExpirationMs = configuration.GetValue<double>("JwtSettings:ExpirationMilliseconds");
    var logger = provider.GetRequiredService<ILoggerPort<JwtAdapter>>(); // <-- CAMBIATA
    return new JwtAdapter(configuration, jwtExpirationMs, logger);
});
builder.Services.AddScoped<IAuthenticationServicePort, AuthenticationService>();

builder.Services.AddSingleton<IEventPublisherPort>(sp =>
{
    var logger = sp.GetRequiredService<ILoggerPort<RabbitMQEventPublisher>>(); // <-- CAMBIATA
    var hostName = builder.Configuration["RabbitMQ:HostName"] ?? "localhost";
    var userName = builder.Configuration["RabbitMQ:UserName"] ?? "guest";
    var password = builder.Configuration["RabbitMQ:Password"] ?? "guest";
    return RabbitMQEventPublisher.CreateAsync(hostName, userName, password, logger)
        .GetAwaiter().GetResult();
});

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
builder.Services.AddLibraryJwtAuthentication(configuration); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Inserisci il token JWT"
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

// Usa CORS prima di UseRouting
app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();