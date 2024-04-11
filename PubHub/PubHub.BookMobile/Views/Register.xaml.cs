using PubHub.BookMobile.ViewModels;

namespace PubHub.BookMobile.Views;

public partial class Register : ContentPage
{
    private readonly RegisterViewModel _viewModel;

    public Register(RegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        BindingContext = _viewModel;
    }
}
