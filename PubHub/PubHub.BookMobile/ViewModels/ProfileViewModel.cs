using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.Auth;
using PubHub.Common;
using PubHub.Common.Models.Users;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class ProfileViewModel : NavigationObject
    {
        private readonly IUserService _userService;

        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private UserInfoModel? _userInfo;
        [ObservableProperty]
        private string? _fullName;
        [ObservableProperty]
        private bool _NotInEditMode = true;
        [ObservableProperty]
        private UserUpdateModel? updateModel;

        public ProfileViewModel(IUserService userService)
        {
            _userService = userService;
        }

        [RelayCommand]
        public async Task FecthUser()
        {
            IsBusy = true;
            if (!User.IsAuthenticated || User.Id is null)
            {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"You do not have permission to view this content.{Environment.NewLine}Please try again or contact PubHub support if the problem persists.{Environment.NewLine}Error: {ErrorsCodeConstants.UNAUTHORIZED}", "OK");
                IsBusy = false;
                return;
            }

            var result = await _userService.GetUserAsync(User.Id.Value);

            if (!result.IsSuccess || result.Instance == null)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"You do not have permission to view this content.{Environment.NewLine}Please try again or contact PubHub support if the problem persists.{Environment.NewLine}Error: {ErrorsCodeConstants.UNAUTHORIZED}", "OK");
                else
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Something went wrong...{Environment.NewLine}Please try again or contact PubHub support if the problem persists.{Environment.NewLine}Error: {ErrorsCodeConstants.NO_CONNECTION}", "OK");
                IsBusy = false;
                return;
            }

            UserInfo = result.Instance;
            FullName = $"{UserInfo.Name} {UserInfo.Surname}";
            UpdateModel = new UserUpdateModel(UserInfo);
            IsBusy = false;
        }

        [RelayCommand(CanExecute = nameof(NotInEditMode))]
        public void ToggleEdit()
        {
            NotInEditMode = false;
        }

        partial void OnNotInEditModeChanged(bool value)
        {
            ToggleEditCommand.NotifyCanExecuteChanged();
        }
    }
}
