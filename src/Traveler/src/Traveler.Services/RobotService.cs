using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<(int X, int Y, char)>> GetEndCoordinatesAsync(string rawRobotsCommands)
        {
            var routeDtos = await _routesService.GetRoutesFromCommandsAsync(rawRobotsCommands);

            var endPositionDtos = routeDtos
                .Select(async r => await GetEndPositionAsync(r))
                .Select(t => t.GetAwaiter().GetResult())
                .ToList();

            var endCoordinates = await GetCoordinatesFromPositionsAsync(endPositionDtos);

            return endCoordinates;
        }

        private async Task<PositionDto> GetEndPositionAsync(RouteDto routeDto)
        {
            var endPosition = await _locationService.CalculateRoutesEndPositionAsync(routeDto);

            return endPosition;
        }

        private static async Task<IEnumerable<(int X, int Y, char)>> GetCoordinatesFromPositionsAsync(IEnumerable<PositionDto> endPositions)
        {
            return endPositions
                .Select(async p => (p.X, p.Y, await ConvertDirectionToCharAsync(p.Direction)))
                .Select(t => t.GetAwaiter().GetResult())
                .ToArray();
        }

        private static async Task<char> ConvertDirectionToCharAsync(Direction direction)
        {
            return direction.ToString()
                .ToCharArray()
                .First();
        }
    }
}