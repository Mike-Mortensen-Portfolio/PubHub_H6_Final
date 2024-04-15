using PubHub.BookMobile.Auth;
using PubHub.Common.ErrorSpecifications;
using PubHub.BookMobile.ViewModels;
using PubHub.Common.Services;

namespace PubHub.BookMobile.Views;

public partial class BookInfo : ContentPage
{
    private readonly BookInfoViewModel _viewModel;
    private readonly IUserService _userService;
    private readonly IAuthenticationService _authService;

    public BookInfo(BookInfoViewModel viewModel, IUserService userService, IAuthenticationService authService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _userService = userService;
        _authService = authService;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        await User.CheckStateAndTryRefreshAsync(_authService);

        _viewModel.IsAuthenticated = User.IsAuthenticated;

        var result = await _userService.GetUserBooksAsync(User.Id!.Value);

        if (!result.IsSuccess)
        {
            await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);


            await Shell.Current.GoToAsync("..");
            return;
        }

        _viewModel.AlreadyOwnsBook = User.Id.HasValue && (result.Instance?.Any(book => book.Id == _viewModel.CurrentViewedItem.Id) ?? false);
        base.OnAppearing();
    }
}
