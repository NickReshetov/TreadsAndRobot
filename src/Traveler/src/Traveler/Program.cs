using System;
using System.IO;

namespace Traveler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Traveler app has been started!");

                var robotCommands = File.ReadAllText("input.txt");
                
                var endCoordinates = TravelParser.Run(robotCommands);

                foreach (var coordinate in endCoordinates)
                {
                    Console.WriteLine($"X={coordinate.x} Y={coordinate.y} D={coordinate.direction}");
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("--------------------------------------------------------------------------");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
