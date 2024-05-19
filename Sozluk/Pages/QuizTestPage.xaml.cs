using Sozluk.Database;
using Sozluk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Sozluk.Pages
{
    public partial class QuizTestPage : ContentPage
    {
        private readonly LocalDatabaseService _localDatabaseService;
        private readonly QuizHelper _quizHelper;
        private List<int> wordIds;
        private int currentWordIndex = 0;
        private List<Button> answerButtons;

        public QuizTestPage(List<int> wordIds)
        {
            InitializeComponent();
            this.wordIds = wordIds;
            _localDatabaseService = new LocalDatabaseService();
            _quizHelper = new QuizHelper(_localDatabaseService.GetConnection());

            // Answer buttons listesi oluştur
            answerButtons = new List<Button> { AnswerButton1, AnswerButton2, AnswerButton3, AnswerButton4 };

            // Soruları oluştur
            LoadQuestion();
        }

        private async void LoadQuestion()
        {
            if (currentWordIndex < wordIds.Count)
            {
                // WordId'ye göre kelimeyi al
                var currentWord = await _localDatabaseService.GetDictionaryById(wordIds[currentWordIndex]);

                // Soruyu ekrana yaz
                QuestionLabel.Text = currentWord.Word;

                // Cevap seçeneklerini oluştur
                List<string> answerOptions = new List<string>
                {
                    currentWord.Meaning // Doğru cevap
                };

                // Veritabanından rastgele 3 anlam daha çek
                var randomMeanings = await _localDatabaseService.GetRandomMeanings(3, currentWord.Meaning);
                answerOptions.AddRange(randomMeanings);

                // Cevap seçeneklerini karıştır
                var shuffledOptions = answerOptions.OrderBy(x => Guid.NewGuid()).ToList();

                // Cevap seçeneklerini butonlara ata
                for (int i = 0; i < answerButtons.Count; i++)
                {
                    answerButtons[i].Text = shuffledOptions[i];
                    answerButtons[i].Clicked -= AnswerButtonClicked; // Olay işleyicisini kaldır
                    answerButtons[i].Clicked += AnswerButtonClicked; // Olay işleyicisini ekle
                }
            }
            else
            {
                // Tüm sorular bittiyse
                await DisplayAlert("Quiz Bitti", "Tüm soruları cevapladınız!", "Tamam");
                await App.Current.MainPage.Navigation.PopModalAsync();
                
            }
        }

        private async void AnswerButtonClicked(object sender, EventArgs e)
        {
            Button selectedButton = (Button)sender;
            var currentWord = await _localDatabaseService.GetDictionaryById(wordIds[currentWordIndex]);

            
            var quizdates = await _localDatabaseService.GetQuizDatesForWord(currentWord);
            
            if (quizdates.Level == 1)
            {
                // GÜnlük kalan kelime sayısını azaltır
                _localDatabaseService.DecreaseDailyWordCount();
                _localDatabaseService.UpdateQuizDates(currentWord.Id);
            }

            if (selectedButton.Text == currentWord.Meaning)
            {
                await DisplayAlert("Doğru!", "Doğru cevabı seçtiniz.", "Tamam");

                // Kelime seviyesini yükselt
                _localDatabaseService.UpdateLevel(currentWord.Id);
            }
            else
            {
                await DisplayAlert("Yanlış!", "Yanlış cevabı seçtiniz.", "Tamam");
                await _quizHelper.ResetLevelAndDates(currentWord.Id);
            }

            currentWordIndex++;
            LoadQuestion();
        }
    }
}
