using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sozluk.Helpers
{
    public static class FileHelper
    {
        public static string SaveImageToFileSystem(byte[] imageData)
        {
            try
            {
                // Uygulamanın özel bir klasörüne dosya yolunu oluştur
                string fileName = $"{Guid.NewGuid()}.jpg"; // Rastgele bir dosya adı oluştur
                string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                // Fotoğraf verilerini dosyaya yaz
                File.WriteAllBytes(filePath, imageData);

                // Dosya yolunu döndür
                return filePath;
            }
            catch (Exception ex)
            {
                // Hata durumunda null döndür
                Console.WriteLine($"Hata: {ex.Message}");
                return null;
            }
        }
    }
}
