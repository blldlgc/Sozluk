using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Sozluk.Models
{
    [Table("stats")]
    public class Stats
    {
        [PrimaryKey, AutoIncrement, Column("dayId")]
        public int Id { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("level1")]
        public int Level1Count { get; set; } = 0;

        [Column("level2")]
        public int Level2Count { get; set; } = 0;

        [Column("level3")]
        public int Level3Count { get; set; } = 0;

        [Column("level4")]
        public int Level4Count { get; set; } = 0;

        [Column("level5")]
        public int Level5Count { get; set; } = 0;

        [Column("level6")]
        public int Level6Count { get; set; } = 0;

        [Column("level7")]
        public int Level7Count { get; set; } = 0;
    }
}
