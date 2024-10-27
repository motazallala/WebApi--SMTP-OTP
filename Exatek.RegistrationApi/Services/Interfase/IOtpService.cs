namespace Exatek.RegistrationApi.Services.Interfase;

public interface IOtpService
{
    Task SaveOtp(string key, string otp);
    public bool TryGetOtp(string key, out string otp);
    public void RemoveOtp(string key);

}
