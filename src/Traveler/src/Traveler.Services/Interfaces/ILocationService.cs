using System.Threading.Tasks;
using Traveler.Dtos;

namespace Traveler.Services.Interfaces
{
    public interface ILocationService
    {
        Task<PositionDto> CalculateRoutesEndPositionAsync(RouteDto route);
    }
}