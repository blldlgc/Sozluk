namespace Sozluk.Pages;

public partial class WordAddingPage : ContentPage
{
	public WordAddingPage()
	{
		InitializeComponent();
	}

	private async void BackBtnClicked(object sender, EventArgs e)
	{
        await App.Current.MainPage.Navigation.PopModalAsync();
    }
}