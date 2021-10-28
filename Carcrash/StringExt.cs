using System;
using System.Collections.Generic;
using System.Text;

namespace Carcrash
{
    public static class StringExt
    {
        public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "...")
        {
            return value?.Length > maxLength
                ? value.Substring(0, maxLength) + truncationSuffix
                : value;
        }
    }

}
