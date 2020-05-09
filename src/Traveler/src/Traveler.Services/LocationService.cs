using System;
using System.Collections.Generic;
using System.Linq;
using Traveler.Dtos;
using Traveler.Services.Exceptions;
using Traveler.Services.Interfaces;

namespace Traveler.Services
{
    public class LocationService : ILocationService
    {
        private const int MovingStep = 1;
        
        public PositionDto CalculateRoutesEndPosition(RouteDto routeDto)
        {
            var routeSteps = TryParseRoute(routeDto.RouteSteps);

            if (routeSteps == null)
                return routeDto.StartingPosition;

            var endPoint = CalculateRouteEndPoint(routeDto.StartingPosition, routeSteps);

            return endPoint;
        }

        private static IEnumerable<RouteStep> TryParseRoute(string route)
        {
            if (string.IsNullOrWhiteSpace(route))
                return null;

            var unparsedRouteSteps = route.Trim().ToCharArray();

            ValidateUnsupportedCharacters(unparsedRouteSteps, typeof(RouteParseException));

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

        private PositionDto CalculateRouteEndPoint(PositionDto startingPoint, IEnumerable<RouteStep> routeSteps)
        {
            var routeCoordinates = new List<PositionDto> { startingPoint };

            foreach (var routeStep in routeSteps)
            {
                var currentPosition = routeCoordinates.LastOrDefault();

                var nextPosition = GetNextPosition(currentPosition, routeStep);

                routeCoordinates.Add(nextPosition);
            }

            var endPosition = routeCoordinates.LastOrDefault();

            return endPosition;
        }

        private PositionDto GetNextPosition(PositionDto currentPosition, RouteStep routeStep)
        {
            PositionDto nextPosition = null;

            switch (routeStep)
            {
                case RouteStep.F:
                    {
                        nextPosition = MoveForward(currentPosition);
                        break;
                    }

                case RouteStep.B:
                    {
                        nextPosition = MoveBackward(currentPosition);
                        break;
                    }

                case RouteStep.L:
                    {
                        nextPosition = TurnLeft(currentPosition);
                        break;
                    }

                case RouteStep.R:
                    {
                        nextPosition = TurnRight(currentPosition); 
                        break;
                    }
            }

            return nextPosition;
        }

        private PositionDto TurnRight(PositionDto currentPosition)
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

        private PositionDto TurnLeft(PositionDto currentPosition)
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

        private PositionDto MoveBackward(PositionDto currentPosition)
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

        private PositionDto MoveForward(PositionDto currentPosition)
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

        private static void ValidateUnsupportedCharacters(char[] inputSymbols, Type type)
        {
            var areThereAnyNonLetterCharacters = inputSymbols.Any(rs => !char.IsLetter(rs));

            if (areThereAnyNonLetterCharacters && type.Name == typeof(RouteParseException).Name)
                throw new RouteParseException($"RouteSteps parameters weren't parsed correctly!");
        }
    }
}
