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
        stackLogin.IsVisible = true;
        stackMain.IsVisible = false;
        stackSignUp.IsVisible = false;
    }
    private void ButtonClickedSignUpStack(object sender, EventArgs e)
    {
        stackLogin.IsVisible = false;
        stackMain.IsVisible = false;
        stackSignUp.IsVisible = true;
    }

    private async void ResetPasswordBtnClicked(object sender, EventArgs e)
    {
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new ResetPasswordPage()));
        //await Navigation.PushAsync(new NavigationPage(new ResetPasswordPage()));

    }

    private async void ButtonClickedSignIn(object sender, EventArgs e)
    {

        FirebaseAuthHelper helper = new FirebaseAuthHelper();

        var auth = await helper.Create(Username, Email, Password);

        await App.Current.MainPage.DisplayAlert("Login", auth?.GetStatusMessage(), "OK");
    }

    private async void ButtonClickedLogin(object sender, EventArgs e)
    {
        
        FirebaseAuthHelper helper = new FirebaseAuthHelper();

        var auth = await helper.Login(Email, Password);

        if (auth == null)
        {
             await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else 
        {
            await App.Current.MainPage.DisplayAlert("Login", auth?.GetStatusMessage(), "OK");
        }
        

        
    }
}