using PubHub.BookMobile.Auth;
using PubHub.BookMobile.ViewModels;

namespace PubHub.BookMobile.Views;

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
