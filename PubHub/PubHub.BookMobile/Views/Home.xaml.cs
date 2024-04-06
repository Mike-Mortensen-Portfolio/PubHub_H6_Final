using PubHub.BookMobile.ViewModels;

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
}
