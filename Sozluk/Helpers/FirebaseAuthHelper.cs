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
    private const string AuthKey = "authState";

    private static FirebaseAuthConfig _config = new FirebaseAuthConfig()
    {
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
        try
        {
            var response = await client.CreateUserWithEmailAndPasswordAsync(email, password, username);
            
            return response?.User?.Uid?.ToString();
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    public async Task<String?>? Login(string email, string password)
    {
        try
        {
            var response = await client.SignInWithEmailAndPasswordAsync(email, password);
            var userInfo = response?.User?.Info;

            if (userInfo != null)
            {
                return null; // Başarılı
                Preferences.Default.Set<bool>(AuthKey, true);
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
        //Preferences.Default.Set<bool>(AuthKey, false);
    }


    public async Task<bool> IsAuthenticated()
    {
        


        try
        {
            await Task.Delay(1000);
            var user = client.User;

            return user != null;

        }
        catch (Exception)
        {
            return false;
        }
       
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

