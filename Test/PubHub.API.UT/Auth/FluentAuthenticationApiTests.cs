using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NSubstitute;
using PubHub.API.Domain.Auth;
using PubHub.API.Domain.Services;
using PubHub.API.UT.Utilities;
using PubHub.Common;

namespace PubHub.API.UT.Auth
{
    public class FluentAuthenticationApiTests : IClassFixture<DatabaseFixture>
    {
        private const string UNRECOGNIZED = "Unrecognized";

        private const string PAPERS_CONTROLLER = "PapersController";
        private const string PEACHES_CONTROLLER = "PeachesController";
        private const string TRAINS_CONTROLLER = "TrainsController";

        private const string PAPERS_ENDPOINT_DRAW = "Draw";
        private const string PAPERS_ENDPOINT_FOLD = "Fold";
        private const string PAPERS_ENDPOINT_TEAR = "Tear";
        private const string PAPERS_ENDPOINT_THROW = "Throw";

        private const string PEACHES_ENDPOINT_PLUCK = "Pluck";
        private const string PEACHES_ENDPOINT_EAT = "Eat";

        private const string TRAINS_ENDPOINT_CHOO = "Choo";
        private const string TRAINS_ENDPOINT_CHUG = "Chug";
        private const string TRAINS_ENDPOINT_CHUFF = "Chuff";

        private static readonly string[] PAPERS_ENDPOINTS_ONE = [PAPERS_ENDPOINT_DRAW, PAPERS_ENDPOINT_FOLD];
        private static readonly string[] PAPERS_ENDPOINTS_TWO = [PAPERS_ENDPOINT_TEAR, PAPERS_ENDPOINT_FOLD, PAPERS_ENDPOINT_THROW];
        private static readonly string[] PEACHES_ENDPOINTS = [PEACHES_ENDPOINT_PLUCK, PEACHES_ENDPOINT_EAT];
        private static readonly string[] TRAINS_ENDPOINTS = [TRAINS_ENDPOINT_CHOO, TRAINS_ENDPOINT_CHUG, TRAINS_ENDPOINT_CHUFF];

        private readonly AccessService _accessService;

        private readonly Guid _subjectGuid = Guid.NewGuid();
        private readonly string _appIdOne = Guid.NewGuid().ToString();
        private readonly string _appIdTwo = Guid.NewGuid().ToString();

        public FluentAuthenticationApiTests(DatabaseFixture databaseFixture)
        {
            DatabaseFixture = databaseFixture;

            // Build whitelist.
            var whitelistOptions = new WhitelistOptions()
            {
                Apps = new List<AppWhitelist>()
                {
                    new()
                    {
                        AppId = _appIdOne,
                        Subjects = new List<string>
                        {
                            "Publisher",
                            "Operator"
                        },
                        ControllerEndpoints = new Dictionary<string, IEnumerable<string>>
                        {
                            { PAPERS_CONTROLLER, PAPERS_ENDPOINTS_ONE },
                            { PEACHES_CONTROLLER, PEACHES_ENDPOINTS },
                            { TRAINS_CONTROLLER, TRAINS_ENDPOINTS }
                        }
                    },
                    new()
                    {
                        AppId = _appIdTwo,
                        Subjects = new List<string>
                        {
                            "User"
                        },
                        ControllerEndpoints = new Dictionary<string, IEnumerable<string>>
                        {
                            { PAPERS_CONTROLLER, PAPERS_ENDPOINTS_TWO },
                            { PEACHES_CONTROLLER, PEACHES_ENDPOINTS }
                        }
                    }
                }
            };
            var options = Substitute.For<IOptions<WhitelistOptions>>();
            options.Value.Returns(whitelistOptions);

            // Create services.
            TypeLookupService typeLookupService = new(Context);
            _accessService = new AccessService(options, typeLookupService);

            // Create user.
            User = new ClaimsPrincipal(new ClaimsIdentity());
            Identity = (ClaimsIdentity)User.Identity!;
            Identity.AddClaim(new(TokenClaimConstants.ID, _subjectGuid.ToString()));
        }

        public ClaimsPrincipal User { get; init; }
        public ClaimsIdentity Identity { get; init; }

        protected DatabaseFixture DatabaseFixture { get; }
        protected PubHubContext Context => DatabaseFixture.Context;

        /// <summary>
        /// Set <see cref="User"/> to be have account type <see cref="AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE"/>.
        /// </summary>
        private void UserIsPublisher() =>
            Identity.AddClaim(new(TokenClaimConstants.ACCOUNT_TYPE, DatabaseFixture.GetTypeId(AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE).ToString()));

        /// <summary>
        /// Set <see cref="User"/> to be have account type <see cref="AccountTypeConstants.OPERATOR_ACCOUNT_TYPE"/>.
        /// </summary>
        private void UserIsOperator() =>
            Identity.AddClaim(new(TokenClaimConstants.ACCOUNT_TYPE, DatabaseFixture.GetTypeId(AccountTypeConstants.OPERATOR_ACCOUNT_TYPE).ToString()));

        /// <summary>
        /// Set <see cref="User"/> to be have account type <see cref="AccountTypeConstants.USER_ACCOUNT_TYPE"/>.
        /// </summary>
        private void UserIsUser() =>
            Identity.AddClaim(new(TokenClaimConstants.ACCOUNT_TYPE, DatabaseFixture.GetTypeId(AccountTypeConstants.USER_ACCOUNT_TYPE).ToString()));

        #region Without User
        [Fact]
        public void CheckWhitelistControllerAndMethod()
        {
            // Arrange.

            // Act.
            var result = _accessService.AccessFor(_appIdOne)
                .CheckWhitelistEndpoint(PEACHES_CONTROLLER, PEACHES_ENDPOINT_EAT)
                .TryVerify(out IResult? problem);

            // Assert.
            Assert.True(result);
            Assert.Null(problem);
        }

        [Fact]
        public void CheckWhitelistControllerAndMethodFailController()
        {
            // Arrange.

            // Act.
            var result = _accessService.AccessFor(_appIdOne)
                .CheckWhitelistEndpoint(UNRECOGNIZED, PEACHES_ENDPOINT_EAT)
                .TryVerify(out IResult? problem);

            // Assert.
            Assert.False(result);
            Assert.NotNull(problem);
        }

        [Fact]
        public void CheckWhitelistControllerAndMethodFailMethod()
        {
            // Arrange.

            // Act.
            var result = _accessService.AccessFor(_appIdOne)
                .CheckWhitelistEndpoint(PAPERS_CONTROLLER, PAPERS_ENDPOINT_TEAR)
                .TryVerify(out IResult? problem);

            // Assert.
            Assert.False(result);
            Assert.NotNull(problem);
        }

        [Fact]
        public void CheckWhitelistControllerAndMethodFailBoth()
        {
            // Arrange.

            // Act.
            var result = _accessService.AccessFor(_appIdOne)
                .CheckWhitelistEndpoint(UNRECOGNIZED, UNRECOGNIZED)
                .TryVerify(out IResult? problem);

            // Assert.
            Assert.False(result);
            Assert.NotNull(problem);
        }

        [Fact]
        public void CheckWhitelistFailAppId()
        {
            // Arrange.

            // Act.
            var result = _accessService.AccessFor(UNRECOGNIZED)
                .CheckWhitelistEndpoint(PEACHES_CONTROLLER, PEACHES_ENDPOINT_EAT)
                .TryVerify(out IResult? problem);

            // Assert.
            Assert.False(result);
            Assert.NotNull(problem);
        }

        [Fact]
        public void CheckWhitelistFailDefault()
        {
            // Arrange.

            // Act.
            var result = _accessService.AccessFor(_appIdOne)
                .TryVerify(out IResult? problem);

            // Assert.
            Assert.False(result);
            Assert.NotNull(problem);
        }
        #endregion

        #region User and Application
        [Fact]
        public void CheckWhitelistUser()
        {
            // Arrange.
            UserIsOperator();

            // Act.
            var result = _accessService.AccessFor(_appIdOne, User)
                .CheckWhitelistEndpoint(PEACHES_CONTROLLER, PEACHES_ENDPOINT_EAT)
                .AllowOperator()
                .TryVerify(out IResult? problem);

            // Assert.
            Assert.True(result);
            Assert.Null(problem);
        }
        #endregion
    }
}
