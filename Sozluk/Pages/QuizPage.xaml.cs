using Sozluk.Database;
using Sozluk.Models;
namespace Sozluk.Pages;

public partial class QuizPage : ContentPage
{

    private readonly LocalDatabaseService _localDatabaseService = new LocalDatabaseService();
    private int quizCount = 4;
    private List<int> wordIds = new List<int>();



    public QuizPage()
	{
        _localDatabaseService = new LocalDatabaseService();
        InitializeComponent();
        for (int i = 1; i <= 7; i++)
        {
            LoadWordsByLevel(i);

        }

    }

    protected override async void OnAppearing()
    {
        
        base.OnAppearing();
        
        /*for (int i = 1; i <= 7; i++)
        {
            LoadWordsByLevel(i);

        }*/


    }



    private async void StartQuizBtnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new QuizTestPage(wordIds)) ;
        
    }

    private async void LoadWordsByLevel(int level)
    {
        // Kelimeleri al
        var words = await _localDatabaseService.GetWordsByLevel(level, quizCount);

        var wordList = string.Join(", ", words.Select(w => w.WordId));

        wordIds.AddRange(words.Select(w => w.WordId)); // wordIds listesine ekle


        var label = (Label)FindByName($"wordLabel{level}");
        label.Text = $"Level {level} kelime id'leri: {wordList}";

    }


}