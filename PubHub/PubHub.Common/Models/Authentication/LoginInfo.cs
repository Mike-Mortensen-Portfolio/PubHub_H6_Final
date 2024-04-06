using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.Models.Authentication
{
    public class LoginInfo
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
