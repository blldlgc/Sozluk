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
        private List<int> wordIds;
        private int currentWordIndex = 0;
        private List<Button> answerButtons;

        public QuizTestPage(List<int> wordIds)
        {
            InitializeComponent();
            this.wordIds = wordIds;
            _localDatabaseService = new LocalDatabaseService();

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
                // Navigate back or other actions
            }
        }

        private async void AnswerButtonClicked(object sender, EventArgs e)
        {
            // Seçilen butonu kontrol edebilirsiniz
            Button selectedButton = (Button)sender;

            // Eğer doğru cevap seçildiyse, kullanıcıya bilgi verebilirsiniz
            var currentWord = await _localDatabaseService.GetDictionaryById(wordIds[currentWordIndex]);
            if (selectedButton.Text == currentWord.Meaning)
            {
                await DisplayAlert("Doğru!", "Doğru cevabı seçtiniz.", "Tamam");
                _localDatabaseService.UpdateLevel(currentWord.Id);
            }
            else
            {
                await DisplayAlert("Yanlış!", "Yanlış cevabı seçtiniz.", "Tamam");
            }

            // Bir sonraki soruya geç
            currentWordIndex++;
            LoadQuestion();
        }
    }
}
