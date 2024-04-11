namespace PubHub.BookMobile
{
    public partial class LoadingShell : Shell
    {
        public LoadingShell()
        {
            InitializeComponent();

            var colors = Application.Current!.Resources.MergedDictionaries.First();
            FlyoutBackgroundColor = (Color)colors["Primary"];
        }
    }
}
