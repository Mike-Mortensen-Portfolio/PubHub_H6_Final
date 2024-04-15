using PubHub.BookMobile.Auth;
using PubHub.BookMobile.ViewModels;
using PubHub.Common.Services;

namespace PubHub.BookMobile.Views;

public partial class BookContentInfo : ContentPage
{
    private readonly BookContentInfoViewModel _viewModel;
    private readonly IAuthenticationService _authService;

    public BookContentInfo(BookContentInfoViewModel viewModel, IAuthenticationService authService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _authService = authService;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        await User.CheckStateAndTryRefreshAsync(_authService);

        _viewModel.IsAuthenticated = User.IsAuthenticated;

        base.OnAppearing();
    }
}
