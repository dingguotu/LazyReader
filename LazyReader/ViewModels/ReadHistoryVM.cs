using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyReader.ViewModels
{
    public class BookReadHistoryVM
    {
        public string Title { get; set; }
        public string BaseDomain { get; set; }
        public string Path { get; set; }
        public int LastReadTime { get; set; }
        public List<ReadHistoryVM> ReadHistories { get; set; }
    }

    public class ReadHistoryVM
    {
        public string Path { get; set; }
        public int CurPageBlockIndex { get; set; }
        public string Summary { get; set; }
        public DateTime ReadTime { get; set; }
    }
}
