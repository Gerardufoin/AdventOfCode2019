using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Day10
{
    class Program
    {
        /// <summary>
        /// Asteroid class stores all the information needed for an asteroid.
        /// </summary>
        class Asteroid
        {
            public int X;
            public int Y;
            public decimal uX;
            public decimal uY;
            public decimal uAngle;
            public double Length;
            public int Amount;
            public Asteroid Reference = null;
            public SortedDictionary<decimal, List<Asteroid>> LineOfSight = null;

            public Asteroid(int x, int y)
            {
                X = x;
                Y = y;
                ComputeValues();
                Amount = 0;
            }

            /// <summary>
            /// Set a few value needed to check the line of sight, namely the unit vector and its angle.
            /// </summary>
            private void ComputeValues()
            {
                if (X != 0 || Y != 0)
                {
                    Length = Math.Sqrt(X * X + Y * Y);
                    uX = (decimal)(X / Length);
                    uY = (decimal)(Y / Length);
                }
                uAngle = (decimal)(180 - (180 / Math.PI) * Math.Atan2((double)uX, (double)uY));
            }
        }

        /// <summary>
        /// Transform the input value into an array of strings, each line being a row of asteroid
        /// </summary>
        /// <param name="input">The input passed to the program</param>
        /// <returns>The map of asteroid in an array of strings</returns>
        static string[] GetMap(string input)
        {
            return Regex.Matches(input, @"([#\.]+)").Cast<Match>().Select(m => m.Value).ToArray();
        }

        /// <summary>
        /// Convert the array of strings into a list of Asteroids.
        /// </summary>
        /// <param name="map">The array of string containing the asteroids positions</param>
        /// <returns>A list of all the asteroids present on the map</returns>
        static List<Asteroid> GetAsteroids(string[] map)
        {
            if (map.Length == 0)
            {
                Advent2019.Utils.FatalError("Map is invalid.");
            }
            int size = map[0].Length;
            List<Asteroid> asteroids = new List<Asteroid>();
            for (int y = 0; y < map.Length; ++y)
            {
                if (map[y].Length != size)
                {
                    Advent2019.Utils.FatalError("All map lines are not of the same size.");
                }
                for (int x = 0; x < size; ++x)
                {
                    if (map[y][x] == '#')
                    {
                        asteroids.Add(new Asteroid(x, y));
                    }
                }
            }
            return asteroids;
        }

        /// <summary>
        /// Display the coordinates of the best asteroid and a map of the line of sight
        /// </summary>
        /// <param name="best">Asteroid to display</param>
        /// <param name="map">Default map of the program</param>
        static void Debug(Asteroid best, string[] map)
        {
            Console.WriteLine("Coordinates: (" + best.X + ";" + best.Y + ")");
            map[best.Y] = map[best.Y].Substring(0, best.X) + "X" + map[best.Y].Substring(best.X + 1);
            foreach (KeyValuePair<decimal, List<Asteroid>> entry in best.LineOfSight)
            {
                Asteroid ast = entry.Value[0].Reference;
                map[ast.Y] = map[ast.Y].Substring(0, ast.X) + "0" + map[ast.Y].Substring(ast.X + 1);
            }
            foreach (string line in map)
            {
                Console.WriteLine(line);
            }
        }

        /// <summary>
        /// Return the asteroid with the best line of sight (able to see the most asteroids)
        /// </summary>
        /// <param name="asteroidsMap">The map of the asteroids</param>
        /// <returns>The best asteroids given the conditions</returns>
        static Asteroid GetBestAsteroid(string[] asteroidsMap)
        {
            List<Asteroid> asteroids = GetAsteroids(asteroidsMap);
            SortedDictionary<decimal, List<Asteroid>> lineOfSight = new SortedDictionary<decimal, List<Asteroid>>();
            Asteroid best = null;
            foreach (Asteroid ast1 in asteroids)
            {
                lineOfSight.Clear();
                foreach (Asteroid ast2 in asteroids)
                {
                    if (!(ast1.X == ast2.X && ast1.Y == ast2.Y))
                    {
                        Asteroid relative = new Asteroid(ast2.X - ast1.X, ast2.Y - ast1.Y);
                        relative.Reference = ast2;
                        if (!lineOfSight.ContainsKey(relative.uAngle))
                        {
                            lineOfSight.Add(relative.uAngle, new List<Asteroid>() { relative });
                        }
                        else
                        {
                            int i = 0;
                            while (i < lineOfSight[relative.uAngle].Count && lineOfSight[relative.uAngle][i].Length < relative.Length)
                            {
                                ++i;
                            }
                            lineOfSight[relative.uAngle].Insert(i, relative);
                        }
                    }
                }
                ast1.Amount = lineOfSight.Count;
                if (best == null || best.Amount < ast1.Amount)
                {
                    if (best != null)
                    {
                        best.LineOfSight = null;
                    }
                    best = ast1;
                    best.LineOfSight = lineOfSight;
                    lineOfSight = new SortedDictionary<decimal, List<Asteroid>>();
                }
            }
            //Debug(best, asteroidsMap);
            return best;
        }

        /// <summary>
        /// Destroy the asteroids into a clockwise order. If there are more than 'number' asteroids, return the y coordinate added to the x coordinate multiplied by 100 for the 'number' asteroid destroyed.
        /// </summary>
        /// <param name="ast">The asteroid from which the others will be destroyed</param>
        /// <param name="number">Which destroyed asteroid the return value is calculed from</param>
        /// <returns>0 or NumberAst.x * 100 + NumberAst.y</returns>
        static int DestroyAsteroids(Asteroid ast, int number)
        {
            int counter = 0;
            int destroyed = 0;
            while (counter < ast.LineOfSight.Count)
            {
                counter = 0;
                foreach (KeyValuePair<decimal, List<Asteroid>> entry in ast.LineOfSight)
                {
                    if (entry.Value.Count > 0)
                    {
                        destroyed++;
                        if (destroyed == number)
                        {
                            return entry.Value[0].Reference.X * 100 + entry.Value[0].Reference.Y;
                        }
                        entry.Value.RemoveAt(0);
                    }
                    if (entry.Value.Count == 0)
                    {
                        counter++;
                    }
                }
            }
            return 0;
        }

        static void Main(string[] args)
        {
            Asteroid best = GetBestAsteroid(GetMap(Advent2019.Utils.GetInput(args)));
            Console.WriteLine("Part 1: " + best.Amount);
            Console.WriteLine("Part 2: " + DestroyAsteroids(best, 200));
        }
    }
}
