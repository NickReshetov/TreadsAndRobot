using System;

namespace Traveler.Services.Exceptions
{
    public class StartingPointParseException : Exception
    {
        public StartingPointParseException(string message) : base(message)
        {
        }
    }
}
