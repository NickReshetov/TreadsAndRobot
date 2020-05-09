using Traveler.Dtos;

namespace Traveler.Services.Interfaces
{
    public interface ILocationService
    {
        PositionDto CalculateRoutesEndPosition(RouteDto route);
    }
}