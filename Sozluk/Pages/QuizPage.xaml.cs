using Sozluk.Database;
using Sozluk.Models;
namespace Sozluk.Pages;

public partial class QuizPage : ContentPage
{

    private readonly LocalDatabaseService _localDatabaseService = new LocalDatabaseService();
    private int quizCount;
    private int wordCount;
    private List<int> wordIds;
    public QuizPage()
	{
        _localDatabaseService = new LocalDatabaseService();
        InitializeComponent();
    }
    protected override async void OnAppearing()
    {
        // Sayfa açıldığında çalışacak kodlar
        base.OnAppearing();
        wordIds = new List<int>();

        wordCount = await _localDatabaseService.GetDailyWordCount(); // Bugün sorulacak kelime sayısını al
        dailyWordCountLabel.Text = $"Bugün ilk defa sorulacak kelime sayısı: {wordCount}";
        await _localDatabaseService.InitializeDailyWordCount(); // Bugün sorulacak kelime sayısını ayarla
        quizCount = await _localDatabaseService.GetDailyQuizCount(); // Bugün sorulacak kelime sayısını al
        dailyQuizCountLabel.Text = $"Kalan kelime sayısı: {quizCount}";

        for (int i = 1; i <= 7; i++)
        {
            LoadWordsByLevel(i);// Her seviyedeki kelimeleri yükleme işlemi

        }
    }

    private async void StartQuizBtnClicked(object sender, EventArgs e)
    {
        // Quiz sayfasına gitme işlemi
        await Navigation.PushAsync(new QuizTestPage(wordIds)) ;
    }

    private async void LoadWordsByLevel(int level)
    {
        // Kelimeleri alır ve sayılarını ekrana yazdırır
        var words = await _localDatabaseService.GetWordsByLevel(level, quizCount);
        int wordCounts = words.Count();
        var wordList = string.Join(", ", words.Select(w => w.WordId)); // Kelimeleri ayır ve virgülle birleştir
        wordIds.AddRange(words.Select(w => w.WordId)); // wordIds listesine ekle

        var label = (Label)FindByName($"wordLabel{level}");
        label.Text = $"Level {level} kelime sayısı: {wordCounts}";
    }


}