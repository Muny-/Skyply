using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyply
{
    public static class ExtensionMethods
    {
        public static String CleanForJavascript(this String source)
        {
            return source.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r\n", "<br>").Replace("\n", "<br>").Trim();
        }
    }
}
