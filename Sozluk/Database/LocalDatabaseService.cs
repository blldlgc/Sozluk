using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sozluk.Helpers;
using Sozluk.Models;
using System.Xml;
using HtmlAgilityPack;

namespace Sozluk.Database
{
    public class LocalDatabaseService
    {
        
        
        private readonly SQLiteAsyncConnection _connection;

        public LocalDatabaseService()
        {
            
            string? userId = FirebaseAuthHelper.userId;
            string DB_NAME = $"{userId}_sozluk.db3";
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            //_connection.CreateTableAsync<Models.Dictionary>();
            //CreateTableAsync().Wait();

            try
            {
                CreateTableWithTimeout();
            }catch(Exception ex)
            {
                 App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            
        }

        private void CreateTableWithTimeout()
        {
            // Zaman aşımı değeri (örneğin 10 saniye)
            int timeoutMilliseconds = 10000;

            // CreateTableAsync metodu çağrılırken bir zaman aşımı uygulayın
            var createTableTask = _connection.CreateTableAsync<Models.Dictionary>();
            var createTableTask2 = _connection.CreateTableAsync<Models.QuizDates>();
            // Zaman aşımı süresi içinde işlemin tamamlanıp tamamlanmadığını kontrol edin
            if (createTableTask.Wait(timeoutMilliseconds))
            {
                // CreateTableAsync metodu başarıyla tamamlandı
            }
            else
            {
                // Zaman aşımı gerçekleşti, bir hata oluşturun
                throw new TimeoutException("CreateTableAsync operation timed out.");
            }
            if (createTableTask2.Wait(timeoutMilliseconds))
            {
                // CreateTableAsync metodu başarıyla tamamlandı
            }
            else
            {
                // Zaman aşımı gerçekleşti, bir hata oluşturun
                throw new TimeoutException("CreateTableAsync operation timed out.");
            }
        }

        public async Task<List<Models.Dictionary>> GetDictionary()
        {
            var dictionaryList = await _connection.Table<Models.Dictionary>().ToListAsync();
            return dictionaryList;

        }

        public async Task<QuizDates> GetQuizDatesForWord(Dictionary word)
        {
            try
            {
                var quizDates = await _connection.Table<QuizDates>().Where(q => q.WordId == word.Id).FirstOrDefaultAsync();

                // Eğer quizDates null ise, varsayılan bir QuizDates nesnesi döndürün:
                return quizDates ?? new QuizDates { WordId = word.Id };

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while fetching QuizDates for word: " + ex.Message);
                return null; // Hata durumunda null döndür
            }
        }

        public async Task UpdateQuizDates(int wordId)
        {
            

            // Get existing QuizDates record for the word
            var existingDates = await _connection.Table<QuizDates>().Where(q => q.WordId == wordId).FirstOrDefaultAsync();

            if (existingDates == null)
            {
                // Create a new QuizDates record if it doesn't exist
                existingDates = new QuizDates { WordId = wordId };
            }

            var today = DateTime.Now;

            // Update each date property with the corresponding new date
            existingDates.date1 = today;
            existingDates.date2 = today.AddDays(1);
            existingDates.date3 = today.AddDays(7);
            existingDates.date4 = today.AddMonths(1);
            existingDates.date5 = today.AddMonths(3);
            existingDates.date6 = today.AddMonths(6);
            existingDates.date7 = today.AddYears(1);

            // Update the entire record in the database
            await _connection.UpdateAsync(existingDates);
        }

        public class HtmlTableParser
        {
            public async Task<List<Dictionary>> ParseHtmlTable(string htmlContent)
            {
                List<Dictionary> dictionaryList = new List<Dictionary>();

                // HtmlAgilityPack kullanarak HTML içeriğini işle
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                // Tablodaki her satırı döngüye al
                foreach (HtmlNode row in doc.DocumentNode.SelectNodes("//tr"))
                {
                    // Tablodaki sütunları döngüye al
                    HtmlNodeCollection cells = row.SelectNodes("td");
                    if (cells != null && (cells.Count == 4 || cells.Count == 5)) // Her satırın 4 sütunu olduğunu varsayalım
                    {
                        // Sütunlardan verileri al
                        string word = cells[1].InnerText.Trim();
                        string meaning = cells[2].InnerText.Trim();
                        string example = cells[3].InnerText.Trim();
                        string imageLink = "";

                        if (cells.Count == 5)
                        {
                            imageLink = cells[4].InnerText.Trim();
                        }

                        // Fotoğrafı indir ve kaydet
                        string imagePath = await DownloadAndSaveImage(imageLink);

                        // Yeni bir Dictionary nesnesi oluştur ve listeye ekle
                        var dictionary = new Dictionary
                        {
                            Word = word,
                            Meaning = meaning,
                            Example = example,
                            Image = imagePath // Kaydedilen fotoğrafın yolunu ekle
                        };
                        dictionaryList.Add(dictionary);
                    }
                }

                return dictionaryList;
            }

            private async Task<string> DownloadAndSaveImage(string imageLink)
            {
                try
                {
                    using var httpClient = new HttpClient();
                    var imageData = await httpClient.GetByteArrayAsync(imageLink);

                    // Uygulama verilerine kaydedilecek dosya yolu
                    string fileName = Path.GetFileName(imageLink);
                    string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                    // Dosyayı kaydet
                    await File.WriteAllBytesAsync(filePath, imageData);

                    return filePath;
                }
                catch (Exception ex)
                {
                    // Hata işleme
                    Console.WriteLine($"Fotoğraf indirilemedi: {ex.Message}");
                    return null; // veya hata durumunda varsayılan bir resim yolu
                }
            }
        }



        public async Task<Models.Dictionary> GetDictionaryById(int id)
        {
            return await _connection.Table<Models.Dictionary>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(Models.Dictionary dictionary)
        {
            var existingWord = await _connection.Table<Models.Dictionary>()
                                       .Where(d => d.Word.ToLower() == dictionary.Word.ToLower()) // Büyük/küçük harf duyarsız karşılaştırma
                                       .FirstOrDefaultAsync();

            if (existingWord == null)
            {
                // Kelime yoksa ekle
                await _connection.InsertAsync(dictionary);
                await Create(new QuizDates { WordId = dictionary.Id, Level = 0 });
            }
            else
            {
                // Kelime varsa, isteğe bağlı olarak kullanıcıya bilgi verebilirsiniz:
                //App.Current.MainPage.DisplayAlert("Bilgi", $"{dictionary.Word} kelimesi zaten sözlükte mevcut!", "Tamam");
            }

        }



        public async Task Update(Models.Dictionary dictionary)
        {
            await _connection.UpdateAsync(dictionary);
        }

        public async Task Delete(Models.Dictionary dictionary)
        {
            await _connection.DeleteAsync(dictionary);
        }

        public async Task Create(QuizDates quizDates)
        {
            await _connection.InsertAsync(quizDates);
        }
    }
}
