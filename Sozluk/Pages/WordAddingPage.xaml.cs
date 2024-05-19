using Sozluk.Database;
using Sozluk.Models;


namespace Sozluk.Pages;

public partial class WordAddingPage : ContentPage
{
    private readonly Dictionary _word;
    private readonly LocalDatabaseService _localDatabaseService;
    private byte[] photoData;
    private string imagePath;


    public WordAddingPage(LocalDatabaseService dbService)
	{
		InitializeComponent();
        _localDatabaseService = dbService;
        
    }

    public WordAddingPage()
    {
           InitializeComponent();
        _localDatabaseService = new LocalDatabaseService();
    }



    private async void SaveBtnClicked(object sender, EventArgs e)
    {
        // Kaydet butonuna basıldığında kelimeyi veritabanına ekler
        try
        {
            // Boş değer kontrolleri
            if (string.IsNullOrWhiteSpace(nameEntryField.Text))
            {
                await App.Current.MainPage.DisplayAlert("Hata", "Lütfen kelime girin.", "Tamam");
                return; // İşlem durdurulur
            }
            if (string.IsNullOrWhiteSpace(meaningEntryField.Text))
            {
                await App.Current.MainPage.DisplayAlert("Hata", "Lütfen anlam girin.", "Tamam");
                return; // İşlem durdurulur
            }
            if (string.IsNullOrWhiteSpace(exampleEntryField.Text))
            {
                await App.Current.MainPage.DisplayAlert("Hata", "Lütfen örnek girin.", "Tamam");
                return; // İşlem durdurulur
            }
            await _localDatabaseService.Create(new Models.Dictionary
            {
                Word = nameEntryField.Text,
                Meaning = meaningEntryField.Text,
                Example = exampleEntryField.Text,
                Image = imagePath
            });
            int wordId = await getId(nameEntryField.Text);
            App.Current.MainPage.DisplayAlert("Kelime veritabanına başarıyla Eklnedi","Kelime Id'si:"+ wordId.ToString(), "Tamam"); //TODO silinecek
            
        }
        catch (Exception ex)
        {
            // Hata durumunda hata mesajını gösterir
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        await App.Current.MainPage.Navigation.PopModalAsync();
    }

    private async Task<int> getId(string word)
    {
        // Veritabanında kelimenin ID'sini almak için sorgu yapılır
        var dictionary = (await _localDatabaseService.GetDictionary()).FirstOrDefault(d => d.Word == word);

       

        // Kelime bulunduysa ID'si döndürülür, bulunamadıysa -1 döndürülür veya hata durumunda -1 döndürülür.
        return dictionary != null ? dictionary.Id : -1;
        
    }




    private async void PhotoBtnClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Please pick a photo"
            });
            if (result != null)
            {
                // Seçilen fotoğrafın işlenmesi
                var stream = await result.OpenReadAsync();
                using (MemoryStream ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    photoData = ms.ToArray();
                    // Burada fotoğraf verilerini kullanabilirsiniz
                    var imageSource = ImageSource.FromStream(() => new MemoryStream(photoData));
                    selectedImage.Source = imageSource;

                    imagePath = Helpers.FileHelper.SaveImageToFileSystem(photoData);
                }
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}