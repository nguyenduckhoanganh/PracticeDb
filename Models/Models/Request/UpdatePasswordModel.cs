using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Models.Request;
public class ResetPasswordModel
{
    public long CustomerId { get; set; }
    public string Password { get; set; }
}

public class UpdatePasswordModel
{
    public long CustomerId { get; set; }
    public string Password { get; set; }
    public string OldPassword { get; set; }
}
