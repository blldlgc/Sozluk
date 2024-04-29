using Sozluk.Helpers;
namespace Sozluk.Pages;

public partial class ResetPasswordPage : ContentPage
{
    public string Email { get; set; }
    public ResetPasswordPage()
	{
		InitializeComponent();
	}

    private async void resetPassword_Clicked(object sender, EventArgs e)
    {
        FirebaseAuthHelper helper = new FirebaseAuthHelper();
        var resetStatus = await helper.ResetPassword(email.Text);

        await App.Current.MainPage.DisplayAlert("Gönderdiğimiz e-posta'daki link üzerinden şifre yenileme işleminizi tamamlayabiirsiniz", resetStatus?.GetStatusMessage(), "OK");
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        //await Navigation.GoBack();
        
        await App.Current.MainPage.Navigation.PushAsync(new NavigationPage(new LoginPage()));
    }
}