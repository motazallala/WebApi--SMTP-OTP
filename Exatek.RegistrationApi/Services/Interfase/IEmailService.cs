namespace Exatek.RegistrationApi.Services.Interfase;

public interface IEmailService
{
    Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachment = null);
    string GenerateOtpEmailTemplate(string customerName, string otpCode);
}
