using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PubHub.Common.Models.Accounts
{
    public class AccountLoginModel
    {
        [FromHeader, Required]
        public required string Email { get; set; }
        [FromHeader, Required]
        public required string Password { get; set; }
    }
}
