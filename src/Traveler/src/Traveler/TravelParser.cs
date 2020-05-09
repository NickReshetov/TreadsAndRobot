using System.Collections.Generic;
using System.Linq;
using Traveler.Services;
using Traveler.Services.Interfaces;

namespace Traveler
{
    public static class TravelParser
    {   //
        //We should use dependency injection instead, but for this task I choose to instantiate services
        //
        private static readonly IRobotService _robotService = new RobotService(new LocationService(), new RoutesService());

        public static (int x, int y, char direction)[] Run(string robotsCommands)
        {
            var endCoordinates = _robotService.GetEndCoordinates(robotsCommands);

            return endCoordinates;
        }
    }
}
