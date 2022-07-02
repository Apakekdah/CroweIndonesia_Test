using System;
using System.Collections.Generic;
using System.Text;

namespace CI.Interface
{
    public interface IPaginator
    {
        int Page { get; set; }
        int PageSize { get; set; }
    }
}