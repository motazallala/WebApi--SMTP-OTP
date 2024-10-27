using Exatek.RegistrationApi.Services.Interfase;

namespace Exatek.RegistrationApi.Services;

public class PhoneSenderService : IPhoneSenderService
{
    public async Task SendPhoneAsync(string phoneNumber, string message)
    {
        // Send phone message
    }
}
