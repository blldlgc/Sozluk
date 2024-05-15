using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sozluk.Models;    

namespace Sozluk.Database
{
    public class QuizHelper
    {
        private readonly SQLiteAsyncConnection _connection;


        public QuizHelper(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task<List<Models.QuizDates>> GetWordsByLevel(int level)
        {
            List<Models.QuizDates> words = new List<Models.QuizDates>();

            switch (level)
            {
                case 1:
                    // Level 1 kelimeleri çek
                    words = await _connection.Table<QuizDates>().Where(q => q.Level == 1).ToListAsync();
                    break;
                case 2:
                    // Level 2 kelimeleri çek (date2 değeri bugünün tarihine eşit olanlar)
                    var today = DateTime.Today;
                    words = await _connection.Table<Models.QuizDates>().Where(w => w.Level == 2 && w.date2 == today).ToListAsync();
                    break;
                // Diğer seviyeler için aynı mantıkla devam edebilirsiniz
                default:
                    break;
            }

            return words;
        }
    }
}
