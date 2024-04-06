using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.Models.Authentication
{
    public class TokenInfo
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
