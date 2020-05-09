using System;

namespace Traveler.Services.Exceptions
{
    public class RouteParseException : Exception
    {
        public RouteParseException(string message) : base(message)
        {
        }
    }
}
