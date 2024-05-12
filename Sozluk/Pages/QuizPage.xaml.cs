using Sozluk.Database;
namespace Sozluk.Pages;

public partial class QuizPage : ContentPage
{
    private int quizCount = 10;
	public QuizPage()
	{
		InitializeComponent();
	}

    

    private async void StartQuizBtnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new QuizTestPage(quizCount));
    }
}