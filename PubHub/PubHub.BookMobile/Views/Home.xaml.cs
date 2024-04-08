using Java.Lang;
using PubHub.BookMobile.Auth;
using PubHub.BookMobile.ViewModels;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Services;

namespace PubHub.BookMobile.Pages;

public partial class Home : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public Home(HomeViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        _viewModel.IsAuthenticated = User.IsAuthenticated;

        base.OnAppearing();
    }
}
