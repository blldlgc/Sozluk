using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;


namespace Sozluk.Models
{
    public class Dictionary
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Word { get; set; }
        public string Meaning { get; set; }
        public string Example { get; set; }
        public string PictureLink { get; set; }
    }
}
