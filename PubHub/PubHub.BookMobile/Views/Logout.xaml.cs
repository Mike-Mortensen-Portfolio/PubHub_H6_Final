using PubHub.BookMobile.Auth;

namespace PubHub.BookMobile.Views;

public partial class Logout : ContentPage
{
    public Logout()
    {
        InitializeComponent();

        User.Unset();

        Application.Current!.MainPage = new AppShell();
    }
}
