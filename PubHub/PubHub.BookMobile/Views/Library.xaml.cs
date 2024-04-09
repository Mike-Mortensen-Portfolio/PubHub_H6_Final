using PubHub.BookMobile.Auth;
using PubHub.BookMobile.Models;
using PubHub.BookMobile.ViewModels;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Genres;
using static Android.App.DownloadManager;

namespace PubHub.BookMobile.Views;

public partial class Library : ContentPage
{
    private readonly LibraryViewModel _viewModel;

    public Library(LibraryViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    private async void FilterByGenre(object sender, SelectionChangedEventArgs e)
    {
        var selectedGenres = e.CurrentSelection.Select(obj => ((GenreInfoModel)obj).Id);
        _viewModel.Query.Genres = ((selectedGenres.Any()) ? (selectedGenres.ToArray()) : (null));
        await _viewModel.FetchBooks();
    }

    protected override async void OnAppearing()
    {
        _viewModel.IsAuthenticated = User.IsAuthenticated;

        if (_viewModel.IsGenreVisible)
            _viewModel.ToggleGenreView();
        _viewModel.Query = new BookQuery();
        _viewModel.SearchTerm = string.Empty;
        await _viewModel.FetchGenres();
        await _viewModel.FetchBooks();
        base.OnAppearing();
    }
}
