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

    private async void SaveBtnClicked(object sender, EventArgs e)
    {
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
                    byte[] photoData = ms.ToArray();
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