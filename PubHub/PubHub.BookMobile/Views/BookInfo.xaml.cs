using PubHub.BookMobile.Auth;
using PubHub.BookMobile.ViewModels;
using PubHub.Common.Services;

namespace PubHub.BookMobile.Views;

public partial class BookInfo : ContentPage
{
    private readonly BookInfoViewModel _viewModel;
    private readonly IUserService _userService;

    public BookInfo(BookInfoViewModel viewModel, IUserService userService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _userService = userService;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        _viewModel.IsAuthenticated = User.IsAuthenticated;
        _viewModel.AlreadyOwnsBook = ((User.Id.HasValue) && (await _userService.GetUserBooksAsync(User.Id!.Value)).Instance!.Any(book => book.Id == _viewModel.CurrentViewedItem.Id));
        base.OnAppearing();
    }
}
