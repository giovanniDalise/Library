using Library.EmailService.Core.Ports;
using Library.EmailService.Infrastructure.Adapters.RabbitMQ;
using Library.Logging.Abstractions;
using Library.Logging.NLog;
using Library.MailService.Core.Application;
using Library.MailService.Infrastructure.Adapters.SMTP;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ILoggerPort>(_ => new NLogAdapter("EmailService"));

builder.Services.AddScoped<IEmailAppServicePort, EmailAppService>();
builder.Services.AddSingleton<IEmailSenderPort>(sp =>
{
    var logger = sp.GetRequiredService<ILoggerPort>();
    var fromEmail = builder.Configuration["Gmail:FromEmail"]!;
    var appPassword = builder.Configuration["Gmail:AppPassword"]!;
    return new GmailEmailSender(fromEmail, appPassword, logger);
});

var rabbitHost = builder.Configuration["RabbitMQ:HostName"] ?? "localhost";
builder.Services.AddSingleton(sp =>
    new UserRegisteredConsumer(
        sp.GetRequiredService<IEmailAppServicePort>(),
        sp.GetRequiredService<ILoggerPort>(),
        rabbitHost));

builder.Services.AddHostedService(sp => sp.GetRequiredService<UserRegisteredConsumer>());

var app = builder.Build();
app.Run();