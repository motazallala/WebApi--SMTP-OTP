namespace Exatek.RegistrationApi.Services.Interfase;

public interface IPhoneSenderService
{
    Task SendPhoneAsync(string phoneNumber, string message);
}
