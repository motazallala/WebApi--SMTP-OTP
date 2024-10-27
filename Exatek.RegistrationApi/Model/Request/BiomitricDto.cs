namespace Exatek.RegistrationApi.Model.Request;

public class BiomitricDto
{
    public string ICNumber { get; set; }
    public bool BiometricStatas { get; set; } = false;
}
