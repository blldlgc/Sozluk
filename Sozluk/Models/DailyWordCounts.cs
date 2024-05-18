using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Sozluk.Models
{
    [Table("wordcounts")]
    public class DailyWordCounts
    {
        [PrimaryKey, AutoIncrement, Column("dayId")]
        public int Id { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("wordCount")]
        public int WordCount { get; set; }
    }
}
