using System.Collections.ObjectModel;

namespace PubHub.BookMobile.Pages;

public partial class Home : ContentPage
{
    public Home()
    {
        InitializeComponent();

        BindingContext = this;
    }

    public static ObservableCollection<Item> Items { get; } =
    [
        new Item
        {
            Description = "This is sample text and it will descripe the image above",
            Header = "This is a header",
            ImageUrl = "https://st.depositphotos.com/2274151/4841/i/450/depositphotos_48410095-stock-photo-sample-blue-square-grungy-stamp.jpg",
        },
        new Item
        {
            Description = "This is sample text and it will descripe the image above",
            Header = "This is a header",
            ImageUrl = "https://st.depositphotos.com/2274151/4841/i/450/depositphotos_48410095-stock-photo-sample-blue-square-grungy-stamp.jpg"
        },
        new Item
        {
            Description = "This is sample text and it will descripe the image above",
            Header = "This is a header",
            ImageUrl = "https://st.depositphotos.com/2274151/4841/i/450/depositphotos_48410095-stock-photo-sample-blue-square-grungy-stamp.jpg"
        },
    ];
}

public class Item
{
    public string? ImageUrl { get; set; }
    public string? Header { get; set; }
    public string? Description { get; set; }
}
