
using Sozluk.Models;
namespace Sozluk.Pages;

public partial class WordDetailPage : ContentPage
{
    private readonly Dictionary _word;
    private readonly QuizDates _quizDates;
    private readonly Database.LocalDatabaseService _localDatabaseService = new Database.LocalDatabaseService();

    public WordDetailPage(Dictionary word, QuizDates quizDates)
    {
        InitializeComponent();

        _word = word;
        _quizDates = quizDates;

        // Sayfa başlığını belirle
        Title = "Word Detail";

        // Detayları görüntüle
        ShowWordDetails(word);
        ShowQuizDetails(quizDates);

        void ShowWordDetails(Dictionary word)
        {
            // Dictionary sınıfından gelen özellikleri kullanarak detayları görüntüle
            IdLabel.Text = "ID: " + word.Id;
            WordLabel.Text ="Kelime: " + word.Word;
            MeaningLabel.Text = "Anlamı: " + word.Meaning;
            ExampleLabel.Text = "Örnek cümle: " + word.Example;
            PictureImage.Source = ImageSource.FromFile(word.Image);
        }
        void ShowQuizDetails(QuizDates quizDates)
        {
            // Dictionary sınıfından gelen özellikleri kullanarak detayları görüntüle
            LevelLabel.Text = "Level: " + quizDates.Level;
            
            for (int i = 1; i <= 7; i++)
            {
                var dateProperty = typeof(QuizDates).GetProperty("date" + i);
                var date = (DateTime)dateProperty.GetValue(quizDates);

                var dateLabel = (Label)FindByName("Date" + i + "Label");

                if (date != DateTime.MinValue)
                {
                    dateLabel.Text = "Date " + i + ": " + date.ToString("dd/MM/yyyy");
                }
                else
                {
                    dateLabel.Text = "Date " + i + ": Not set";
                }
            }
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
            await _localDatabaseService.Delete(_word, _quizDates);
            await DisplayAlert("Başarılı", "Kelime veritabanından silidi.", "Tamam");
            // Geri git
            await Navigation.PopAsync();
        }
    }

    private async void EditButtonClicked(object sender, EventArgs e)
    {

        // Update quiz dates using the retrieved ID and new dates
        await _localDatabaseService.UpdateQuizDates(_word.Id);

        await _localDatabaseService.UpdateLevel(_word.Id);

        // Display success message or handle errors
        await DisplayAlert("Başarılı", "Quiz tarihleri güncellendi.", "Tamam");

    }
}