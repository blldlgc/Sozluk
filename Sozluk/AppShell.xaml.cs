using Sozluk.Pages;

namespace Sozluk
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
            Routing.RegisterRoute(nameof(WordAddingPage), typeof(WordAddingPage));
            Routing.RegisterRoute(nameof(DictionaryPage), typeof(DictionaryPage));

        }
    }
}
