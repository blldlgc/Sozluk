using Firebase.Auth;
using Firebase.Auth.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sozluk.Helpers;

public class FirebaseAuthHelper
{
    public static string? userId;
    private const string AuthKey = "authState";

    public static string? CurrentUsername { get; private set; }

    private static FirebaseAuthConfig _config = new FirebaseAuthConfig()
    {
        // Firebase Console ile uygulamayı bağlama işlemi
        ApiKey = "AIzaSyAbW7ocZLO9SziPuJljvurRnyU2fNLkjns",
        AuthDomain = "sozluk-61e07.firebaseapp.com",
        Providers = new FirebaseAuthProvider[]
        {
            new EmailProvider()
        }
    };

    FirebaseAuthClient client = new FirebaseAuthClient(_config);

    public async Task<String?>? Create(string username, string email, string password)
    {
        // Firebase üzerinde kullanıcı oluşturma işlemi
        try
        {
            var response = await client.CreateUserWithEmailAndPasswordAsync(email, password, username);

            CurrentUsername = username;

            return response?.User?.Uid?.ToString();
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public async Task<String?>? Login(string email, string password)
    {
        // Firebase üzerinde kullanıcı girişi işlemi
        try
        {
            var response = await client.SignInWithEmailAndPasswordAsync(email, password);
            var userInfo = response?.User?.Info;

            if (userInfo != null)
            {
                
                Preferences.Default.Set<bool>(AuthKey, true);
                userId = userInfo.Uid;

                CurrentUsername = userInfo.DisplayName;
                Preferences.Default.Set<bool>(AuthKey, true);
                return null; // Başarılı
            }
            else
            {
                return "Giriş yapılamadı.";
            }


        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public void signOut()
    {
        client.SignOut();
        Preferences.Default.Set<bool>(AuthKey, false);
    }

    public async Task<string?> ResetPassword(string email)
    {
        try
        {
            await client.ResetEmailPasswordAsync(email);
            return null; // No error occurred
        }
        catch (Exception e)
        {
            return e.Message; // Return the error message
        }
    }



}

