namespace PubHub.API.Domain.Auth
{
    public class WhitelistOptions
    {
        public required ICollection<AppWhitelist> Apps { get; init; }
    }
}
