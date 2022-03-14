using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mustaxe.Application.Common.Exceptions
{
    public class InputValidationException : Exception
    {
        public IDictionary<string, string[]> Errors;

        public InputValidationException(IDictionary<string, string[]> errors)
        {
            Errors = errors;
        }
    }
}
