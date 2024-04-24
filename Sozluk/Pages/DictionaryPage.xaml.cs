namespace Sozluk.Pages;

public partial class DictionaryPage : ContentPage
{
	private readonly Database.LocalDatabaseService _localDatabaseService;
	public DictionaryPage()
	{
		InitializeComponent();
		//_localDatabaseService = localDatabaseService;
		//Task.Run(async () =>  ListView.ItemsSource = await _localDatabaseService.GetDictionary());
	}

	private async void AddWordBtnClicked(object sender, EventArgs e)
	{
        await App.Current.MainPage.Navigation.PushModalAsync(new WordAddingPage());
    }

	private void WordsListView_ItemTapped(object sender, ItemTappedEventArgs e)
	{
        WordsListView.SelectedItem = null;
    }
}