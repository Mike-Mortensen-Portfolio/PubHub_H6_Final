using PubHub.BookMobile.ViewModels;

namespace PubHub.BookMobile.Views;

public partial class AudiobookView : ContentPage
{
    private readonly AudiobookViewModel _viewModel;

    public AudiobookView(AudiobookViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        BindingContext = _viewModel;
    }
}
