namespace Sozluk
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        public void RestartApp()
        {
            // Yeni bir AppShell örneği oluştur ve MainPage olarak ayarla
            MainPage = new AppShell();
        }
    }
}
