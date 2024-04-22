using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using PubHub.BookMobile.Auth;
using PubHub.Common.ErrorSpecifications;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class LoginViewModel : NavigationObject
    {
        public const int MAX_EMAIL_LENGTH = 256;
        public const int MAX_PASSWORD_LENGTH = 64;

        private readonly IAuthenticationService _authService;
        private readonly Regex _emailRegex;

        [ObservableProperty]
        private bool _isBusy = false;
        [ObservableProperty]
        private string? _email;
        [ObservableProperty]
        private string? _password;

        public LoginViewModel(IAuthenticationService authService, IConfiguration configuration)
        {
            var stringRegex = configuration.GetSection("RegexValues").GetValue<string>("EmailRegex") ?? throw new Exception("Email regex was null. That's not supposed to happen!");
            _emailRegex = new Regex(stringRegex);
            _authService = authService;
        }

        private bool IsValidEmail => !string.IsNullOrWhiteSpace(Email) && _emailRegex.Match(Email).Success && Email.Length <= MAX_EMAIL_LENGTH;
        private bool IsValidPassword => !string.IsNullOrWhiteSpace(Password) && Password.Length <= MAX_PASSWORD_LENGTH;
        private bool CanSignIn => IsValidEmail && IsValidPassword && !IsBusy;

        [RelayCommand(CanExecute = nameof(CanSignIn))]
        public async Task SignIn()
        {
            IsBusy = true;
            var result = await _authService.LoginAsync(new LoginInfo { Email = Email!, Password = Password! });

            TokenResponseModel tokens;
            if (!result.IsSuccess || result.Instance is null)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Shell.Current.CurrentPage.DisplayAlert("Validation Error", $"Email or password wasn't correct. Try again.", "OK");
                else if (result.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    await Shell.Current.CurrentPage.DisplayAlert(TooManyRequestError.TITLE, TooManyRequestError.ERROR_MESSAGE, TooManyRequestError.BUTTON_TEXT);
                else
                    await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);

                IsBusy = false;
                return;
            }

            tokens = result.Instance;

            try
            {
                await User.SetAsync(tokens);
            }
            catch (Exception)
            {

                await Shell.Current.CurrentPage.DisplayAlert(SetSecureStorageError.TITLE, SetSecureStorageError.ERROR_MESSAGE, SetSecureStorageError.BUTTON_TEXT);
            }


            Application.Current!.MainPage = new AuthorizedShell();

            IsBusy = false;
        }

        partial void OnEmailChanged(string? value)
        {
            SignInCommand.NotifyCanExecuteChanged();
        }

        partial void OnPasswordChanged(string? value)
        {
            SignInCommand.NotifyCanExecuteChanged();
        }
    }
}
