using Microsoft.Maui.Controls;
using Sozluk.Helpers;

namespace Sozluk.Pages;


public partial class LoginPage : ContentPage
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Username { get; set; }

    public LoginPage()
    {
        InitializeComponent();

        this.BindingContext = this;
    }



    private void ButtonClickedLoginStack(object sender, EventArgs e)
    {
        //Giriş yapma ekranını açar
        stackLogin.IsVisible = true;
        stackMain.IsVisible = false;
        stackSignUp.IsVisible = false;
    }
    private void ButtonClickedSignUpStack(object sender, EventArgs e)
    {
        //Kayıt olma ekranını açar
        stackLogin.IsVisible = false;
        stackMain.IsVisible = false;
        stackSignUp.IsVisible = true;
    }

    private async void ResetPasswordBtnClicked(object sender, EventArgs e)
    {
        //Şifremi unuttum sayfasını açar
        await Navigation.PushAsync(new ResetPasswordPage());

    }

    private async void ButtonClickedSignIn(object sender, EventArgs e)
    {
        // Buton tıklandığında bekleme göstergesini görünür yap
        loadingGrid.IsVisible = true;
        loadingIndicator.IsRunning = true;
        loadingIndicator.IsVisible = true;

        //Kayıt olma işlemi için kullanıcı adı, email ve şifre bilgilerini alır ve firebase üzerinde kayıt işlemi yapar
        FirebaseAuthHelper helper = new FirebaseAuthHelper();

        var auth = await helper.Create(Username, Email, Password);

        // Giriş işlemi tamamlandığında bekleme göstergesini gizle 
        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
        loadingGrid.IsVisible = false;

        await App.Current.MainPage.DisplayAlert("Kayıt Ol", auth?.GetStatusMessage(), "OK");
    }

    private async void ButtonClickedLogin(object sender, EventArgs e)
    {
        // Buton tıklandığında bekleme göstergesini görünür yap
        loadingGrid.IsVisible = true;
        loadingIndicator.IsRunning = true;
        loadingIndicator.IsVisible = true;
        

        //Giriş yapma işlemi için email ve şifre bilgilerini alır ve firebase üzerinde giriş işlemi yapar
        FirebaseAuthHelper helper = new FirebaseAuthHelper();

        var auth = await helper.Login(Email, Password);

        // Giriş işlemi tamamlandığında bekleme göstergesini gizle 
        loadingIndicator.IsRunning = false;
        loadingIndicator.IsVisible = false;
        loadingGrid.IsVisible = false;



        if (auth == null)
        {
             await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else 
        {
            await App.Current.MainPage.DisplayAlert("Giriş Yap", auth?.GetStatusMessage(), "OK");
        }
        

        
    }
}