using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using PubHub.BookMobile.Auth;
using PubHub.BookMobile.ErrorSpecifications;
using PubHub.Common;
using PubHub.Common.Models.Users;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class ProfileViewModel : NavigationObject
    {
        public const int MAX_NAME_LENGTH = 128;
        public const int MAX_EMAIL_LENGTH = 256;

        private readonly IUserService _userService;
        private readonly Regex _emailRegex;
        private UserInfoModel? _userInfo;

        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private string? _fullName;
        [ObservableProperty]
        private bool _NotInEditMode = true;
        [ObservableProperty]
        private string? _name;
        [ObservableProperty]
        private string? _surname;
        [ObservableProperty]
        private string? _email;
        [ObservableProperty]
        private bool _isAuthenticated;
        private bool FormIsFilled => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Surname) && !string.IsNullOrWhiteSpace(Email) && Name.Length <= MAX_NAME_LENGTH && Surname.Length <= MAX_NAME_LENGTH;
        private bool IsValidEmail => !string.IsNullOrWhiteSpace(Email) && _emailRegex.Match(Email!).Success && Email.Length <= MAX_EMAIL_LENGTH;
        private bool CanUpdate => FormIsFilled && IsValidEmail;

        public ProfileViewModel(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            var stringRegex = configuration.GetSection("RegexValues").GetValue<string>("EmailRegex") ?? throw new Exception("Email regex was null. That's not supposed to happen!");
            _emailRegex = new Regex(stringRegex);
        }

        [RelayCommand]
        public async Task FecthUser()
        {
            IsBusy = true;
            if (!User.IsAuthenticated || User.Id is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert(UnauthorizedError.TITLE, UnauthorizedError.ERROR_MESSAGE, UnauthorizedError.BUTTON_TEXT);
                IsBusy = false;

                await NavigateToPage("Home");
                return;
            }

            var result = await _userService.GetUserAsync(User.Id.Value);

            if (!result.IsSuccess || result.Instance == null)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await Shell.Current.CurrentPage.DisplayAlert(UnauthorizedError.TITLE, UnauthorizedError.ERROR_MESSAGE, UnauthorizedError.BUTTON_TEXT);

                    await NavigateToPage("Home");
                }
                else
                    await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);
                IsBusy = false;
                return;
            }

            _userInfo = result.Instance;
            FullName = $"{_userInfo!.Name} {_userInfo.Surname}";
            Name = _userInfo.Name;
            Surname = _userInfo.Surname;
            Email = _userInfo.Email;
            IsBusy = false;
        }

        [RelayCommand(CanExecute = nameof(NotInEditMode))]
        public void GoIntoEditMode()
        {
            NotInEditMode = false;
        }

        [RelayCommand(CanExecute = nameof(CanUpdate))]
        public async Task Update()
        {
            IsBusy = true;
            var updateModel = new UserUpdateModel(_userInfo!)
            {
                Name = Name!,
                Surname = Surname!,
            };
            updateModel.Account.Email = Email!;
            var result = await _userService.UpdateUserAsync(User.Id!.Value, updateModel);

            if (!result.IsSuccess || result.Instance is null)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Shell.Current.CurrentPage.DisplayAlert(UnauthorizedError.TITLE, UnauthorizedError.ERROR_MESSAGE, UnauthorizedError.BUTTON_TEXT);
                else
                    await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);

                IsBusy = false;
                return;
            }

            _userInfo = result.Instance;

            Name = result.Instance.Name;
            Surname = result.Instance.Surname;
            Email = result.Instance.Email;

            IsBusy = false;
            NotInEditMode = true;
        }

        [RelayCommand]
        public void Cancel()
        {
            Name = _userInfo!.Name;
            Surname = _userInfo.Surname;
            Email = _userInfo.Email;

            NotInEditMode = true;
        }

        partial void OnNotInEditModeChanged(bool value)
        {
            GoIntoEditModeCommand.NotifyCanExecuteChanged();
        }

        partial void OnNameChanged(string? value)
        {
            UpdateCommand.NotifyCanExecuteChanged();
        }

        partial void OnSurnameChanged(string? value)
        {
            UpdateCommand.NotifyCanExecuteChanged();
        }

        partial void OnEmailChanged(string? value)
        {
            UpdateCommand.NotifyCanExecuteChanged();
        }
    }
}
