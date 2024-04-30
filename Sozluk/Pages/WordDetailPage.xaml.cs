using Sozluk.Models;
namespace Sozluk.Pages;

public partial class WordDetailPage : ContentPage
{
    private readonly Dictionary _word;
    private readonly Database.LocalDatabaseService _localDatabaseService = new Database.LocalDatabaseService();

    public WordDetailPage(Dictionary word)
    {
        InitializeComponent();

        _word = word;

        // Sayfa başlığını belirle
        Title = "Word Detail";

        // Detayları görüntüle
        ShowWordDetails(word);

        void ShowWordDetails(Dictionary word)
        {
            // Dictionary sınıfından gelen özellikleri kullanarak detayları görüntüle
            WordLabel.Text ="Kelime: " + word.Word;
            MeaningLabel.Text = "Anlamı: " + word.Meaning;
            ExampleLabel.Text = "Örnek cümle: " + word.Example;
            PictureImage.Source = ImageSource.FromFile(word.Image);
        }
    }

    

    private void WordTTSButton_Clicked(object sender, EventArgs e)
    {
        TextToSpeech.SpeakAsync(_word.Word);
    }

    private void ExampleTTSButton_Clicked(object sender, EventArgs e)
    {
        TextToSpeech.SpeakAsync(_word.Example);
    }

    private async void DeleteButtonClicked(object sender, EventArgs e)
    {
        var action = await DisplayAlert("Silme Onayı", "Bu kelimeyi silmek istediğinize emin misiniz?", "Evet", "Hayır");
        if (action)
        {
            // Silme işlemi için kelimeyi parametre olarak gönder
            await _localDatabaseService.Delete(_word);
            await DisplayAlert("Başarılı", "Kelime veritabanından silidi.", "Tamam");
            // Geri git
            await Navigation.PopAsync();
        }
    }

    private void EditButtonClicked(object sender, EventArgs e)
    {

    }
}