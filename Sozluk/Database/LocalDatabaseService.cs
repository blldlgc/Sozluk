using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sozluk.Helpers;
using Sozluk.Models;

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
                // Belirli bir kelimeye ait QuizDates nesnesini almak için veritabanı sorgusu yapın
                // Diyelim ki QuizDates tablosunda kelimeye göre filtreleme yapacağız
                var quizDates = await _connection.Table<QuizDates>().Where(q => q.WordId == word.Id).FirstOrDefaultAsync();

                return quizDates;
            }
            catch (Exception ex)
            {
                // Hata oluştuğunda burada işleyin (loglama veya kullanıcıya bildirim gibi)
                Console.WriteLine("Error occurred while fetching QuizDates for word: " + ex.Message);
                return null; // Hata durumunda null döndür
            }
        }


        public async Task<Models.Dictionary> GetDictionaryById(int id)
        {
            return await _connection.Table<Models.Dictionary>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(Models.Dictionary dictionary)
        {
            await _connection.InsertAsync(dictionary);
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
