using PubHub.BookMobile.Auth;
using PubHub.BookMobile.ViewModels;

namespace PubHub.BookMobile.Views;

public partial class Profile : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    public Profile(ProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        _viewModel.NotInEditMode = true;
        _viewModel.IsAuthenticated = User.IsAuthenticated;
        await _viewModel.FecthUserCommand.ExecuteAsync(null);
        base.OnAppearing();
    }
}
