using System.IO;
using MZMemoize.Extensions;

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
