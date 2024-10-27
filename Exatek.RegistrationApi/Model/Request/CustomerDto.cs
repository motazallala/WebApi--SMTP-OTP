namespace Exatek.RegistrationApi.Model.Request;

public class CustomerDto
{
    public string ICNumber { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool PhoneVerify { get; set; } = false;
    public bool EmailVerify { get; set; } = false;
    public bool Policy { get; set; } = false;
    public bool biometric { get; set; } = false;
}
