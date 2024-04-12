using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using PubHub.BookMobile.Auth;
using PubHub.Common;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class LoginViewModel : NavigationObject
    {
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

        private bool IsValidEmail => !string.IsNullOrWhiteSpace(Email) && _emailRegex.Match(Email).Success;
        private bool IsValidPassword => !string.IsNullOrWhiteSpace(Password);
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
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Email or password wasn't correct. Try again.", "OK");
                else
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Couldn't sign in.{Environment.NewLine}Please try again or contact PubHub support if the problem persists.{Environment.NewLine}Error: {ErrorsCodeConstants.NO_CONNECTION}", "OK");

                IsBusy = false;
                return;
            }

            tokens = result.Instance;

            try
            {
                await User.Set(tokens);
            }
            catch (Exception)
            {

                await Application.Current!.MainPage!.DisplayAlert("Error", $"Couldn't sign in.{Environment.NewLine}Please try again or contact PubHub support if the problem persists.{Environment.NewLine}Error: {ErrorsCodeConstants.INVALID_TOKEN}", "OK");
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
