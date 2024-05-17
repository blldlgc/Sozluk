using Sozluk.Database;
using Sozluk.Models;

namespace Sozluk.Pages;

public partial class QuizTestPage : ContentPage
{

    private readonly LocalDatabaseService _localDatabaseService = new LocalDatabaseService();
    //private readonly QuizHelper _quizHelper;
    private List<int> wordIds;
    private int currentWordIndex = 0;

    public QuizTestPage(List<int> wordIds)
    {
        InitializeComponent();
        this.wordIds = wordIds;
        _localDatabaseService = new LocalDatabaseService();

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
            List<string> answerOptions = new List<string>();
            answerOptions.Add(currentWord.Meaning); // Doğru cevap

            /*while (answerOptions.Count < 4)
            {
                // Rastgele bir kelime seç
                Dictionary randomWord = await _quizHelper.GetRandomWord();

                // Bu kelime daha önce eklenmediyse listeye ekle
                if (!answerOptions.Contains(randomWord.Meaning))
                {
                    answerOptions.Add(randomWord.Meaning);
                }
            }*/
            answerOptions = answerOptions.OrderBy(x => Guid.NewGuid()).ToList();
        }
        else
        {
            // Tüm sorular bittiyse
            // ...
        }
    }

    private async void AnswerButtonClicked(object sender, EventArgs e)
    {
        
    }
}