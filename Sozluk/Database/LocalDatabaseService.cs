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

        public async Task UpdateQuizDates(int wordId)
        {
            

            // Get existing QuizDates record for the word
            var existingDates = await _connection.Table<QuizDates>().Where(q => q.WordId == wordId).FirstOrDefaultAsync();

            if (existingDates == null)
            {
                // Create a new QuizDates record if it doesn't exist
                existingDates = new QuizDates { WordId = wordId };
            }

            var today = DateTime.Today;

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

        public async Task UpdateLevel(int wordId)
        {
            var quizDates = await _connection.Table<QuizDates>().Where(q => q.WordId == wordId).FirstOrDefaultAsync();

            if (quizDates != null)
            {
                // Get the current level
                int currentLevel = quizDates.Level;

                if (currentLevel == 1) { UpdateQuizDates(wordId); }

                // Increase the level by 1
                int newLevel = currentLevel + 1;

                // Update the level in the database
                quizDates.Level = newLevel;
                await _connection.UpdateAsync(quizDates);
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
                    string filePath = $"path/to/save/{Guid.NewGuid()}.jpg"; // Dosya yolu düzenlenmeli

                    await File.WriteAllBytesAsync(filePath, imageData);
                    return filePath;
                }
                catch (Exception ex)
                {
                    // Hata yönetimi
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

        public async Task<List<string>> GetRandomMeanings(int count, string excludeMeaning)
        {
            var allMeanings = await _connection.Table<Models.Dictionary>()
                                               .Where(x => x.Meaning != excludeMeaning)
                                               .ToListAsync();

            var randomMeanings = allMeanings.OrderBy(x => Guid.NewGuid()).Take(count).Select(x => x.Meaning).ToList();
            return randomMeanings;
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
