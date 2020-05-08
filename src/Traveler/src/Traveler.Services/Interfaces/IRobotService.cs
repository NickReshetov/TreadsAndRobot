namespace Traveler.Services.Interfaces
{
    public interface IRobotService
    {
        (int x, int y, char direction)[] GetEndCoordinates(string routeDtos);
    }
}