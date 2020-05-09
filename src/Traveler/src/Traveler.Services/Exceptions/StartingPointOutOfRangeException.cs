using System;

namespace Traveler.Services.Exceptions
{
    public class StartingPointOutOfRangeException : Exception
    {
        public StartingPointOutOfRangeException(string message) : base(message)
        {
        }
    }
}
