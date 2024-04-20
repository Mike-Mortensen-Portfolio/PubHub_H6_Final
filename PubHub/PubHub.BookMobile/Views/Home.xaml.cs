using PubHub.BookMobile.Auth;
using PubHub.BookMobile.ViewModels;
using PubHub.Common.Services;

namespace PubHub.BookMobile.Views;

public partial class Home : ContentPage
{
    private readonly HomeViewModel _viewModel;
    private readonly IAuthenticationService _authService;

    public Home(HomeViewModel viewModel, IAuthenticationService authService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _authService = authService;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        _viewModel.IsAuthenticated = User.IsAuthenticated;

        base.OnAppearing();
    }
}
