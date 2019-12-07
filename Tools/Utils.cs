﻿using System;
using System.IO;

namespace Advent2019
{
    /// <summary>
    /// The Utils class contains static methods needed through all the exercices.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Get the content of a file and handle errors.
        /// </summary>
        /// <param name="args">Arguments of the program. The path to the file should be the only argument.</param>
        /// <returns>The content of the file given as parameter.</returns>
        public static string GetInput(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Use: .\\" + System.AppDomain.CurrentDomain.FriendlyName + " path");
                System.Environment.Exit(0);
            }
            try
            {
                return File.ReadAllText(args[0]);
            }
            catch
            {
                Console.WriteLine("Error: Failed to read the input file '" + args[0] + "'.");
                System.Environment.Exit(0);
            }
            return "";
        }
    }
}
