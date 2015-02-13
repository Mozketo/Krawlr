using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krawlr.Core.Handlers
{
    public interface IHandler
    {
        string Url { get; set; }
        void Act<T>(Action<T> action);
    }
}
