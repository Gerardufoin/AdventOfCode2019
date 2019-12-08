using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day06
{
    class Program
    {
        #region Part 1
        /// <summary>
        /// Get the input containing the orbits and split them into an array of array of string, each element containing the two orbits.
        /// </summary>
        /// <param name="input">The input passed to the program</param>
        /// <returns>An array[] of array[2] of string</returns>
        static string[][] GetOrbits(string input)
        {
            return Regex.Matches(input, @"((\w+)\)(\w+))").Cast<Match>().Select(m => new string[2] { m.Groups[2].Value, m.Groups[3].Value }).ToArray();
        }

        /// <summary>
        /// Take the orbits and transform them into a map. Each key of the map is an object and the value is the object it is orbiting around. If it is orbiting around nothing, the string is empty.
        /// </summary>
        /// <param name="orbits">The array of array of string containing the orbits</param>
        /// <returns>The orbits map as a Dictionnary</returns>
        static Dictionary<string, string> CreateOrbitMap(string[][] orbits)
        {
            Dictionary<string, string> omap = new Dictionary<string, string>();
            foreach (string[] o in orbits)
            {
                if (!omap.ContainsKey(o[0]))
                {
                    omap.Add(o[0], "");
                }
                if (!omap.ContainsKey(o[1]))
                {
                    omap.Add(o[1], "");
                }
                if (!omap[o[1]].Contains(o[0]))
                {
                    if (omap[o[1]] != "")
                    {
                        Advent2019.Utils.FatalError("An object can only orbit around one other object, but '" + o[1] + "' orbits around '" + omap[o[1]] + "' and '" + o[0] + "'.");
                    }
                    omap[o[1]] = o[0];
                }
            }
            return omap;
        }

        /// <summary>
        /// Count the orbits and sub-orbits of all the object in the orbits map.
        /// </summary>
        /// <param name="omap">The orbits map</param>
        /// <returns>The total number of orbits and sub-orbits</returns>
        static int CountOrbits(Dictionary<string, string> omap)
        {
            int count = 0;
            foreach (KeyValuePair<string, string> obj in omap)
            {
                string cur = obj.Key;
                while (omap[cur] != "")
                {
                    count++;
                    cur = omap[cur];
                }
            }
            return count;
        }
        #endregion

        #region Part 2
        /// <summary>
        /// Create a list containing all the object from a specified object to the center of the map (object orbiting around nothing).
        /// </summary>
        /// <param name="objName">The starting object</param>
        /// <param name="omap">The orbits map</param>
        /// <returns>A list containing all the objects from objName to the center</returns>
        static List<string> PathToCenter(string objName, Dictionary<string, string> omap)
        {
            List<string> path = new List<string>();
            string curr = objName;
            if (!omap.ContainsKey(objName))
            {
                Advent2019.Utils.FatalError("Orbit map need to contain a '" + objName + "' object.");
            }
            while (omap[curr] != "")
            {
                curr = omap[curr];
                path.Add(curr);
            }
            return path;
        }

        /// <summary>
        /// Find the shortest path from YOU to SAN. To do that, we take the complete path of both YOU and SAN to the center and look which object they both cross, which are the connexions.
        /// We then took the connexion with the shortest amount of orbits adding YOU->CON + CON->SAN.
        /// </summary>
        /// <param name="omap">The orbits map</param>
        /// <returns>The number of orbits on the shortest path from YOU to SAN</returns>
        static int ShortestPath(Dictionary<string, string> omap)
        {
            List<string> youToCenter = PathToCenter("YOU", omap);
            List<string> sanToCenter = PathToCenter("SAN", omap);
            int shortest = -1;
            for (int i = 0; i < youToCenter.Count; ++i)
            {
                int sanDist = sanToCenter.IndexOf(youToCenter[i]);
                if (sanDist >= 0 && (shortest < 0 || sanDist + i < shortest))
                {
                    shortest = sanDist + i;
                }
            }
            return shortest;
        }
        #endregion

        static void Main(string[] args)
        {
            Dictionary<string, string> omap = CreateOrbitMap(GetOrbits(Advent2019.Utils.GetInput(args)));
            Console.WriteLine("Part 1: " + CountOrbits(omap));
            Console.WriteLine("Part 2: " + ShortestPath(omap));
        }
    }
}
