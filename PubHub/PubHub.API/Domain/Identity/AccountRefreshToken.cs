namespace PubHub.API.Domain.Identity
{
    public class AccountRefreshToken
    {
        public required Guid AccountId { get; set; }
        public required string Value { get; set; }
        public required DateTime Expiration { get; set; }

        #region Navs
        public Account Account { get; set; } = null!;
        #endregion
    }
}
