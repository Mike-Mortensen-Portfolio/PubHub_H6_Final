using PubHub.BookMobile.Auth;
using PubHub.Common;
using PubHub.Common.Services;

namespace PubHub.BookMobile.Views;

public partial class Logout : ContentPage
{
    private readonly IAuthenticationService _authService;

    public Logout(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override async void OnAppearing()
    {
        var result = await _authService.RevokeTokenAsync();
        if (!result.IsSuccess)
        {
            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Something went wrong... We'll sign you out anyways but contact PubHub support if the problem persists{Environment.NewLine}Error: {ErrorsCodeConstants.UNAUTHORIZED}", "OK");
            else
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Something went wrong... We'll sign you out anyways but contact PubHub support if the problem persists{Environment.NewLine}Error: {ErrorsCodeConstants.NO_CONNECTION}", "OK");
        }

        User.Unset();
        Application.Current!.MainPage = new AppShell();

        base.OnAppearing();
    }
}
