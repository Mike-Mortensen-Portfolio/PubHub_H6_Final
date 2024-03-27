using Microsoft.AspNetCore.Identity;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Identity
{
    public sealed class Account : IdentityUser<int>
    {
        private readonly UpperInvariantLookupNormalizer _normalizer = new();

        public required int AccountTypeId { get; set; }
        /// <summary>
        /// Gets or sets the email for this user.
        /// <br/>
        /// <br/><strong>Overridden to additonally set:</strong>
        /// <br/>NormalizedEmail
        /// <br/>UserName
        /// <br/>NormalizedUserName
        /// </summary>
        new public required string Email
        {
            get
            {
                return base.Email!;
            }

            set
            {
                if (value is null)
                    throw new InvalidOperationException($"{nameof(Email)} can't be null");

                base.Email = value;
                base.NormalizedEmail = _normalizer.NormalizeEmail(value);

                base.UserName = base.Email;
                base.NormalizedUserName = NormalizedEmail;
            }
        }
        /// <summary>
        /// Gets the normalized email for this user
        /// </summary>
        public override string NormalizedEmail
        {
            get
            {
                return base.NormalizedEmail!;
            }
        }
        /// <summary>
        /// Gets the user name for this user
        /// </summary>
        public override string UserName
        {
            get
            {
                return base.UserName!;
            }
        }
        /// <summary>
        /// Gets the normalized user name for this user
        /// </summary>
        public override string NormalizedUserName
        {
            get
            {
                return base.NormalizedUserName!;
            }
        }
        public DateTime? LastSignIn { get; set; }
        public bool IsDeleted { get; set; }

        #region Navs
        public AccountType AccountType { get; set; } = null!;
        #endregion
    }
}
