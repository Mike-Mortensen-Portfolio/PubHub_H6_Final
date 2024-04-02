namespace PubHub.BookMobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            var colors = App.Current!.Resources.MergedDictionaries.First();
            FlyoutBackgroundColor = (Color)colors["Primary"];
        }
    }
}
