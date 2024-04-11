using PubHub.BookMobile.Auth;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Services;

namespace PubHub.BookMobile
{
    public partial class App : Application
    {
        private readonly IAuthenticationService _authService;

        public App(IAuthenticationService authService)
        {
            InitializeComponent();
            _authService = authService;

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            if (User.GetCachedToken(out TokenInfo? result) && result != null)
            {
                var response = await _authService.RefreshTokenAsync();

                if (response.IsSuccess && response.Instance != null)
                {
                    User.Set(response.Instance);
                    MainPage = new AuthorizedShell();
                }
            }

            base.OnStart();
        }

        protected override async void OnResume()
        {
            if (User.GetCachedToken(out TokenInfo? result) && result != null)
            {
                var response = await _authService.RefreshTokenAsync();

                if (response.IsSuccess && response.Instance != null)
                {
                    User.Set(response.Instance);
                    MainPage = new AuthorizedShell();
                }
            }

            base.OnResume();
        }
    }
}
