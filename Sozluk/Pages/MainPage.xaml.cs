using HtmlAgilityPack;
using Sozluk.Database;
using Sozluk.Helpers;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sozluk.Pages
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        QuizHelper _quizHelper;

        public MainPage()
        {
            InitializeComponent();
            WelcomeText.Text = "Sözlük-Quiz Uygulamasına Hoşgeldiniz " + FirebaseAuthHelper.CurrentUsername + "! 👋";
            var databaseService = new LocalDatabaseService();
            _quizHelper = new QuizHelper(databaseService.GetConnection());
            _quizHelper.ResetOldDays(); // Önceki çözülmeyen günlerdeki soruların sıfırlanması
     
        }
      
    }

}
