using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace Sozluk.Models
{
    [Table("dictionary")]
    public class Dictionary
    {
        [PrimaryKey, AutoIncrement, Column("id")]
        public int Id { get; set; }
        [Column("word")]
        public string Word { get; set; }
        [Column("meaning")]
        public string Meaning { get; set; }
        [Column("example")]
        public string Example { get; set; }
        [Column("pictureLink")]
        public string Image { get; set; }
    }
}
