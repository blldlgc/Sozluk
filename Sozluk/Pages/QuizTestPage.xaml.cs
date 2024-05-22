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
            try
            {
                if (currentWordIndex < wordIds.Count)
                {
                    // WordId'ye göre kelimeyi alır
                    var currentWord = await _localDatabaseService.GetDictionaryById(wordIds[currentWordIndex]);

                    // Soruyu ekrana yazar
                    QuestionLabel.Text = currentWord.Word;
                    PictureImage.Source = ImageSource.FromFile(currentWord.Image);

                    // Cevap seçeneklerini oluşturur
                    List<string> answerOptions = new List<string> { currentWord.Meaning };

                    // Veritabanından rastgele 3 anlam daha çeker
                    var randomMeanings = await _quizHelper.GetRandomMeanings(3, currentWord.Meaning);
                    answerOptions.AddRange(randomMeanings);

                    // Cevap seçeneklerini karıştırır
                    var shuffledOptions = answerOptions.OrderBy(x => Guid.NewGuid()).ToList();

                    // Cevap seçeneklerini butonlara ataar
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
            catch (ArgumentOutOfRangeException ex)
            {
                await DisplayAlert("Hata", $"Bir hata oluştu: {ex.Message}", "Tamam");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Beklenmeyen bir hata oluştu: {ex.Message}", "Tamam");
            }
        }

        private async void AnswerButtonClicked(object sender, EventArgs e)
        {
            // Butona tıklanınca çalışacak kodlar
            Button selectedButton = (Button)sender;
            var currentWord = await _localDatabaseService.GetDictionaryById(wordIds[currentWordIndex]);
            var quizdates = await _localDatabaseService.GetQuizDatesForWord(currentWord);
            
            if (quizdates.Level == 1)
            {
                // GÜnlük kalan level1 kelime sayısını azaltır
                await _localDatabaseService.DecreaseDailyWordCount();
                await _quizHelper.UpdateQuizDates(currentWord.Id);
            }

            if (selectedButton.Text == currentWord.Meaning)
            {
                await DisplayAlert("Doğru!", "Doğru cevabı seçtiniz.", "Tamam");
                // Kelime seviyesini yükselt
                await _quizHelper.UpdateLevel(currentWord.Id);
                await _quizHelper.IncrementStats(quizdates.Level);
            }
            else
            {
                // Yanlış cevap seçildiyse leveli sıfırlar
                await DisplayAlert("Yanlış!", "Yanlış cevabı seçtiniz.", "Tamam");
                await _quizHelper.ResetLevelAndDates(currentWord.Id);
            }

            currentWordIndex++;
            LoadQuestion();
        }
    }
}
