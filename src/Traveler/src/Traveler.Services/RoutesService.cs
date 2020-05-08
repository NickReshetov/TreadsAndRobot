using System;
using System.Collections.Generic;
using System.Linq;
using Traveler.Dtos;
using Traveler.Services.Exceptions;
using Traveler.Services.Interfaces;

namespace Traveler.Services
{
    public class RoutesService : IRoutesService
    {
        private const string CommandsSeparator = "POS=";
        private const string CommentMarker = "//";
        private const string NewLineSeparator = "\r\n";
        private const string ParsingParameterX = "X";
        private const string ParsingParameterY = "Y";
        private const string ParsingParameterDirection = "Direction";
        private const int StartingPointParametersCount = 3;
        private const char StartingPointParametersSeparator = ',';

        public IEnumerable<RouteDto> GetRoutesFromCommands(string rawRobotsCommands)
        {
            var routeDtos = new List<RouteDto>();

            var robotCommandsWithoutComments = GetRobotCommandsWithoutComments(rawRobotsCommands);

            var startingPositionsAndRouteSteps = GetStartingPositionsAndRouteSteps(robotCommandsWithoutComments);

            foreach (var startingPositionAndRouteStep in startingPositionsAndRouteSteps)
            {
                var startingPosition = GetStartingPosition(startingPositionAndRouteStep);

                var routeSteps = GetRouteSteps(startingPositionAndRouteStep);

                var routeDto = new RouteDto
                {
                    StartingPosition = startingPosition,
                    RouteSteps = routeSteps
                };

                routeDtos.Add(routeDto);
            }

            return routeDtos;
        }

        private static string GetRobotCommandsWithoutComments(string rawRobotsCommands)
        {
            var robotsCommandsWithoutComments = rawRobotsCommands
                .Split(NewLineSeparator)
                .Where(line => line != string.Empty && !line.StartsWith(CommentMarker));

            var robotsCommands = string.Join(NewLineSeparator, robotsCommandsWithoutComments);

            return robotsCommands;
        }

        private static string[] GetStartingPositionsAndRouteSteps(string robotCommand)
        {
            return robotCommand
                .Split(CommandsSeparator)
                .Where(command => command != string.Empty)
                .ToArray();
        }

        private PositionDto GetStartingPosition(string startingPositionsAndRouteStep)
        {
            var startingPosition = startingPositionsAndRouteStep
                .Split(NewLineSeparator)
                .First(line => line != string.Empty);
                
            var startingPositionDto = CreateRouteStartingPoint(startingPosition);

            return startingPositionDto;
        }

        private static string GetRouteSteps(string startingPositionsAndRouteStep)
        {
            //We are skipping starting position and selecting routeSteps only,
            //even if they are in different lines
            var routeStepsFromDifferentLines = startingPositionsAndRouteStep
                .Split(NewLineSeparator)
                .Where(line => line != string.Empty)
                .Skip(1)
                .ToArray();

            //Joining route steps from different lines
            var routeSteps = string.Join(string.Empty, routeStepsFromDifferentLines);

            return routeSteps;
        }

        private PositionDto CreateRouteStartingPoint(string unparsedStartingPointParameters)
        {
            var startingPointParameters = TryParseStartingPointParameters(unparsedStartingPointParameters);

            var startingPoint = CreateRouteStartingPoint(startingPointParameters.x, startingPointParameters.y, startingPointParameters.direction);

            return startingPoint;
        }

        private (int x, int y, Direction direction) TryParseStartingPointParameters(string unparsedStartingPointParameters)
        {
            if (string.IsNullOrWhiteSpace(unparsedStartingPointParameters))
                throw new StartingPointParseException($"StartingPoint wasn't parsed, because it was not specified!");

            var startingPointCoordinatesAndDirection = unparsedStartingPointParameters
                .Trim()
                .Split(StartingPointParametersSeparator);

            if (startingPointCoordinatesAndDirection.Length != StartingPointParametersCount)
                throw new StartingPointParseException($"StartingPoint wasn't parsed, because it was not specified properly!");

            var parsingX = startingPointCoordinatesAndDirection[0];

            var parsingY = startingPointCoordinatesAndDirection[1];

            var parsingDirection = startingPointCoordinatesAndDirection[2];

            var x = ParseStartingPointParameter(parsingX, ParsingParameterX);

            var y = ParseStartingPointParameter(parsingY, ParsingParameterY);

            var direction = (Direction)ParseStartingPointParameter(parsingDirection, ParsingParameterDirection);

            return (x, y, direction);
        }

        private static int ParseStartingPointParameter(string unparsedStartingPointParameter, string parameterType)
        {
            object direction = null;
            var startingPointParameter = 0;

            if (parameterType == ParsingParameterDirection)
                ValidateUnsupportedCharacters(unparsedStartingPointParameter.ToCharArray(), typeof(StartingPointParseException));

            var isParsed = parameterType == ParsingParameterDirection ?
                Enum.TryParse(typeof(Direction), unparsedStartingPointParameter, true, out direction) :
                int.TryParse(unparsedStartingPointParameter, out startingPointParameter);

            if (!isParsed)
                throw new StartingPointParseException($"{parameterType} wasn't parsed correctly!");

            if (parameterType == ParsingParameterDirection && direction != null)
                return (int)direction;

            return startingPointParameter;
        }

        private PositionDto CreateRouteStartingPoint(int x, int y, Direction direction)
        {
            ValidateStartingPointParameters(x, y, direction);

            var startingPoint = new PositionDto
            {
                X = x,
                Y = y,
                Direction = direction
            };

            return startingPoint;
        }

        private void ValidateStartingPointParameters(int x, int y, Direction direction)
        {
            if (x < 0)
                throw new StartingPointOutOfRangeException("Starting point X coordinate should be greater or equal zero");

            if (y < 0)
                throw new StartingPointOutOfRangeException("Starting point Y coordinate should be greater or equal zero");
        }

        private static void ValidateUnsupportedCharacters(char[] inputSymbols, Type type)
        {
            var areThereAnyNonLetterCharacters = inputSymbols.Any(rs => !char.IsLetter(rs));

            if (areThereAnyNonLetterCharacters && type.Name == typeof(StartingPointParseException).Name)
                throw new StartingPointParseException($"Starting point parameters weren't parsed correctly!");
        }
    }
}
