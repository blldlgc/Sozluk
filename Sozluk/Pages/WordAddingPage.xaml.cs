using Sozluk.Database;


namespace Sozluk.Pages;

public partial class WordAddingPage : ContentPage
{

    private readonly Database.LocalDatabaseService _localDatabaseService;
    private int _editId;
    private byte[] photoData;


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

	private async void BackBtnClicked(object sender, EventArgs e)
	{
        
        await App.Current.MainPage.Navigation.PopModalAsync();
    }

    private async void SaveBtnClicked(object sender, EventArgs e)
    {
        try
        {
            if (_editId == 0)
            {
                // Yeni kelime ekleme işlemi
                await _localDatabaseService.Create(new Models.Dictionary
                {
                    Word = nameEntryField.Text,
                    Meaning = meaningEntryField.Text,
                    Example = exampleEntryField.Text,
                    //Image = photoData
                });
            }
            else
            {
                // Kelime güncelleme işlemi
                await _localDatabaseService.Update(new Models.Dictionary
                {
                    Id = _editId,
                    Word = nameEntryField.Text,
                    Meaning = meaningEntryField.Text,
                    Example = exampleEntryField.Text,
                    //Image = photoData
                });

                _editId = 0;
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
        await App.Current.MainPage.Navigation.PopModalAsync();
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
                }
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}