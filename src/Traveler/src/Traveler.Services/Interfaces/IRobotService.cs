using System.Collections.Generic;
using System.Threading.Tasks;

namespace Traveler.Services.Interfaces
{
    public interface IRobotService
    {
        Task<IEnumerable<(int X, int Y, char)>> GetEndCoordinatesAsync(string routeDtos);
    }
}