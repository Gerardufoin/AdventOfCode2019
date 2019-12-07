using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Day01
{
    class Program
    {
        #region Part 1
        /// <summary>
        /// Transform the input text into an array of integer.
        /// </summary>
        /// <param name="input">Text input of the exercice. Here a list of int separated by newlines.</param>
        /// <returns>An array of ints containing all the modules' mass.</returns>
        static int[] GetModulesMass(string input)
        {
            return Regex.Matches(input, @"(\d+)").Cast<Match>().Select(match => int.Parse(match.Value)).ToArray();
        }

        /// <summary>
        /// Tranform the moduleMass into fuel as per the exercice's instructions.
        /// </summary>
        /// <param name="moduleMass">Mass of one module.</param>
        /// <returns>The fuel as an integer.</returns>
        static int GetFuel(int moduleMass)
        {
            return moduleMass / 3 - 2;
        }

        static int GetFuelPart1(int[] modulesMass)
        {
            return modulesMass.Sum(mass => GetFuel(mass));
        }
        #endregion

        #region Part 2
        /// <summary>
        /// Calculate a module's fuel, then the fuel's fuel, and so on until the fuel is equal or less than 0.
        /// </summary>
        /// <param name="mass">Mass of one module.</param>
        /// <returns>The total fuel needed for this one module.</returns>
        static int GetFuelFromFuelFromModuleMass(int mass)
        {
            int fuel = GetFuel(mass);
            int totalFuel = 0;
            while (fuel > 0)
            {
                totalFuel += fuel;
                fuel = GetFuel(fuel);
            }
            return totalFuel;
        }

        static int GetFuelPart2(int[] modulesMass)
        {
            return modulesMass.Sum(mass => GetFuelFromFuelFromModuleMass(mass));
        }
        #endregion

        static void Main(string[] args)
        {
            int[] modulesMass = GetModulesMass(Advent2019.Utils.GetInput(args));
            Console.WriteLine("Part 1: " + GetFuelPart1(modulesMass));
            Console.WriteLine("Part 2: " + GetFuelPart2(modulesMass));
        }
    }
}
