using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krawlr.Core.Extensions
{
    public static class FileEx
    {
        public static bool ExistsEx(this string path)
        {
            bool result = path.HasValue() && File.Exists(path);
            return result;
        }
    }
}
