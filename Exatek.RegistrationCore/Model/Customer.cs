using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exatek.RegistrationCore.Model;
public class Customer
{
    public string ICNumber { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool PhoneVerify { get; set; }
    public bool EmailVerify { get; set; }
    public bool Policy { get; set; }
    public bool biometric { get; set; }
}
