using Sozluk.Database;
using Sozluk.Models;
namespace Sozluk.Pages;

public partial class QuizPage : ContentPage
{

    private readonly LocalDatabaseService _localDatabaseService = new LocalDatabaseService();
    private int quizCount = 10;
    private int level = 1;
	public QuizPage()
	{
        _localDatabaseService = new LocalDatabaseService();
        InitializeComponent();
        
	}

    

    private async void StartQuizBtnClicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new QuizTestPage(quizCount));
        LoadWordsByLevel(1);
    }

    private async void LoadWordsByLevel(int level)
    {
        // Kelimeleri al
        var words = await _localDatabaseService.GetWordsByLevel(level);

        var wordList = string.Join(", ", words.Select(w => w.WordId));


        var label = (Label)FindByName($"wordLabel{level}");
        label.Text = $"Level {level} kelime id'leri: {wordList}";

    }


}