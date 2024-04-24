using Sozluk.Helpers;

namespace Sozluk.Pages;

public partial class LoadingPage : ContentPage
{
	private readonly FirebaseAuthHelper _firebaseAuthHelper;

	public LoadingPage(FirebaseAuthHelper firebaseAuthHelper)
	{
		InitializeComponent();
		_firebaseAuthHelper = firebaseAuthHelper;
	}

	protected async override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		if(await _firebaseAuthHelper.IsAuthenticated())
		{
			//kullanıcı giriş yapmış
            await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
        }
        else
		{
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
	}
}