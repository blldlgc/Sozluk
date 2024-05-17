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

        public async Task<List<Models.QuizDates>> GetWordsByLevel(int level, int quizCount)
        {
            List<Models.QuizDates> words = new List<Models.QuizDates>();
            var today = DateTime.Today;
            switch (level)
            {
                case 1:
                    // Level 1 kelimeleri çek
                    words = await _connection.Table<QuizDates>().Where(q => q.Level == 1).ToListAsync();
                    var random = new Random();
                    words = words.OrderBy(x => random.Next()).Take(quizCount).ToList();                
                    break;
                case 2:
                    // Level 2 kelimeleri çek (date2 değeri bugünün tarihine eşit olanlar)
                    words = await _connection.Table<Models.QuizDates>().Where(w => w.Level == 2 && w.date2 == today).ToListAsync();
                    break;
                case 3:
                    // Level 3 kelimeleri çek (date2 değeri bugünün tarihine eşit olanlar)
                    words = await _connection.Table<Models.QuizDates>().Where(w => w.Level == 3 && w.date3 == today).ToListAsync();
                    break;
                case 4:
                    // Level 4 kelimeleri çek (date2 değeri bugünün tarihine eşit olanlar)
                    words = await _connection.Table<Models.QuizDates>().Where(w => w.Level == 4 && w.date4 == today).ToListAsync();
                    break;
                case 5:
                    // Level 5 kelimeleri çek (date2 değeri bugünün tarihine eşit olanlar)
                    words = await _connection.Table<Models.QuizDates>().Where(w => w.Level == 5 && w.date5 == today).ToListAsync();
                    break;
                case 6:
                    // Level 6 kelimeleri çek (date2 değeri bugünün tarihine eşit olanlar)
                    words = await _connection.Table<Models.QuizDates>().Where(w => w.Level == 6 && w.date6 == today).ToListAsync();
                    break;
                case 7:
                    // Level 7 kelimeleri çek (date2 değeri bugünün tarihine eşit olanlar)
                    words = await _connection.Table<Models.QuizDates>().Where(w => w.Level == 7 && w.date7 == today).ToListAsync();
                    break;
                default:
                    break;
            }

            return words;
        }


        

    }
}
