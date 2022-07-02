using System;
using System.Collections.Generic;

namespace CI.Data.EF.MongoDB.Models
{
    class IndexHeader
    {
        public IndexHeader()
        {
            Details = new HashSet<IndexDetail>();
        }

        public string Name { get; set; }

        public ICollection<IndexDetail> Details { get; set; }
    }

    class IndexDetail
    {
        public string Name { get; set; }
        public bool Ascending { get; set; }
        public Type Type { get; set; }
    }
}
