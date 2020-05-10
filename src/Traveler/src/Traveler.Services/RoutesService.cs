using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<RouteDto>> GetRoutesFromCommandsAsync(string rawRobotsCommands)
        {
            var routeDtos = new List<RouteDto>();

            var robotCommandsWithoutComments = await GetRobotCommandsWithoutCommentsAsync(rawRobotsCommands);

            var startingPositionsAndRouteSteps = await GetStartingPositionsAndRouteStepsAsync(robotCommandsWithoutComments);

            foreach (var startingPositionAndRouteStep in startingPositionsAndRouteSteps)
            {
                var startingPosition = await GetStartingPositionAsync(startingPositionAndRouteStep);

                var routeSteps = await GetRouteStepsAsync(startingPositionAndRouteStep);

                var routeDto = new RouteDto
                {
                    StartingPosition = startingPosition,
                    RouteSteps = routeSteps
                };

                routeDtos.Add(routeDto);
            }

            return routeDtos;
        }

        private static async Task<string> GetRobotCommandsWithoutCommentsAsync(string rawRobotsCommands)
        {
            var robotsCommandsWithoutComments = rawRobotsCommands
                .Split(NewLineSeparator)
                .Where(line => line != string.Empty && !line.StartsWith(CommentMarker));

            var robotsCommands = string.Join(NewLineSeparator, robotsCommandsWithoutComments);

            return robotsCommands;
        }

        private static async Task<string[]> GetStartingPositionsAndRouteStepsAsync(string robotCommand)
        {
            return robotCommand
                .Split(CommandsSeparator)
                .Where(command => command != string.Empty)
                .ToArray();
        }

        private async Task<PositionDto> GetStartingPositionAsync(string startingPositionsAndRouteStep)
        {
            var startingPosition = startingPositionsAndRouteStep
                .Split(NewLineSeparator)
                .First(line => line != string.Empty);
                
            var startingPositionDto = await CreateRouteStartingPointAsync(startingPosition);

            return startingPositionDto;
        }

        private static async Task<string> GetRouteStepsAsync(string startingPositionsAndRouteStep)
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

        private async Task<PositionDto> CreateRouteStartingPointAsync(string unparsedStartingPointParameters)
        {
            var startingPointParameters = await TryParseStartingPointParametersAsync(unparsedStartingPointParameters);

            var startingPoint = await CreateRouteStartingPointAsync(startingPointParameters.x, startingPointParameters.y, startingPointParameters.direction);

            return startingPoint;
        }

        private async Task<(int x, int y, Direction direction)> TryParseStartingPointParametersAsync(string unparsedStartingPointParameters)
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

            var x = await ParseStartingPointParameterAsync(parsingX, ParsingParameterX);

            var y = await ParseStartingPointParameterAsync(parsingY, ParsingParameterY);

            var direction = await ParseStartingPointParameterAsync(parsingDirection, ParsingParameterDirection);

            return (x, y, (Direction)direction);
        }

        private static async Task<int> ParseStartingPointParameterAsync(string unparsedStartingPointParameter, string parameterType)
        {
            object direction = null;
            var startingPointParameter = 0;

            if (parameterType == ParsingParameterDirection)
                await ValidateUnsupportedCharactersAsync(unparsedStartingPointParameter.ToCharArray(), typeof(StartingPointParseException));

            var isParsed = parameterType == ParsingParameterDirection ?
                Enum.TryParse(typeof(Direction), unparsedStartingPointParameter, true, out direction) :
                int.TryParse(unparsedStartingPointParameter, out startingPointParameter);

            if (!isParsed)
                throw new StartingPointParseException($"{parameterType} wasn't parsed correctly!");

            if (parameterType == ParsingParameterDirection && direction != null)
                return (int)direction;

            return startingPointParameter;
        }

        private async Task<PositionDto> CreateRouteStartingPointAsync(int x, int y, Direction direction)
        {
           await ValidateStartingPointParametersAsync(x, y, direction);

            var startingPoint = new PositionDto
            {
                X = x,
                Y = y,
                Direction = direction
            };

            return startingPoint;
        }

        private async Task ValidateStartingPointParametersAsync(int x, int y, Direction direction)
        {
            if (x < 0)
                throw new StartingPointOutOfRangeException("Starting point X coordinate should be greater or equal zero");

            if (y < 0)
                throw new StartingPointOutOfRangeException("Starting point Y coordinate should be greater or equal zero");
        }

        private static async Task ValidateUnsupportedCharactersAsync(char[] inputSymbols, Type type)
        {
            var areThereAnyNonLetterCharacters = inputSymbols.Any(rs => !char.IsLetter(rs));

            if (areThereAnyNonLetterCharacters && type.Name == typeof(StartingPointParseException).Name)
                throw new StartingPointParseException($"Starting point parameters weren't parsed correctly!");
        }
    }
}
