using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotesOfWisdom.Common
{
    public interface IPagedResponse<T>
    {
        IEnumerable<T> Items { get; }
        int VirtualCount { get; }
    }
}
