using System;
using System.Collections.Generic;
using System.Numerics;

namespace Day11
{
    class Program
    {
        /// <summary>
        /// Emergency Hull Painting Robot. Will paint your hull for free as long as you know intcode. So convenient.
        /// </summary>
        class PaintingRobot
        {
            public (int x, int y) Coord;
            private int Direction;

            public PaintingRobot()
            {
                Coord = (0, 0);
                Direction = 0;
            }

            /// <summary>
            /// Paint the hull at the given position based on the order.
            /// </summary>
            /// <param name="order">Paint order. 0 for black, 1 for white</param>
            /// <param name="hull">Hull of the ship consisting of color and coordinates</param>
            public void Paint(int order, Dictionary<(int x, int y), char> hull)
            {
                if (order >= 0 && !hull.ContainsKey(Coord))
                {
                    hull.Add(Coord, '.');
                }
                switch (order)
                {
                    case 0:
                        hull[Coord] = '.';
                        break;
                    case 1:
                        hull[Coord] = '#';
                        break;
                }
            }

            /// <summary>
            /// Turn the robot based on order and then move forward.
            /// </summary>
            /// <param name="order">Turn the robot. 0 for left, 1 for right</param>
            public void Move(int order)
            {
                if (order >= 0 && order <= 1)
                {
                    Turn((order == 0 ? -1 : 1));
                    if (Direction % 2 == 0)
                    {
                        Coord.y += (Direction == 0 ? -1 : 1);
                    }
                    else
                    {
                        Coord.x += (Direction == 1 ? 1 : -1);
                    }
                }
            }

            /// <summary>
            /// Turn the robot based on dir. Direction is always between 0 and 3, 0 being up and 1 being right and so on
            /// </summary>
            /// <param name="dir">Turn the robot. 0 for left, 1 for right</param>
            private void Turn(int dir)
            {
                Direction = (Direction + dir) % 4;
                if (Direction < 0)
                {
                    Direction = 3;
                }
            }
        }

        /// <summary>
        /// Get the result of the paint job done by PaintingRobot based on the Intcode program passed as input
        /// </summary>
        /// <param name="computer">The IntcodeComputer containing the brain of the robot</param>
        /// <param name="startColor">Color of the default position of the robot</param>
        /// <returns>A dictionary of coordinates with their color</returns>
        static Dictionary<(int x, int y), char> GetPaint(IntcodeComputer computer, char startColor)
        {
            Dictionary<(int x, int y), char> hullPaint = new Dictionary<(int x, int y), char>();
            hullPaint.Add((0, 0), startColor);
            PaintingRobot robot = new PaintingRobot();
            do
            {
                List<BigInteger> outputs = computer.Execute((hullPaint.ContainsKey(robot.Coord) && hullPaint[robot.Coord] == '#' ? 1 : 0));
                robot.Paint((outputs.Count > 0 ? (int)outputs[0] : -1), hullPaint);
                robot.Move((outputs.Count > 1 ? (int)outputs[1] : -1));
            } while (computer.Running);
            return hullPaint;
        }

        /// <summary>
        /// Display a painted hull in the console.
        /// </summary>
        /// <param name="hull">A dictionary of coordinates and color representing the hull to paint</param>
        static void ShowPaint(Dictionary<(int x, int y), char> hull)
        {
            (int x, int y) min = (0, 0);
            (int x, int y) max = (0, 0);
            (int x, int y) offset = (0, 0);
            foreach (KeyValuePair<(int x, int y), char> entry in hull)
            {
                min = (Math.Min(min.x, entry.Key.x), Math.Min(min.y, entry.Key.y));
                max = (Math.Max(max.x, entry.Key.x), Math.Max(max.y, entry.Key.y));
            }
            offset.x = (min.x < 0 ? -min.x : 0);
            offset.y = (min.y < 0 ? -min.y : 0);
            for (int y = 0; y <= max.y + offset.y; ++y)
            {
                for (int x = 0; x <= max.x + offset.x; ++x)
                {
                    (int x, int y) coord = (x + offset.x, y + offset.y);
                    Console.Write((hull.ContainsKey(coord) ? hull[coord] : '.'));
                }
                Console.WriteLine();
            }
        }

        static void Main(string[] args)
        {
            IntcodeComputer computer = new IntcodeComputer(Advent2019.Utils.GetInput(args));
            Console.WriteLine("Part 1: " + GetPaint(computer, '.').Count);
            Console.WriteLine("Part 2:");
            ShowPaint(GetPaint(computer, '#'));
        }
    }
}
