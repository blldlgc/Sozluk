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
        private const string DatabaseFileName = "Sozluk.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDatabaseService()
        {
            _connection = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            _connection.CreateTableAsync<Models.Dictionary>();
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
            await _connection.InsertAsync(dictionary);
        }

        public async Task Delete(Models.Dictionary dictionary)
        {
            await _connection.DeleteAsync(dictionary);
        }
    }
}
