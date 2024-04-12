using PubHub.BookMobile.Auth;
using PubHub.BookMobile.ViewModels;

namespace PubHub.BookMobile.Views;

public partial class BookInfo : ContentPage
{
    private readonly BookInfoViewModel _viewModel;
    public BookInfo(BookInfoViewModel viewModel)
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
