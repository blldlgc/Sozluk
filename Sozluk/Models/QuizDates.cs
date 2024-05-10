using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Sozluk.Models
{
    
        [Table("quizdates")]
        public class QuizDates
        {
            [PrimaryKey, Column("id")]
            public int WordId { get; set; }
            [Column("wordLevel")]
            public int Level { get; set; }

            [Column("date1")]
            public DateTime date1 { get; set; }
            [Column("date2")]
            public DateTime date2 { get; set; }
            [Column("date3")]
            public DateTime date3 { get; set; }
            [Column("date4")]
            public DateTime date4 { get; set; }
            [Column("date5")]
            public DateTime date5 { get; set; }
            [Column("date6")]
            public DateTime date6 { get; set; }
            [Column("date7")]
            public DateTime date7 { get; set; }

    }
}
