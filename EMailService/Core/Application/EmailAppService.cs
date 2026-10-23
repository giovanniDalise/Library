
using Library.EmailService.Core.Domain.Events;
using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;

namespace Library.EmailService.Core.Application
{
    public class EmailAppService : IEmailAppServicePort
    {
        private readonly IEmailServicePort _emailService;
        private readonly ILoggerPort _logger;
        private readonly string _frontendBaseUrl;

        public EmailAppService(
            IEmailServicePort emailService,
            ILoggerPort logger,
            IConfiguration configuration)
        {
            _emailService = emailService;
            _logger = logger;
            _frontendBaseUrl = configuration["App:FrontendBaseUrl"] ?? "http://localhost:4200";
        }

        public async Task SendConfirmationEmailAsync(UserRegisteredEvent @event)
        {
            _logger.Info($"EmailAppService - SendConfirmationEmailAsync | UserId: {@event.UserId}");
            var confirmUrl = $"{_frontendBaseUrl}/confirm-email?token={@event.ConfirmationToken}";
            await _emailService.SendConfirmationEmailAsync(
                @event.Email, @event.FullName, confirmUrl, @event.ExpiresAt);
            _logger.Info($"EmailAppService - Completed | Email: {@event.Email}");
        }

        public async Task SendResetPasswordEmailAsync(PasswordResetEvent @event)
        {
            _logger.Info($"EmailAppService - SendResetPasswordEmailAsync | Email: {@event.Email}");
            var resetUrl = $"{_frontendBaseUrl}/reset-password?token={@event.ResetToken}";
            await _emailService.SendResetPasswordEmailAsync(
                @event.Email, @event.FullName, resetUrl, @event.ExpiresAt);
            _logger.Info($"EmailAppService - Completed | Email: {@event.Email}");
        }
        public async Task SendPasswordResetConfirmEmailAsync(PasswordResetConfirmEvent @event)
        {
            _logger.Info($"EmailAppService - SendPasswordResetConfirmAsync | Email: {@event.Email}");
            await _emailService.SendPasswordResetConfirmEmailAsync(@event.Email, @event.FullName, @event.ChangedAt);
            _logger.Info($"EmailAppService - Completed | Email: {@event.Email}");
        }
    }
}