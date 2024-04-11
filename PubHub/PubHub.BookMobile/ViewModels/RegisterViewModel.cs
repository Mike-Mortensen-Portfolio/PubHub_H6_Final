using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using PubHub.BookMobile.Auth;
using PubHub.Common;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Users;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthenticationService _authService;
        private readonly Regex _emailRegex;

        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private string? _name;
        [ObservableProperty]
        private string? _surname;
        [ObservableProperty]
        private DateTime _birthday;
        [ObservableProperty]
        private string? _email;
        [ObservableProperty]
        private string? _password;
        [ObservableProperty]
        private string? _passwordConfirm;


        public RegisterViewModel(IAuthenticationService authService, IConfiguration configuration)
        {
            var stringRegex = configuration.GetSection("RegexValues").GetValue<string>("EmailRegex") ?? throw new Exception("Email regex was null. That's not supposed to happen!");
            _emailRegex = new Regex(stringRegex);
            Birthday = DateTime.Today.AddYears(-12);

            _authService = authService;
        }

#pragma warning disable CA1822 // Mark members as static
        public string AtLeastTwelve => DateOnly.FromDateTime(DateTime.Today.AddYears(-12)).ToString("dd/MM/yyyy");
        public string AtMostHundredAndTwenty => DateOnly.FromDateTime(DateTime.Today.AddYears(-120)).ToString("dd/MM/yyyy");
#pragma warning restore CA1822 // Mark members as static
        public bool IsValidName => !string.IsNullOrWhiteSpace(Name);
        public bool IsValidSurname => !string.IsNullOrWhiteSpace(Surname);
        public bool IsValidBirthday => Birthday.Date >= DateTime.Parse(AtMostHundredAndTwenty) && Birthday.Date <= DateTime.Parse(AtLeastTwelve);
        public bool IsValidEmail => !string.IsNullOrWhiteSpace(Email) && _emailRegex.Match(Email).Success;
        public bool IsValidPassword => !string.IsNullOrWhiteSpace(Password) && !string.IsNullOrWhiteSpace(PasswordConfirm) && Password.Equals(PasswordConfirm);
        public bool CanRequestRegistration => IsValidName && IsValidSurname && IsValidBirthday && IsValidEmail && IsValidPassword;

        [RelayCommand(CanExecute = nameof(CanRequestRegistration))]
        public async Task Register()
        {
            IsBusy = true;
            var result = await _authService.RegisterUserAsync(new UserCreateModel
            {
                Account = new AccountCreateModel
                {
                    Email = Email!,
                    Password = Password!
                },
                Birthday = DateOnly.FromDateTime(Birthday),
                Name = Name!,
                Surname = Surname!
            });

            if (!result.IsSuccess || result.Instance is null)
            {
                IsBusy = false;
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Couldn't register user.{Environment.NewLine}Please try again or contact PubHub support if the problem persists.{Environment.NewLine}Error: {ErrorsCodeConstants.NO_CONNECTION}", "OK");
                return;
            }

            User.Set(result.Instance.TokenResponseModel);
            Application.Current!.MainPage = new AuthorizedShell();
        }

        partial void OnSurnameChanged(string? value) => RegisterCommand.NotifyCanExecuteChanged();
        partial void OnBirthdayChanged(DateTime value) => RegisterCommand.NotifyCanExecuteChanged();
        partial void OnNameChanged(string? value) => RegisterCommand.NotifyCanExecuteChanged();
        partial void OnEmailChanged(string? value) => RegisterCommand.NotifyCanExecuteChanged();
        partial void OnPasswordChanged(string? value) => RegisterCommand.NotifyCanExecuteChanged();
        partial void OnPasswordConfirmChanged(string? value) => RegisterCommand.NotifyCanExecuteChanged();
    }
}
