using Exatek.RegistrationApi.Services.Config;
using Exatek.RegistrationApi.Services.Interfase;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Exatek.RegistrationApi.Services;

public class EmailService : IEmailService
{
    private readonly MailConfig _mailSettings;
    public EmailService(IOptions<MailConfig> mailSettings)
    {
        _mailSettings = mailSettings.Value;
    }

    public async Task SendEmailAsync(string mailTo, string subject, string body, IList<IFormFile> attachments = null)
    {
        var email = new MimeMessage()
        {
            Sender = MailboxAddress.Parse(_mailSettings.Email),
            Subject = subject
        };

        email.To.Add(MailboxAddress.Parse(mailTo));
        var builder = new BodyBuilder();

        if (attachments != null)
        {
            foreach (var file in attachments)
            {
                if (file.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                }
            }
        }

        builder.HtmlBody = body;
        email.Body = builder.ToMessageBody();
        email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Email));

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.SslOnConnect);
        await smtp.AuthenticateAsync(_mailSettings.Email, _mailSettings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public string GenerateOtpEmailTemplate(string customerName, string otpCode)
    {
        return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
            color: #333;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #ffffff;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #4CAF50;
            padding: 20px;
            text-align: center;
            border-radius: 8px 8px 0 0;
            color: #ffffff;
            font-size: 24px;
        }}
        .content {{
            padding: 20px;
            font-size: 16px;
            line-height: 1.5;
            color: #333333;
        }}
        .otp {{
            font-size: 24px;
            font-weight: bold;
            color: #4CAF50;
            margin: 20px 0;
            text-align: center;
        }}
        .footer {{
            padding: 10px;
            text-align: center;
            color: #888888;
            font-size: 14px;
        }}
        .button {{
            display: inline-block;
            padding: 12px 24px;
            background-color: #4CAF50;
            color: #ffffff;
            border-radius: 4px;
            text-decoration: none;
            font-size: 16px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            Secure OTP Verification
        </div>
        <div class='content'>
            <p>Dear {customerName},</p>
            <p>To complete your request, please use the One-Time Password (OTP) below to verify your account:</p>
            <div class='otp'>{otpCode}</div>
            <p>If you did not request this OTP, please ignore this email or contact our support team immediately.</p>
            <p>Thank you for choosing our services!</p>
            <p>Best regards, <br> The Exatek Team</p>
        </div>
        <div class='footer'>
            &copy; {DateTime.Today.Year} Exatek. All rights reserved.
        </div>
    </div>
</body>
</html>";
    }
}
