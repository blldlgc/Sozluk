using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sozluk.Helpers
{
    public static class FirebaseExceptionHelper
    {
        public static string GetStatusMessage(this string message)
        {
            var resultMessage = string.Empty;
            if (message == null) resultMessage += "Bir hata oluştu. Lütfen daha sonra tekrar deneyin";

            if (message.Contains("MISSING_EMAIL"))
            {
                resultMessage += "e-posta adresi giriniz";
            }
            if (message.Contains("INVALID_EMAIL"))
            {
                resultMessage += "Geçersiz e-posta adresi";
            }
            else if (message.Contains("EMAIL_NOT_FOUND"))
            {
                resultMessage += "E-posta adresi bulunamadı";
            }
            else if (message.Contains("INVALID_PASSWORD"))
            {
                resultMessage += "Geçersiz şifre";
            }
            else if (message.Contains("EMAIL_EXISTS"))
            {
                resultMessage += "E-posta adresi zaten kullanımda";
            }
            else if (message.Contains("WEAK_PASSWORD"))
            {
                resultMessage += "Zayıf şifre";
            }
            else if (message.Contains("TOO_MANY_ATTEMPTS_TRY_LATER"))
            {
                resultMessage += "Çok fazla deneme yaptınız. Lütfen daha sonra tekrar deneyin";
            }
            else if (message.Contains("INVALID_LOGIN_CREDENTIALS"))
            {
                resultMessage += "Kullanıcı adı veya şifre hatalı.";
            }
            else if (message.Contains("USER_DISABLED"))
            {
                resultMessage += "Kullanıcı hesabı devre dışı bırakıldı.";
            }
            else if (message.Contains("USER_NOT_FOUND"))
            {
                resultMessage += "Kullanıcı bulunamadı.";
            }
            else if (message.Contains("INVALID_ID_TOKEN"))
            {
                resultMessage += "Geçersiz kimlik belirteci.";
            }
            else if (message.Contains("MISSING_PASSWORD"))
            {
                resultMessage += "Şifre giriniz.";
            }
            else if (message.Contains("MISSING_EMAIL"))
            {
                resultMessage += "E-posta adresi giriniz.";
            }
            else if (message.Contains("MISSING_USERNAME"))
            {
                resultMessage += "Kullanıcı adı giriniz.";
            }
            else if (message.Contains("MISSING_DISPLAY_NAME"))
            {
                resultMessage += "Ad ve soyad giriniz.";
            }
            else if (message.Contains("MISSING_PHONE_NUMBER"))
            {
                resultMessage += "Telefon numarası giriniz.";
            }
            else if (message.Contains("MISSING_VERIFICATION_CODE"))
            {
                resultMessage += "Doğrulama kodu giriniz.";
            }
            else if (message.Contains("MISSING_VERIFICATION_ID"))
            {
                resultMessage += "Doğrulama kimlik belirteci giriniz.";
            }
            else if (message.Contains("INVALID_VERIFICATION_CODE"))
            {
                resultMessage += "Geçersiz doğrulama kodu.";
            }
            else if (message.Contains("INVALID_VERIFICATION_ID"))
            {
                resultMessage += "Geçersiz doğrulama kimlik belirteci.";
            }
            else if (message.Contains("INVALID_PHONE_NUMBER"))
            {
                resultMessage += "Geçersiz telefon numarası.";
            }
            else if (message.Contains("PHONE_NUMBER_ALREADY_EXISTS"))
            {
                resultMessage += "Telefon numarası zaten kullanımda.";
            }
            else if (message.Contains("PHONE_NUMBER_NOT_FOUND"))
            {
                resultMessage += "Telefon numarası bulunamadı.";
            }
            else if (message.Contains("INVALID_IDP_RESPONSE"))
            {
                resultMessage += "Geçersiz kimlik sağlayıcı yanıtı";

            }

            return !string.IsNullOrEmpty(resultMessage) ? resultMessage : message;

        }
    }
}
