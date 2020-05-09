using System.Collections.Generic;
using System.Linq;
using Traveler.Dtos;
using Traveler.Services.Interfaces;

namespace Traveler.Services
{
    public class RobotService : IRobotService
    {
        private readonly ILocationService _locationService;
        private readonly IRoutesService _routesService;

        public RobotService(ILocationService locationService, IRoutesService routesService)
        {
            _locationService = locationService;

            _routesService = routesService;
        }

        public (int x, int y, char direction)[] GetEndCoordinates(string rawRobotsCommands)
        {
            var routeDtos = _routesService.GetRoutesFromCommands(rawRobotsCommands);

            var endPositionDtos = routeDtos.Select(GetEndPosition);

            var endCoordinates = GetCoordinatesFromPositions(endPositionDtos);

            return endCoordinates;
        }

        private PositionDto GetEndPosition(RouteDto routeDto)
        {
            var endPosition = _locationService.CalculateRoutesEndPosition(routeDto);

            return endPosition;
        }

        private static (int X, int Y, char)[] GetCoordinatesFromPositions(IEnumerable<PositionDto> endPositions)
        {
            return endPositions
                .Select(p => (p.X, p.Y, ConvertDirectionToChar(p.Direction)))
                .ToArray();
        }

        private static char ConvertDirectionToChar(Direction direction)
        {
            return direction.ToString()
                .ToCharArray()
                .First();
        }
    }
}