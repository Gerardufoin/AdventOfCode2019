using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day08
{
    class Program
    {
        /// <summary>
        /// Separate all the layers from the string input based on their size.
        /// </summary>
        /// <param name="input">String input containing all the layers given to the program</param>
        /// <param name="w">Width of the layers</param>
        /// <param name="h">Height of the layers</param>
        /// <returns>An array of string containing all the layers</returns>
        static string[] GetLayers(string input, int w, int h)
        {
            return Regex.Matches(input, @"\d{" + (w * h) + "}").Cast<Match>().Select(m => m.Value).ToArray();
        }

        #region Part 1
        /// <summary>
        /// Get the layer with the less 0's and return the product of its number of 1's and 2's.
        /// </summary>
        /// <param name="layers">List of all the layers</param>
        /// <returns>The product of the number of 1's and 2's in the layer with the least 0's</returns>
        static int CheckImageCorruption(string[] layers)
        {
            int[] count = new int[] { -1, 0, 0 };
            foreach (string layer in layers)
            {
                int[] curCount = new int[] { 0, 0, 0 };
                foreach (char c in layer)
                {
                    int val = int.Parse(c.ToString());
                    if (val >= 0 && val < curCount.Length)
                    {
                        curCount[val]++;
                    }
                }
                if (count[0] < 0 || curCount[0] < count[0])
                {
                    count = curCount;
                }
            }
            return count[1] * count[2];
        }
        #endregion

        #region Part 2
        /// <summary>
        /// Display the image in the console by superposing all the layers. 0 is black, 1 is white and 2 is transparent.
        /// </summary>
        /// <param name="layers">List of all the layers</param>
        /// <param name="w">Width of the image</param>
        /// <param name="h">Height of the image</param>
        static void DisplayImage(string[] layers, int w, int h)
        {
            for (int i = 0; i < w * h; ++i)
            {
                if (i % w == 0)
                {
                    Console.WriteLine();
                }
                for (int j = 0; j < layers.Length; ++j)
                {
                    if (layers[j][i] != '2')
                    {
                        Console.Write(layers[j][i]);
                        break;
                    }
                }
            }
        }
        #endregion

        static void Main(string[] args)
        {
            string[] layers = GetLayers(Advent2019.Utils.GetInput(args), 25, 6);
            Console.WriteLine("Part 1: " + CheckImageCorruption(layers));
            Console.Write("Part 2: ");
            DisplayImage(layers, 25, 6);
        }
    }
}
