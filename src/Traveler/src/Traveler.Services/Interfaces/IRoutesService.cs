using System.Collections.Generic;
using Traveler.Dtos;

namespace Traveler.Services.Interfaces
{
    public interface IRoutesService
    {
        IEnumerable<RouteDto> GetRoutesFromCommands(string rawRobotsCommands);
    }
}