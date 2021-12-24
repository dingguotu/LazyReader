using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadBookWPF.Model
{
    [Table("Book")]
    public class Book
    {
        [Key, Column(Order = 0)]
        public string? Name { get; set; }
        [Key, Column(Order = 1)]
        public string? BaseDomain { get; set; }
        public string? Path { get; set; }
        public DateTime LastReadTime { get; set; }
    }
}
