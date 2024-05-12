using Sozluk.Helpers;
using Sozluk.Database;
using Sozluk.Models;

namespace Sozluk.Pages;

public partial class ProfilePage : ContentPage
{
	private readonly FirebaseAuthHelper _authHelper;
    private readonly LocalDatabaseService _localDatabaseService = new LocalDatabaseService();


    public ProfilePage()
    {
        InitializeComponent();
    }

    public ProfilePage(FirebaseAuthHelper firebaseAuthHelper) : this()
    {
        _authHelper = firebaseAuthHelper;
    }


    private void Signout_Clicked(object sender, EventArgs e)
	{
        if (_authHelper == null)
        {
            //_authHelper.signOut();
            Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        else
        {
            // _authHelper null ise, uygun bir şekilde işleyin veya hata mesajı gösterin
        }
    }

    private async Task<string> ReadHtmlContentFromFileAsync(string filePath)
    {
        try
        {
            // Dosya yolundaki metin dosyasını oku
            string htmlContent = await File.ReadAllTextAsync(filePath);
            return htmlContent;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading HTML content from file: {ex.Message}");
            return null;
        }
    }

    private async void AddWord_Clicked(object sender, EventArgs e)
    {
        using var stream = await FileSystem.Current.OpenAppPackageFileAsync("WordList.html"); // Dosya adını doğru yazdığınızdan emin olun
        using var reader = new StreamReader(stream);
        string htmlContent = await reader.ReadToEndAsync();

        if (htmlContent != null)
        {
            // HtmlTableParser örneği oluştur
            var parser = new LocalDatabaseService.HtmlTableParser();

            // HTML içeriğini işle ve Dictionary listesi al
            List<Dictionary> dictionaryList = parser.ParseHtmlTable(htmlContent);

            // Elde edilen Dictionary listesini veritabanına ekleyebilirsiniz
            foreach (var dictionary in dictionaryList)
            {
                await _localDatabaseService.Create(dictionary);
            }
        }
        else
        {
            Console.WriteLine("HTML content could not be read from file.");
        }
    }
}