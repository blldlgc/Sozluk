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

        public async Task ResetLevelAndDates(int id)
        {
            var quizDates = await _connection.Table<QuizDates>().Where(q => q.WordId == id).FirstOrDefaultAsync();
            List<Models.QuizDates> words = new List<Models.QuizDates>();


            // Level ve tarihi veritabanında sıfırlama
            quizDates.Level = 1;
            quizDates.date1 = DateTime.MinValue;
            quizDates.date2 = DateTime.MinValue;
            quizDates.date3 = DateTime.MinValue;
            quizDates.date4 = DateTime.MinValue;
            quizDates.date5 = DateTime.MinValue;
            quizDates.date6 = DateTime.MinValue;
            quizDates.date7 = DateTime.MinValue;
            await _connection.UpdateAsync(quizDates);

        }

        public async Task IncrementStats(int level)
        {
            var today = DateTime.Today;

            // Bugünün tarihindeki kaydı al
            var stats = await _connection.Table<Stats>().FirstOrDefaultAsync(s => s.Date == today);

            if (stats == null)
            {
                // Bugünün tarihindeki kayıt yoksa oluştur
                stats = new Stats
                {
                    Date = today
                };
                await _connection.InsertAsync(stats);
            }

            // Seviyeye göre ilgili sayacı artır
            switch (level)
            {
                case 1:
                    stats.Level1Count++;
                    break;
                case 2:
                    stats.Level2Count++;
                    break;
                case 3:
                    stats.Level3Count++;
                    break;
                case 4:
                    stats.Level4Count++;
                    break;
                case 5:
                    stats.Level5Count++;
                    break;
                case 6:
                    stats.Level6Count++;
                    break;
                case 7:
                    stats.Level7Count++;
                    break;
                default:
                    throw new ArgumentException("Invalid level");
            }

            // Güncellenen kaydı veritabanına yaz
            await _connection.UpdateAsync(stats);
        }

        public async Task<Stats> GetTotalStatsAsync()
        {
            var totalStats = new Stats();
            var allStats = await _connection.Table<Stats>().ToListAsync();

            foreach (var stats in allStats)
            {
                totalStats.Level1Count += stats.Level1Count;
                totalStats.Level2Count += stats.Level2Count;
                totalStats.Level3Count += stats.Level3Count;
                totalStats.Level4Count += stats.Level4Count;
                totalStats.Level5Count += stats.Level5Count;
                totalStats.Level6Count += stats.Level6Count;
                totalStats.Level7Count += stats.Level7Count;
            }

            return totalStats;
        }

        public async Task<List<Stats>> GetDailyStatsAsync()
        {
            return await _connection.Table<Stats>().OrderByDescending(s => s.Date).ToListAsync();
        }




    }
}
