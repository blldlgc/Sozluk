using Sozluk.Database;
using Sozluk.Helpers;

namespace Sozluk.Pages
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        QuizHelper _quizHelper;

        public MainPage()
        {
            InitializeComponent();
            WelcomeText.Text = "Sözlük-Quiz Uygulamasına Hoşgeldiniz " + FirebaseAuthHelper.CurrentUsername + "!";
            var databaseService = new LocalDatabaseService();
            _quizHelper = new QuizHelper(databaseService.GetConnection());
            _quizHelper.ResetOldDays();
            
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
