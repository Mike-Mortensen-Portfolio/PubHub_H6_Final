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

            MainPage = new LoadingShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            (bool IsSuccess, _) = await User.TryGetCachedTokenAsync();
            if (!IsSuccess)
            {
                User.Unset();
                MainPage = new AppShell();
                return;
            }

            var response = await _authService.RefreshTokenAsync();

            if (!response.IsSuccess || response.Instance is null)
            {
                User.Unset();
                MainPage = new AppShell();
                return;
            }

            await User.SetAsync(response.Instance);
            MainPage = new AuthorizedShell();
        }

        protected override async void OnResume()
        {
            base.OnResume();

            (bool IsSuccess, _) = await User.TryGetCachedTokenAsync();
            if (!IsSuccess)
            {
                User.Unset();
                MainPage = new AppShell();
                return;
            }

            var response = await _authService.RefreshTokenAsync();

            if (!response.IsSuccess || response.Instance is null)
            {
                User.Unset();
                MainPage = new AppShell();
                return;
            }

            await User.SetAsync(response.Instance);
            MainPage = new AuthorizedShell();
        }
    }
}
