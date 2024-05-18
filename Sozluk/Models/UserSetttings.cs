using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace Sozluk.Models
{
    [Table("usersettings")]
    public class UserSettings
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }

        [Column("dailyWordCount")]
        public int DailyWordCount { get; set; } = 10; // Varsayılan değer 10
    }
}
