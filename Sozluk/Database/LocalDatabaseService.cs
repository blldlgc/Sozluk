using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sozluk.Database
{
    public class LocalDatabaseService
    {
        private const string DB_NAME = "sozluk.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDatabaseService()
        {
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
        private async Task CreateTableAsync()
        {
            await _connection.CreateTableAsync<Models.Dictionary>();
        }

        private void CreateTableWithTimeout()
        {
            // Zaman aşımı değeri (örneğin 10 saniye)
            int timeoutMilliseconds = 10000;

            // CreateTableAsync metodu çağrılırken bir zaman aşımı uygulayın
            var createTableTask = _connection.CreateTableAsync<Models.Dictionary>();
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
        }

        public async Task<List<Models.Dictionary>> GetDictionary()
        {
            return await _connection.Table<Models.Dictionary>().ToListAsync();
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
    }
}
