using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyReader.Models
{
    [Table("Book")]
    public class Book
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string BaseDomain { get; set; }
        [Required]
        public string Path { get; set; }
        public DateTime LastReadTime { get; set; }
    }
}
