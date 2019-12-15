using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Day15
{
    class Program
    {
        /// <summary>
        /// Get the next position based on the direction.
        /// </summary>
        /// <param name="position">Current position</param>
        /// <param name="direction">Direction</param>
        /// <returns>The new direction after moving in the given direction</returns>
        static (int, int) GetNextPosition((int x, int y) position, int direction)
        {
            switch (direction)
            {
                case 1:
                    position.y += 1;
                    break;
                case 2:
                    position.y -= 1;
                    break;
                case 3:
                    position.x -= 1;
                    break;
                case 4:
                    position.x += 1;
                    break;
            }
            return position;
        }

        #region Part 1
        /// <summary>
        /// Recursive. Create the map discovered by the intcode computer.
        /// </summary>
        /// <param name="computer">Intcode computer</param>
        /// <param name="position">Current position</param>
        /// <param name="map">The map to populate</param>
        static void PopulateMap(IntcodeComputer computer, (int x, int y) position, Dictionary<(int x, int y), int> map)
        {
            if (!map.ContainsKey(position))
            {
                map.Add(position, 1);
            }
            for (int i = 1; i <= 4; ++i)
            {
                (int x, int y) next = GetNextPosition(position, i);
                if (!map.ContainsKey(next))
                {
                    List<BigInteger> res = computer.Execute(i);
                    if (res.Count != 1)
                    {
                        Advent2019.Utils.FatalError("Expected exactly one output from intcode computer, got " + res);
                    }
                    map.Add(next, (int)res[0]);
                    if (res[0] > 0)
                    {
                        PopulateMap(computer, next, map);
                        computer.Execute(i % 2 == 0 ? i - 1 : i + 1);
                    }
                }
            }
        }

        /// <summary>
        /// Display the map in the console. Not needed to get the result.
        /// </summary>
        /// <param name="map">Map to display</param>
        static void DisplayMap(Dictionary<(int x, int y), int> map)
        {
            (int min, int max) xBounds = (map.Min(m => m.Key.x), map.Max(m => m.Key.x));
            (int min, int max) yBounds = (map.Min(m => m.Key.y), map.Max(m => m.Key.y));
            for (int y = yBounds.min; y <= yBounds.max; ++y)
            {
                for (int x = xBounds.min; x <= xBounds.max; ++x)
                {
                    Console.Write(map.ContainsKey((x, y)) && map[(x, y)] > 0 ? (map[(x, y)] == 2 ? "X" : " ") : "#");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Recursive. Find the shortest path to the oxygen generator from (0, 0).
        /// As the given map is a maze without loop, we don't need to remember every visited case.
        /// </summary>
        /// <param name="map">Map discovered by the robot</param>
        /// <param name="position">Current position</param>
        /// <param name="length">Current length of the path</param>
        /// <param name="pDir">Direction of the last movement</param>
        /// <returns>The shortest length if the oxagen generator has been found or 0 otherwise</returns>
        static int GetShortestPath(Dictionary<(int x, int y), int> map, (int x, int y) position, int length = 0, int pDir = 0)
        {
            pDir = (pDir % 2 == 0 ? pDir - 1 : pDir + 1);
            int l = 0;
            for (int i = 1; i <= 4; ++i)
            {
                if (pDir != i)
                {
                    (int x, int y) next = GetNextPosition(position, i);
                    if (map.ContainsKey(next) && map[next] > 0)
                    {
                        if (map[next] == 2)
                        {
                            return length + 1;
                        }
                        int res = GetShortestPath(map, next, length + 1, i);
                        if (res > 0 && (l == 0 || res < l))
                        {
                            l = res;
                        }
                    }
                }
            }
            return l;
        }
        #endregion

        #region Part 2
        /// <summary>
        /// Recursive. Find the longest path possible from position.
        /// As the map is a maze without loop, this part is juste the opposite of part 1 starting from the oxygen generator. The longest path possible is the time needed to fill everything.
        /// </summary>
        /// <param name="map">The populated map from Part 1</param>
        /// <param name="position">Current position</param>
        /// <param name="length">Length of the current path</param>
        /// <param name="pDir">Direction of the previous move</param>
        /// <returns>The longest path found</returns>
        static int GetLongestPath(Dictionary<(int x, int y), int> map, (int x, int y) position, int length = 0, int pDir = 0)
        {
            pDir = (pDir % 2 == 0 ? pDir - 1 : pDir + 1);
            int l = length;
            for (int i = 1; i <= 4; ++i)
            {
                if (pDir != i)
                {
                    (int x, int y) next = GetNextPosition(position, i);
                    if (map.ContainsKey(next) && map[next] > 0)
                    {
                        int res = GetLongestPath(map, next, length + 1, i);
                        if (res > l)
                        {
                            l = res;
                        }
                    }
                }
            }
            return l;
        }
        #endregion

        static void Main(string[] args)
        {
            IntcodeComputer computer = new IntcodeComputer(Advent2019.Utils.GetInput(args));
            Dictionary<(int x, int y), int> map = new Dictionary<(int x, int y), int>();
            PopulateMap(computer, (0, 0), map);
            DisplayMap(map);
            Console.WriteLine("Part 1: " + GetShortestPath(map, (0, 0)));
            Console.WriteLine("Part 1: " + GetLongestPath(map, map.Where(m => m.Value == 2).First().Key));
        }
    }
}
