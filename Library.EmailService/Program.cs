using Library.EmailService.Core.Domain.Services;
using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;
using Library.Logging.NLog;
using Library.MailService.Infrastructure.Adapters.SMTP;
using Library.MailService.Infrastructure.Options;
using Library.EmailService.Core.Application;
using Library.EmailService.Infrastructure.Adapters.RabbitMQ;

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
builder.Services.AddSingleton<IEmailServicePort, EmailService>();
builder.Services.AddSingleton<IEmailAppServicePort, EmailAppService>();

var rabbitHost = builder.Configuration["RabbitMQ:HostName"] ?? "localhost";
var rabbitUser = builder.Configuration["RabbitMQ:UserName"] ?? "guest";
var rabbitPassword = builder.Configuration["RabbitMQ:Password"] ?? "guest";

builder.Services.AddSingleton(sp =>
    new UserRegisteredConsumer(
        sp.GetRequiredService<IEmailAppServicePort>(),
        sp.GetRequiredService<ILoggerPort>(),
        rabbitHost,
        rabbitUser,
        rabbitPassword));

builder.Services.AddHostedService(sp => sp.GetRequiredService<UserRegisteredConsumer>());

var app = builder.Build();
app.Run();