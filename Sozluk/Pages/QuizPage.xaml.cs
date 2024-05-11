namespace Sozluk.Pages;

public partial class QuizPage : ContentPage
{
	public QuizPage()
	{
		InitializeComponent();
	}

    private async void StartQuizBtnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new QuizTestPage());
    }
}