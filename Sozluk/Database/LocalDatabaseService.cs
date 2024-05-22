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
    public class LocalDatabaseService : IDisposable
    {
        
        
        private readonly SQLiteAsyncConnection _connection;
        private readonly QuizHelper _QuizHelper;
        public LocalDatabaseService()
        {
            
            string? userId = FirebaseAuthHelper.userId;
            string DB_NAME = $"{userId}_sozluk.db3";
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _QuizHelper = new QuizHelper(_connection);
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
        public void Dispose()
        {
            _connection?.CloseAsync().Wait();
        }

        public SQLiteAsyncConnection GetConnection()
        {
            return _connection;
        }

        private void CreateTableWithTimeout()
        {
            // Zaman aşımı değeri (örneğin 10 saniye)
            int timeoutMilliseconds = 10000;

            // CreateTableAsync metodu çağrılırken bir zaman aşımı uygulayın
            var createTableTask = _connection.CreateTableAsync<Models.Dictionary>();
            var createTableTask2 = _connection.CreateTableAsync<Models.QuizDates>();
            var createTableTask3 = _connection.CreateTableAsync<Models.DailyWordCounts>();
            var createTableTask4 = _connection.CreateTableAsync<Models.UserSettings>();
            var createTableTask5 = _connection.CreateTableAsync<Models.Stats>();
            // Zaman aşımı süresi içinde işlemin tamamlanıp tamamlanmadığını kontrol edin
            if (!(createTableTask.Wait(timeoutMilliseconds) && createTableTask2.Wait(timeoutMilliseconds) && createTableTask3.Wait(timeoutMilliseconds) && createTableTask4.Wait(timeoutMilliseconds) && createTableTask5.Wait(timeoutMilliseconds)))
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

        public async Task SaveDailyWordCount(int wordCount)
        {
            var settings = await _connection.Table<UserSettings>().FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new UserSettings { DailyWordCount = wordCount };
                await _connection.InsertAsync(settings);
            }
            else
            {
                settings.DailyWordCount = wordCount;
                await _connection.UpdateAsync(settings);
            }
        }

        public async Task<int> GetDailyWordCount()
        {
            var settings = await _connection.Table<UserSettings>().FirstOrDefaultAsync();
            return settings?.DailyWordCount ?? 10; // Varsayılan değer 10
        }

        public async Task<int> DecreaseDailyWordCount()
        {
            var today = DateTime.Today;
            var existingEntry = await _connection.Table<DailyWordCounts>().FirstOrDefaultAsync(d => d.Date == today);

            if (existingEntry != null)
            {
                existingEntry.WordCount--;
                await _connection.UpdateAsync(existingEntry);

                // Güncelleme işlemi sonrasında veritabanından tekrar kontrol edelim
                var updatedEntry = await _connection.Table<DailyWordCounts>().FirstOrDefaultAsync(d => d.Date == today);
                Console.WriteLine($"[DEBUG] Güncellenmiş WordCount: {updatedEntry.WordCount}");

                return updatedEntry.WordCount;
            }
            else
            {
                return 10; // Varsayılan değer
            }
        }

        public async Task InitializeDailyWordCount()
        {
            var today = DateTime.Today;

            // Bugünün tarihine ait bir kayıt olup olmadığını kontrol et
            var existingEntry = await _connection.Table<DailyWordCounts>().FirstOrDefaultAsync(d => d.Date == today);

            if (existingEntry == null)
            {
                // UserSettings tablosundan kelime sayısını al
                var settings = await _connection.Table<UserSettings>().FirstOrDefaultAsync();
                int wordCount = settings?.DailyWordCount ?? 10; // Varsayılan değer 10

                // Yeni bir kayıt oluştur ve ekle
                var newEntry = new DailyWordCounts
                {
                    Date = today,
                    WordCount = wordCount
                };
                await _connection.InsertAsync(newEntry);
            }
        }

        public async Task SaveAndUpdateDailyWordCount(int wordCount)
        {
            // UserSettings tablosunu güncelle
            var settings = await _connection.Table<UserSettings>().FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new UserSettings { DailyWordCount = wordCount };
                await _connection.InsertAsync(settings);
            }
            else
            {
                settings.DailyWordCount = wordCount;
                await _connection.UpdateAsync(settings);
            }

            // DailyWordCounts tablosunu güncelle
            var today = DateTime.Today;
            var existingEntry = await _connection.Table<DailyWordCounts>().FirstOrDefaultAsync(d => d.Date == today);
            if (existingEntry != null)
            {
                existingEntry.WordCount = wordCount;
                await _connection.UpdateAsync(existingEntry);
            }
            else
            {
                var newEntry = new DailyWordCounts
                {
                    Date = today,
                    WordCount = wordCount
                };
                await _connection.InsertAsync(newEntry);
            }
        }
        public async Task<int> GetDailyQuizCount()
        {
            var today = DateTime.Today;
            var wordCounts = await _connection.Table<DailyWordCounts>()
                                               .FirstOrDefaultAsync(d => d.Date == today);
            return wordCounts?.WordCount ?? 10; // Varsayılan değer 10
        }


        public async Task<List<Models.QuizDates>> GetWordsByLevel(int level, int quizCount)
        {
            try
            {
                // WordRepository sınıfını kullanarak kelimeleri al
                return await _QuizHelper.GetWordsByLevel(level,quizCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while getting words by level: {ex.Message}");
                return null; // Hata durumunda null dön
            }
        }

        

        public class HtmlTableParser
        {
            private readonly HttpClient _httpClient;

            public HtmlTableParser()
            {
                _httpClient = new HttpClient();
            }
            public async Task<List<Dictionary>> ParseHtmlTable(string htmlContent)
            {
                List<Dictionary> dictionaryList = new List<Dictionary>();

                var doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                foreach (HtmlNode row in doc.DocumentNode.SelectNodes("//tr"))
                {
                    HtmlNodeCollection cells = row.SelectNodes("td");
                    if (cells != null && (cells.Count == 4 || cells.Count == 5))
                    {
                        string word = cells[1].InnerText.Trim();
                        string meaning = cells[2].InnerText.Trim();
                        string example = cells[3].InnerText.Trim();
                        string imageLink = cells.Count == 5 ? cells[4].InnerText.Trim() : "";

                        string imagePath = null;
                        if (!string.IsNullOrEmpty(imageLink))
                        {
                            imagePath = await DownloadAndSaveImage(imageLink);
                        }

                        var dictionary = new Dictionary
                        {
                            Word = word,
                            Meaning = meaning,
                            Example = example,
                            Image = imagePath
                        };
                        dictionaryList.Add(dictionary);
                    }
                }

                return dictionaryList;
            }

            private async Task<string> DownloadAndSaveImage(string imageUrl)
            {
                if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out Uri uri))
                {
                    throw new ArgumentException("Invalid URL provided.");
                }

                try
                {
                    var response = await _httpClient.GetAsync(uri);
                    response.EnsureSuccessStatusCode();

                    var imageData = await response.Content.ReadAsByteArrayAsync();
                    string directoryPath = Path.Combine(FileSystem.AppDataDirectory, "images");

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    string filePath = Path.Combine(directoryPath, $"{Guid.NewGuid()}.jpg");
                    await File.WriteAllBytesAsync(filePath, imageData);
                    return filePath;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error downloading image: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<Models.Dictionary> GetDictionaryById(int id)
        {
            return await _connection.Table<Models.Dictionary>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Models.Dictionary> GetQuizDatesById(int id)
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
                await Create(new QuizDates { WordId = dictionary.Id, Level = 1 });
            }
        }

        public async Task Delete(Models.Dictionary dictionary, Models.QuizDates quizdates)
        {
            await _connection.DeleteAsync(dictionary);
            await _connection.DeleteAsync(quizdates);
        }

        public async Task Create(QuizDates quizDates)
        {
            await _connection.InsertAsync(quizDates);
        }
    }
}
