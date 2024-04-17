using PubHub.BookMobile.ViewModels;

namespace PubHub.BookMobile.Views;

public partial class Login : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public Login(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        BindingContext = _viewModel;
    }
}
