using Exatek.RegistrationApi.Model.Request;
using Exatek.RegistrationApi.Model.Response;
using Exatek.RegistrationApi.Services.Interfase;
using Exatek.RegistrationCore.Model;
using Exatek.RegistrationEF.AppData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Exatek.RegistrationApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IPhoneSenderService _phoneSenderService;
    private readonly IOtpService _otpService;
    public LoginController(AppDbContext context, IPhoneSenderService phoneSenderService, IOtpService otpService, IEmailService emailService)
    {
        _context = context;
        _phoneSenderService = phoneSenderService;
        _emailService = emailService;
        _otpService = otpService;
    }


    // POST: api/Registration
    [HttpPost]
    public async Task<ActionResult<UserInfo>> Login(LoginDto loginData)
    {
        var user = await _context.Customers.FindAsync(loginData.ICNumber);
        if (user is null)
        {
            return BadRequest("User not exist please register");
        }
        if (_otpService.TryGetOtp(user.PhoneNumber, out var oldOtp))
        {
            return BadRequest("The OTP has been sended");
        }
        var otp = new Random().Next(1000, 9999).ToString(); // Generates a 4-digit OTP

        // Save the OTP with timestamp in a temporary in-memory cache
        await _otpService.SaveOtp(user.PhoneNumber, otp);
        // This service is demo change it to your actual service inside the implementation of PhoneSenderService
        await _phoneSenderService.SendPhoneAsync(user.PhoneNumber, $"Welcome to Exatek your Otp : {otp}");
        return Ok(new UserInfo
        {
            ICNumber = user.ICNumber,
            Message = "** ** ***" + user.PhoneNumber.Substring(user.PhoneNumber.Length - 4),
            //This will be removed in the production version
            Otp = otp
        });
    }

    // POST: api/Registration/VerifyPhone
    [HttpPost("VerifyPhone")]
    public async Task<ActionResult<UserInfo>> VerifyPhone(VerifyDto verifyData)
    {
        var user = await _context.Customers.FindAsync(verifyData.ICNumber);
        if (user is null)
        {
            return BadRequest("User not exist please register");
        }
        if (!_otpService.TryGetOtp(user.PhoneNumber, out var otp))
        {
            return BadRequest("OTP not found or expired");
        }
        if (otp != verifyData.Otp)
        {
            return BadRequest("Invalid OTP");
        }
        _otpService.RemoveOtp(user.PhoneNumber);
        if (_otpService.TryGetOtp(user.Email, out var oldOtp))
        {
            return BadRequest("The OTP has been sended");
        }
        var otpForEmail = new Random().Next(100000, 999999).ToString(); // Generates a 4-digit OTP

        // Save the OTP with timestamp in a temporary in-memory cache
        await _otpService.SaveOtp(user.Email, otpForEmail);
        string emailBody = _emailService.GenerateOtpEmailTemplate(user.Name, otpForEmail);
        await _emailService.SendEmailAsync(user.Email, "Otp Verification", emailBody);
        // Find the position of the '@' sign
        int atIndex = user.Email.IndexOf('@');

        // Replace the third character up to the '@' with '*'
        string maskedEmail = user.Email.Substring(0, 2)
                            + new string('*', atIndex - 2)
                            + user.Email.Substring(atIndex, 1) // '@' sign
                            + "***.com";
        return Ok(new UserInfo
        {
            ICNumber = user.ICNumber,
            Message = maskedEmail,
            //This will be removed in the production version
            Otp = otpForEmail
        });
    }

    // POST: api/Registration/VerifyEmail
    [HttpPost("VerifyEmail")]
    public async Task<ActionResult<UserInfo>> VerifyEmail(VerifyDto verifyData)
    {
        var user = await _context.Customers.FindAsync(verifyData.ICNumber);
        if (user is null)
        {
            return BadRequest("User not exist please register");
        }
        if (!_otpService.TryGetOtp(user.Email, out var otp))
        {
            return BadRequest("OTP not found or expired");
        }
        if (otp != verifyData.Otp)
        {
            return BadRequest("Invalid OTP");
        }
        _otpService.RemoveOtp(user.Email);
        return Ok(new UserInfo
        {
            ICNumber = user.ICNumber,
            Message = user.Email,
            //This will be removed in the production version
            Otp = otp
        });
    }
}
