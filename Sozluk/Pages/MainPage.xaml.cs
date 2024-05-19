using Sozluk.Helpers;

namespace Sozluk.Pages
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            WelcomeText.Text = "Sözlük-Quiz Uygulamasına Hoşgeldiniz " + FirebaseAuthHelper.CurrentUsername + "!";
        }

        private async void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
            await Shell.Current.GoToAsync($"//{nameof(DictionaryPage)}");
        }
    }

}
