namespace AuctionSys.Application.Interfaces.Auth;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otp);
}
