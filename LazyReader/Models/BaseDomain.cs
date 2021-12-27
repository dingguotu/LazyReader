using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyReader.Models
{
    [Table("BaseDomain")]
    public class BaseDomain
    {
        [Key]
        public Guid Id { get; set; }
        public string? DomainName { get; set; }
        public string? BaseUrl { get; set; }
    }
}
