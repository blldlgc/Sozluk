using Sozluk.Database;
using Sozluk.Models;
using Sozluk.Helpers;

namespace Sozluk.Pages;

public partial class DictionaryPage : ContentPage
{
	private readonly LocalDatabaseService _localDatabaseService = new LocalDatabaseService(); 
	//Veritabanı işlemleri için LocalDatabaseService sınıfından nesne oluşturulur
	
    public DictionaryPage()
    {
        InitializeComponent();
		
    }

	protected override async void OnAppearing()
	{
		//sayfa açıldığında veritabanından kelimeleri çeker ve listview'e yükler
		base.OnAppearing();
        LocalDatabaseService _localDatabaseService = new LocalDatabaseService(); 

        var words = await _localDatabaseService.GetDictionary();
		WordsListView.ItemsSource = words;
	}

    private async void AddWordBtnClicked(object sender, EventArgs e)
	{
		//Kelime ekleme sayfasını açar
        await App.Current.MainPage.Navigation.PushModalAsync(new WordAddingPage());
    }

	private async void WordsListView_ItemTapped(object sender, ItemTappedEventArgs e)
	{
		// Kelime detay sayfasını tıklanan kelimeyi paramete göndererek açar
        var item = (Dictionary)e.Item;
        var quizDates = await _localDatabaseService.GetQuizDatesForWord(item);
        
        await Navigation.PushAsync(new WordDetailPage(item,quizDates));
    }
}