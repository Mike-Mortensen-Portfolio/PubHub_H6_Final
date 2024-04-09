namespace PubHub.BookMobile
{
    public partial class AuthorizedShell : Shell
    {
        public AuthorizedShell()
        {
            InitializeComponent();

            var colors = Application.Current!.Resources.MergedDictionaries.First();
            FlyoutBackgroundColor = (Color)colors["Primary"];
        }
    }
}
