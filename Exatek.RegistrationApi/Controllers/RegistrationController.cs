using Exatek.RegistrationApi.Model.Request;
using Exatek.RegistrationApi.Model.Response;
using Exatek.RegistrationApi.Services.Interfase;
using Exatek.RegistrationCore.Model;
using Exatek.RegistrationEF.AppData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exatek.RegistrationApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RegistrationController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IPhoneSenderService _phoneSenderService;
    private readonly IOtpService _otpService;
    public RegistrationController(AppDbContext context, IPhoneSenderService phoneSenderService, IOtpService otpService, IEmailService emailService)
    {
        _context = context;
        _phoneSenderService = phoneSenderService;
        _emailService = emailService;
        _otpService = otpService;
    }





    [HttpPost]
    public async Task<ActionResult<UserInfo>> PostUser(Customer user)
    {
        _context.Customers.Add(user);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            if (UserExists(user.ICNumber))
            {
                return Conflict("There is an Conflict");
            }
            else
            {
                throw;
            }
        }
        if (_otpService.TryGetOtp(user.PhoneNumber, out var oldOtp))
        {
            return BadRequest("The OTP has been sended");
        }
        var otp = new Random().Next(1000, 9999).ToString(); // Generates a 4-digit OTP

        // Save the OTP with timestamp in a temporary in-memory cache
        await _otpService.SaveOtp(user.PhoneNumber, otp);
        return Ok(new UserInfo
        {
            ICNumber = user.ICNumber,
            Message = user.PhoneNumber,
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
        await _emailService.SendEmailAsync(user.Email, "Otp Verification", $"Your Otp is : {otpForEmail}");
        user.PhoneVerify = true;
        await _context.SaveChangesAsync();
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Conflict("There is an Conflict");
        }
        return Ok(new UserInfo
        {
            ICNumber = user.ICNumber,
            Message = user.Email,
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
        user.EmailVerify = true;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return Conflict("There is an Conflict");
        }
        return Ok(new UserInfo
        {
            ICNumber = user.ICNumber,
            Message = user.Email,
            //This will be removed in the production version
            Otp = otp
        });
    }

    private bool UserExists(string id)
    {
        return _context.Customers.Any(e => e.ICNumber == id);
    }
}