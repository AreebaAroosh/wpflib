using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WPFLib.Contracts
{
    public interface IAsyncSearchListBoxController
    {
        Task<IEnumerable<object>> GetResults(CancellationTokenSource token, string searchQuery, int maxResults);
    }
}
