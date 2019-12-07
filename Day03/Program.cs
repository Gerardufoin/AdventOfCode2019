using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// For this exercice, I transformed the coordinates passed as input into lines in a 2D plane. I also separated vertical and horizontal lines to prevent unnecessary checks.
/// </summary>
namespace Day03
{
    /// <summary>
    /// Line class containing the 4 coordinates and the total length of the segment since the start.
    /// </summary>
    class Line
    {
        public int X1;
        public int Y1;
        public int X2;
        public int Y2;
        public int TotalSize;

        public Line(int x1, int y1, int x2, int y2, int currentSize)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
            TotalSize = currentSize;
        }
    }

    /// <summary>
    /// Class Wire containing multiple lines separated into vertical and horizontal.
    /// </summary>
    class Wire
    {
        // Separating vertical and horizontal lines allow to divide by 2 the numbers of checks needed to find the intersections
        public List<Line> _verticalLines = new List<Line>();
        public List<Line> _horizontalLines = new List<Line>();

        // P1 result
        public int ShrtManhattan;
        // P2 result
        public int ShrtLength;

        /// <summary>
        /// Recuperation and processing of the input into lines.
        /// </summary>
        /// <param name="data">Input for this wire</param>
        public Wire(string data)
        {
            MatchCollection matches = Regex.Matches(data, @"(?<Direction>[RDUL])(?<Distance>\d+)");
            Line current = new Line(0, 0, 0, 0, 0);
            foreach (Match match in matches)
            {
                int dist = int.Parse(match.Groups["Distance"].Value);
                switch (match.Groups["Direction"].Value)
                {
                    case "R":
                        current.X2 += dist;
                        break;
                    case "L":
                        current.X2 -= dist;
                        break;
                    case "U":
                        current.Y2 += dist;
                        break;
                    case "D":
                        current.Y2 -= dist;
                        break;
                }
                current.TotalSize += dist;
                if (current.X1 == current.X2)
                {
                    _verticalLines.Add(current);
                }
                else
                {
                    _horizontalLines.Add(current);
                }
                current = new Line(current.X2, current.Y2, current.X2, current.Y2, current.TotalSize);
            }
        }

        /// <summary>
        /// Check if the two lines given as parameter intersect with each other.
        /// If it is the case, the length and manhattan distance are calculated and stored if they happend to be the current lowest.
        /// </summary>
        /// <param name="vl">Vertical Line</param>
        /// <param name="hl">Horizontal Line</param>
        private void IntersectionDistance(Line vl, Line hl)
        {
            if (vl.X1 == 0 && vl.Y1 == 0 && hl.X1 == 0 && hl.Y1 == 0)
                return;
            if (vl.X1 >= Math.Min(hl.X1, hl.X2) && vl.X1 <= Math.Max(hl.X1, hl.X2) &&
                hl.Y1 >= Math.Min(vl.Y1, vl.Y2) && hl.Y1 <= Math.Max(vl.Y1, vl.Y2))
            {
                int dist = Math.Abs(hl.Y1) + Math.Abs(vl.X1);
                if (this.ShrtManhattan < 0 || dist < this.ShrtManhattan)
                {
                    this.ShrtManhattan = dist;
                }
                int length = vl.TotalSize - Math.Abs(vl.Y2 - hl.Y1) + hl.TotalSize - Math.Abs(hl.X2 - vl.X1);
                if (this.ShrtLength < 0 || length < this.ShrtLength)
                {
                    this.ShrtLength = length;
                }
            }
        }

        /// <summary>
        /// Compare this Wire with another one. The vertical lines of W1 are compared with the horizontal of W2 and inversely.
        /// </summary>
        /// <param name="other">The other Wire compared to</param>
        public void ComputeClosestIntersectionDistances(Wire other)
        {
            this.ShrtManhattan = -1;
            this.ShrtLength = -1;
            foreach (Line l1 in this._horizontalLines)
            {
                foreach (Line l2 in other._verticalLines)
                {
                    this.IntersectionDistance(l2, l1);
                }
            }
            foreach (Line l1 in this._verticalLines)
            {
                foreach (Line l2 in other._horizontalLines)
                {
                    this.IntersectionDistance(l1, l2);
                }
            }
            if (this.ShrtManhattan == -1)
            {
                Advent2019.Utils.FatalError("The two wires don't intersect.");
            }
        }
    }

    class Program
    {
        /// <summary>
        /// Transform the input into an array of string containing the data of each Wire.
        /// </summary>
        /// <param name="input">Text input of the program contained in the file passed as parameter.</param>
        /// <returns>Array of string containing the wires' datas</returns>
        static string[] GetLines(string input)
        {
            return Regex.Matches(input, @"((?:[RDUL]\d+,?)+)").Cast<Match>().Select(match => match.Value).ToArray();
        }

        static void Main(string[] args)
        {
            string[] linesData = GetLines(Advent2019.Utils.GetInput(args));
            if (linesData.Length < 2)
            {
                Advent2019.Utils.FatalError("Not enough lines found in the input file.");
            }
            Wire line1 = new Wire(linesData[0]);
            Wire line2 = new Wire(linesData[1]);
            line1.ComputeClosestIntersectionDistances(line2);
            Console.WriteLine("Part 1: " + line1.ShrtManhattan);
            Console.WriteLine("Part 2: " + line1.ShrtLength);
        }
    }
}
