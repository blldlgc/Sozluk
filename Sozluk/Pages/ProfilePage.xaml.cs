using Sozluk.Helpers;

namespace Sozluk.Pages;

public partial class ProfilePage : ContentPage
{
	private readonly FirebaseAuthHelper _authHelper;

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
}