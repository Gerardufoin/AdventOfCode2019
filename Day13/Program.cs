using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace Day13
{
    class Program
    {
        /// <summary>
        /// Update the map and the score with the output of the intcode computer.
        /// </summary>
        /// <param name="values">Output of the intcode computer</param>
        /// <param name="score">Current score</param>
        /// <param name="map">Current map</param>
        static void ProcessValues(List<BigInteger> values, ref int score, Dictionary<(int x, int y), int> map)
        {
            if (values.Count % 3 != 0)
            {
                Advent2019.Utils.FatalError("Intcode computer didn't output a set of 3 variables.");
            }
            for (int i = 0; i < values.Count; i += 3)
            {
                (int x, int y, int v) cur = ((int)values[i], (int)values[i + 1], (int)values[i + 2]);
                if (cur.x == -1 && cur.y == 0)
                {
                    score = cur.v;
                }
                else
                {
                    if (!map.ContainsKey((cur.x, cur.y)))
                    {
                        map.Add((cur.x, cur.y), cur.v);
                    }
                    else
                    {
                        map[(cur.x, cur.y)] = cur.v;
                    }
                }
            }
        }

        #region Part 1
        /// <summary>
        /// Create the map in its default state and return the number of blocks.
        /// </summary>
        /// <param name="input">The input of the program</param>
        /// <returns>The number of blocks</returns>
        static int GetWallCount(string input)
        {
            IntcodeComputer computer = new IntcodeComputer(input);
            Dictionary<(int x, int y), int> map = new Dictionary<(int x, int y), int>();
            int score = 0;
            ProcessValues(computer.Execute(), ref score, map);
            return map.Where(d => d.Value == 2).ToArray().Count();
        }
        #endregion

        #region Part 2
        /// <summary>
        /// Used only to draw the map for debug (or entertainement) purposes.
        /// </summary>
        /// <param name="code">The tile code to convert to char</param>
        /// <returns>The display char linked to the tile code</returns>
        static char GetCharFromTileCode(int code)
        {
            switch (code)
            {
                case 0:
                    return ' ';
                case 1:
                    return '#';
                case 2:
                    return '+';
                case 3:
                    return '_';
                case 4:
                    return 'o';
            }
            Advent2019.Utils.FatalError("'" + code + "' is not a valide tile code.");
            return ' ';
        }

        /// <summary>
        /// Draw the map in its current state. Only used for debug/entertainement purposes.
        /// </summary>
        /// <param name="score">Current score to display</param>
        /// <param name="map">Current map to display</param>
        static void DrawMap(int score, Dictionary<(int x, int y), int> map)
        {
            Console.Clear();
            Console.WriteLine("SCORE: " + score);
            (int width, int height) limits = (map.Max(v => v.Key.x), map.Max(v => v.Key.y));
            for (int y = 0; y <= limits.height; ++y)
            {
                for (int x = 0; x <= limits.width; ++x)
                {
                    Console.Write(GetCharFromTileCode(map.ContainsKey((x, y)) ? map[(x, y)] : 0));
                }
                Console.WriteLine();
            }
            System.Threading.Thread.Sleep(100);
        }

        /// <summary>
        /// Get the next paddle move based on the ball position.
        /// </summary>
        /// <param name="map">The current state of the game</param>
        /// <returns>-1 if the paddle is on the right of the ball, 1 if the paddle is on the left of the ball, 0 otherwise</returns>
        static int GetNextMove(Dictionary<(int x, int y), int> map)
        {
            (int x, int y) ball = map.Where(m => m.Value == 4).First().Key;
            (int x, int y) paddle = map.Where(m => m.Value == 3).First().Key;
            return (paddle.x < ball.x ? 1 : (paddle.x > ball.x ? -1 : 0));
        }

        /// <summary>
        /// Cheating is the way. Why play a game when the computer can play it for you, hmm? We livin' in the future baby.
        /// </summary>
        /// <param name="input">The input of the program</param>
        /// <returns>The final score</returns>
        static int CheatYourWayToVictory(string input)
        {
            IntcodeComputer computer = new IntcodeComputer("2" + input.Substring(1));
            Dictionary<(int x, int y), int> map = new Dictionary<(int x, int y), int>();
            int score = 0;
            do
            {
                ProcessValues((computer.Running ? computer.Execute(GetNextMove(map)) : computer.Execute()), ref score, map);
                if (map.Count == 0)
                {
                    Advent2019.Utils.FatalError("Computer generated an empty map.");
                }
                //DrawMap(score, map);
            } while (computer.Running == true);
            return score;
        }
        #endregion

        static void Main(string[] args)
        {
            string input = Advent2019.Utils.GetInput(args);
            int p1 = GetWallCount(input);
            int p2 = CheatYourWayToVictory(input);
            Console.WriteLine("Part 1: " + p1);
            Console.WriteLine("Part 2: " + p2);
        }
    }
}
