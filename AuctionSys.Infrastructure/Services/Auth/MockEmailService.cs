using AuctionSys.Application.Interfaces.Auth;
using Microsoft.Extensions.Logging;

namespace AuctionSys.Infrastructure.Services.Auth;

public class MockEmailService : IEmailService
{
    private readonly ILogger<MockEmailService> _logger;

    public MockEmailService(ILogger<MockEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendOtpEmailAsync(string toEmail, string otp)
    {
        // Mock sending email
        _logger.LogInformation("===============================================");
        _logger.LogInformation("MOCK EMAIL SENT TO: {Email}", toEmail);
        _logger.LogInformation("MÃ OTP CỦA BẠN LÀ: {Otp}", otp);
        _logger.LogInformation("===============================================");
        
        return Task.CompletedTask;
    }
}
