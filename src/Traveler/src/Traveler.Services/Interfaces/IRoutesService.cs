using System.Collections.Generic;
using System.Threading.Tasks;
using Traveler.Dtos;

namespace Traveler.Services.Interfaces
{
    public interface IRoutesService
    {
        Task<IEnumerable<RouteDto>> GetRoutesFromCommandsAsync(string rawRobotsCommands);
    }
}