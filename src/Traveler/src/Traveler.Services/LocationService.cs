using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Traveler.Dtos;
using Traveler.Services.Exceptions;
using Traveler.Services.Interfaces;

namespace Traveler.Services
{
    public class LocationService : ILocationService
    {
        private const int MovingStep = 1;
        
        public async Task<PositionDto> CalculateRoutesEndPositionAsync(RouteDto routeDto)
        {
            var routeSteps = await ParseRouteAsync(routeDto.RouteSteps);

            if (routeSteps == null)
                return routeDto.StartingPosition;

            var endPoint = await CalculateRouteEndPointAsync(routeDto.StartingPosition, routeSteps);

            return endPoint;
        }

        private static async Task<IEnumerable<RouteStep>> ParseRouteAsync(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return null;

            var unparsedRouteSteps = route.Trim().ToCharArray();

            await ValidateUnsupportedCharactersAsync(unparsedRouteSteps, typeof(RouteParseException));

            var routeSteps = new List<RouteStep>();

            foreach (var unparsedRouteStep in unparsedRouteSteps)
            {
                var routeStepToParse = unparsedRouteStep.ToString();

                var isParsed = Enum.TryParse(typeof(RouteStep), routeStepToParse, true, out var routeStep);

                if (!isParsed)
                    throw new RouteParseException($"RouteStep {routeStepToParse} wasn't parsed correctly!");

                routeSteps.Add((RouteStep)routeStep);
            }

            return routeSteps;
        }

        private async Task<PositionDto> CalculateRouteEndPointAsync(PositionDto startingPoint, IEnumerable<RouteStep> routeSteps)
        {
            var routeCoordinates = new List<PositionDto> { startingPoint };

            foreach (var routeStep in routeSteps)
            {
                var currentPosition = routeCoordinates.LastOrDefault();

                var nextPosition = await GetNextPositionAsync(currentPosition, routeStep);

                routeCoordinates.Add(nextPosition);
            }

            var endPosition = routeCoordinates.LastOrDefault();

            return endPosition;
        }

        private async Task<PositionDto> GetNextPositionAsync(PositionDto currentPosition, RouteStep routeStep)
        {
            PositionDto nextPosition = null;

            switch (routeStep)
            {
                case RouteStep.F:
                    {
                        nextPosition = await MoveForwardAsync(currentPosition);
                        break;
                    }

                case RouteStep.B:
                    {
                        nextPosition = await MoveBackwardAsync(currentPosition);
                        break;
                    }

                case RouteStep.L:
                    {
                        nextPosition = await TurnLeftAsync(currentPosition);
                        break;
                    }

                case RouteStep.R:
                    {
                        nextPosition = await TurnRightAsync(currentPosition); 
                        break;
                    }
            }

            return nextPosition;
        }

        private async Task<PositionDto> TurnRightAsync(PositionDto currentPosition)
        {
            var nextPosition = new PositionDto();

            switch (currentPosition.Direction)
            {
                case Direction.N:
                {
                    nextPosition.X = currentPosition.X;
                    nextPosition.Y = currentPosition.Y;
                    nextPosition.Direction = Direction.E;
                    break;
                }

                case Direction.S:
                {
                    nextPosition.X = currentPosition.X;
                    nextPosition.Y = currentPosition.Y;
                    nextPosition.Direction = Direction.W;
                    break;
                }

                case Direction.W:
                {
                    nextPosition.X = currentPosition.X;
                    nextPosition.Y = currentPosition.Y;
                    nextPosition.Direction = Direction.N;
                    break;
                }

                case Direction.E:
                {
                    nextPosition.X = currentPosition.X;
                    nextPosition.Y = currentPosition.Y;
                    nextPosition.Direction = Direction.S;
                    break;
                }
            }

            return nextPosition;
        }

        private async Task<PositionDto> TurnLeftAsync(PositionDto currentPosition)
        {
            var nextPosition = new PositionDto();

            switch (currentPosition.Direction)
            {
                case Direction.N:
                {
                    nextPosition.X = currentPosition.X;
                    nextPosition.Y = currentPosition.Y;
                    nextPosition.Direction = Direction.W;
                    break;
                }

                case Direction.S:
                {
                    nextPosition.X = currentPosition.X;
                    nextPosition.Y = currentPosition.Y;
                    nextPosition.Direction = Direction.E;
                    break;
                }

                case Direction.W:
                {
                    nextPosition.X = currentPosition.X;
                    nextPosition.Y = currentPosition.Y;
                    nextPosition.Direction = Direction.S;
                    break;
                }

                case Direction.E:
                {
                    nextPosition.X = currentPosition.X;
                    nextPosition.Y = currentPosition.Y;
                    nextPosition.Direction = Direction.N;
                    break;
                }
            }

            return nextPosition;
        }

        private async Task<PositionDto> MoveBackwardAsync(PositionDto currentPosition)
        {
            var nextPosition = new PositionDto();

            switch (currentPosition.Direction)
            {
                case Direction.N:
                    {
                        nextPosition.X = currentPosition.X;
                        nextPosition.Y = currentPosition.Y + MovingStep;
                        nextPosition.Direction = Direction.N;
                        break;
                    }

                case Direction.S:
                    {
                        nextPosition.X = currentPosition.X;
                        nextPosition.Y = currentPosition.Y - MovingStep;
                        nextPosition.Direction = Direction.S;
                        break;
                    }

                case Direction.W:
                    {
                        nextPosition.X = currentPosition.X + MovingStep;
                        nextPosition.Y = currentPosition.Y;
                        nextPosition.Direction = Direction.W;
                        break;
                    }

                case Direction.E:
                    {
                        nextPosition.X = currentPosition.X - MovingStep;
                        nextPosition.Y = currentPosition.Y;
                        nextPosition.Direction = Direction.E;
                        break;
                    }
            }

            return nextPosition;
        }

        private async Task<PositionDto> MoveForwardAsync(PositionDto currentPosition)
        {
            var nextPosition = new PositionDto();

            switch (currentPosition.Direction)
            {
                case Direction.N:
                    {
                        nextPosition.X = currentPosition.X;
                        nextPosition.Y = currentPosition.Y - MovingStep;
                        nextPosition.Direction = Direction.N;
                        break;
                    }

                case Direction.S:
                    {
                        nextPosition.X = currentPosition.X;
                        nextPosition.Y = currentPosition.Y + MovingStep;
                        nextPosition.Direction = Direction.S;
                        break;
                    }

                case Direction.W:
                    {
                        nextPosition.X = currentPosition.X - MovingStep;
                        nextPosition.Y = currentPosition.Y;
                        nextPosition.Direction = Direction.W;
                        break;
                    }

                case Direction.E:
                    {
                        nextPosition.X = currentPosition.X + MovingStep;
                        nextPosition.Y = currentPosition.Y;
                        nextPosition.Direction = Direction.E;
                        break;
                    }
            }

            return nextPosition;
        }

        private static async Task ValidateUnsupportedCharactersAsync(char[] inputSymbols, Type type)
        {
            var areThereAnyNonLetterCharacters = inputSymbols.Any(rs => !char.IsLetter(rs));

            if (areThereAnyNonLetterCharacters && type.Name == typeof(RouteParseException).Name)
                throw new RouteParseException($"RouteSteps parameters weren't parsed correctly!");
        }
    }
}
