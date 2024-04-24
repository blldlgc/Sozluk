namespace Sozluk.Pages;

public partial class DictionaryPage : ContentPage
{
	public DictionaryPage()
	{
		InitializeComponent();
	}

	private async void AddWordBtnClicked(object sender, EventArgs e)
	{
        await App.Current.MainPage.Navigation.PushModalAsync(new WordAddingPage());
    }
}