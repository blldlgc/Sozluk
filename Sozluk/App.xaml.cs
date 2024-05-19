namespace Sozluk
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
            this.RequestedThemeChanged += (s, e) => { Application.Current.UserAppTheme = AppTheme.Light; };

            MainPage = new AppShell();
        }

        public void RestartApp()
        {
            // Yeni bir AppShell örneği oluştur ve MainPage olarak ayarla
            MainPage = new AppShell();
        }
    }
}
