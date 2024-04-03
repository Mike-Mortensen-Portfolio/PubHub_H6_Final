using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.Models.Authentication
{
    public class TokenInfo
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
