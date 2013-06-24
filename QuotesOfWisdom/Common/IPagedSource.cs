using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotesOfWisdom.Common
{
    public interface IPagedSource<T>
    {
        Task<IPagedResponse<T>> GetPage(string query, int pageIndex, int pageSize);
    }
}
