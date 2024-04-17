using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PubHub.API.Domain.Auth
{
    public class AuthOptions
    {
        public required IEnumerable<string> Issuers { get; init; }
        public required int Lifetime { get; init; }
        public required int RefreshLifetime { get; init; }
        public required IEnumerable<string> Audiences { get; init; }
        public required string Key {  get; init; }

        public SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(Key));
    }
}
