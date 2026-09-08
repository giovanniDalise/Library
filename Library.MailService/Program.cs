using Library.EmailService.Core.Domain.Services;
using Library.EmailService.Core.Ports;
using Library.EmailService.Infrastructure.Adapters.RabbitMQ;
using Library.Logging.Abstractions;
using Library.Logging.NLog;
using Library.MailService.Core.Application;
using Library.MailService.Infrastructure.Adapters.SMTP;
using Library.MailService.Infrastructure.Options;
using MailKit;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.local.json", optional: true)
    .AddEnvironmentVariables();

var emailOptions = builder.Configuration
    .GetSection("Email")
    .Get<EmailOptions>()!;

builder.Services.AddSingleton<ILoggerPort>(_ => new NLogAdapter("MailService"));
builder.Services.AddSingleton(emailOptions);
builder.Services.AddSingleton<IEmailSenderPort>(sp =>
    new SmtpEmailSender(emailOptions, sp.GetRequiredService<ILoggerPort>()));
builder.Services.AddScoped<IEmailServicePort, EmailService>();
builder.Services.AddScoped<IEmailAppServicePort, EmailAppService>();

var rabbitHost = builder.Configuration["RabbitMQ:HostName"] ?? "localhost";
builder.Services.AddSingleton(sp =>
    new UserRegisteredConsumer(
        sp.GetRequiredService<IEmailAppServicePort>(),
        sp.GetRequiredService<ILoggerPort>(),
        rabbitHost));
builder.Services.AddHostedService(sp => sp.GetRequiredService<UserRegisteredConsumer>());

var app = builder.Build();
app.Run();