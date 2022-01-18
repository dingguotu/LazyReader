using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyReader.Models
{
    [Table("BookChapter")]
    public class BookChapter
    {
        [Key, Column(Order = 1)]
        public Guid BookId { get; set; }
        [Key, Column(Order = 2)]
        public int Index { get; set; }
        public string Title { get; set; }
        public string Path { get; set; } = "";
    }
}
