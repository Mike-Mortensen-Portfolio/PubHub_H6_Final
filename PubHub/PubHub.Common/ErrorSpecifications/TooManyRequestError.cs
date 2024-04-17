using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.ErrorSpecifications
{
    public static class TooManyRequestError
    {
        public const string TITLE = "Too many requests.";
        public const string ERROR_MESSAGE = $"You made too many requests. Try again later or contact PubHub support if the problem persists\nError: {ErrorsCodeConstants.TOO_MANY_REQUEST}";
        public const string BUTTON_TEXT = "OK";
    }
}
