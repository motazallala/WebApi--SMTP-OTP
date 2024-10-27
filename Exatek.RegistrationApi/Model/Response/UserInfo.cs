namespace Exatek.RegistrationApi.Model.Response;

public class UserInfo
{
    public string ICNumber { get; set; }
    public string Message { get; set; }
    //This will be removed in the production version
    public string Otp { get; set; }
}
