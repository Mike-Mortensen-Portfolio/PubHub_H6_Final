using PubHub.BookMobile.ViewModels;

namespace PubHub.BookMobile.Views;

public partial class EBookView : ContentPage
{
    private readonly EBookViewModel _viewModel;

    public EBookView(EBookViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        _viewModel.ScrollView = () => scrollView;

        base.OnAppearing();
    }
}
