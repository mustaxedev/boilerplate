using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mustaxe.Application.Common.Utils
{
    public static class RegexUtils
    {
        public static Regex OnlyNumbersRegex => new Regex("^[0-9]*$");
    }
}
