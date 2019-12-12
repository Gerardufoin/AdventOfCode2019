using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Day12
{
    class Program
    {
        /// <summary>
        /// Moon class, contains all the informations about a moon, namely the Position and the Velocity
        /// </summary>
        class Moon
        {
            // Position of the moon
            public Vector3 Position;
            // Velocity of the moon
            public Vector3 Velocity;

            public Moon(Vector3 position)
            {
                Position = position;
                Velocity = new Vector3();
            }

            /// <summary>
            /// Update the velocities between this moon and another one based on their position
            /// </summary>
            /// <param name="other">The other moon to update</param>
            public void UpdateVelocity(Moon other)
            {
                GravityPull((int)Position.X, (int)other.Position.X, ref Velocity.X, ref other.Velocity.X);
                GravityPull((int)Position.Y, (int)other.Position.Y, ref Velocity.Y, ref other.Velocity.Y);
                GravityPull((int)Position.Z, (int)other.Position.Z, ref Velocity.Z, ref other.Velocity.Z);
            }

            /// <summary>
            /// Apply a gravitational pull to the velocity of two moons, based on their positions
            /// </summary>
            /// <param name="pa">Position A of the first moon</param>
            /// <param name="pb">Position A of the second moon</param>
            /// <param name="va">Velocity A of the first moon</param>
            /// <param name="vb">Velocity A of the second moon</param>
            private void GravityPull(int pa, int pb, ref float va, ref float vb)
            {
                if (pa < pb)
                {
                    va += 1;
                    vb -= 1;
                }
                else if (pb < pa)
                {
                    va -= 1;
                    vb += 1;
                }
            }

            /// <summary>
            /// Update the Position with the current Velocity
            /// </summary>
            public void ApplyVelocity()
            {
                Position += Velocity;
            }

            /// <summary>
            /// Get the energy for the given vector
            /// </summary>
            /// <param name="vec">Vector3 whose energy we want</param>
            /// <returns>The energy of the vector</returns>
            private int GetEnergy(Vector3 vec)
            {
                return (Math.Abs((int)vec.X) + Math.Abs((int)vec.Y) + Math.Abs((int)vec.Z));
            }

            /// <summary>
            /// Get the potential energy and multiply it with the kinetic energy
            /// </summary>
            /// <returns>The result</returns>
            public int GetTotalEnergy()
            {
                return GetEnergy(Position) * GetEnergy(Velocity);
            }

            /// <summary>
            /// Display the informations of a moon in the console
            /// </summary>
            public void Display()
            {
                Console.WriteLine("pos=" + Position + ", vel=" + Velocity);
            }
        }

        /// <summary>
        /// Transform the input value into an array of Moon, each line being the coordinate of a moon
        /// </summary>
        /// <param name="input">The input passed to the program</param>
        /// <returns>The array of moons</returns>
        static Moon[] GetMoons(string input)
        {
            return Regex.Matches(input, @"(<x=(-?\d+), +y=(-?\d+), +z=(-?\d+)>)").Cast<Match>().Select(m => new Moon(new Vector3(int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value)))).ToArray();
        }

        #region Part 1
        /// <summary>
        /// Debug function. Display the informations of all the moon on the console
        /// </summary>
        /// <param name="moons">The moons to display</param>
        /// <param name="step">The current itération</param>
        static void DisplayMoons(Moon[] moons, long step)
        {
            Console.WriteLine("Step " + step + ":");
            foreach (Moon moon in moons)
            {
                moon.Display();
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Apply the pull between all the moons and then update their position
        /// </summary>
        /// <param name="moons">The moons to update</param>
        static void ApplyPull(Moon[] moons)
        {
            for (int i = 0; i < moons.Length; ++i)
            {
                for (int j = i + 1; j < moons.Length; ++j)
                {
                    moons[i].UpdateVelocity(moons[j]);
                }
                moons[i].ApplyVelocity();
            }
        }

        /// <summary>
        /// Get the system energy after a given number of steps
        /// </summary>
        /// <param name="moons">The moons used by the system</param>
        /// <param name="time">Number of steps</param>
        /// <returns>The total energy after time steps</returns>
        static int GetEnergyAfterTime(Moon[] moons, long time)
        {
            for (long t = 0; t < time; ++t)
            {
                ApplyPull(moons);
            }
            return moons.Sum(x => x.GetTotalEnergy());
        }
        #endregion

        #region Part 2
        /// <summary>
        /// Get the greatest common divident of two numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>Greatest divident between a and b</returns>
        static long gcd(long a, long b)
        {
            return (b == 0 ? a : gcd(b, a % b));
        }

        /// <summary>
        /// Get the lowest common multiple of two numbers
        /// </summary>
        /// <param name="a">First number</param>
        /// <param name="b">Second number</param>
        /// <returns>The lowest common multiple between a and b</returns>
        static long lcm(long a, long b)
        {
            return a * b / gcd(a, b);
        }

        /// <summary>
        /// Get the number of steps needed for the system to go back to its starting position. As each axis work separately, we check how many steps are needed to reset each one and then we find the lowest common multiple between those 3 numbers.
        /// </summary>
        /// <param name="moons">List of the moons in any given configurations (doesn't need to be the initial one)</param>
        /// <returns>The number of steps needed for the system to reset to the initial configuration</returns>
        static long GetStepsToReset(Moon[] moons)
        {
            (long x, long y, long z) steps = (-1, -1, -1);
            List<Vector3> defPos = new List<Vector3>();
            List<Vector3> defVel = new List<Vector3>();
            foreach (Moon m in moons)
            {
                defPos.Add(m.Position);
                defVel.Add(m.Velocity);
            }
            long inc = 1;
            do
            {
                ApplyPull(moons);
                if (steps.x < 0 && moons.Select((m, i) => new { moon = m, idx = i }).All(v => v.moon.Position.X == defPos[v.idx].X && v.moon.Velocity.X == defVel[v.idx].X))
                {
                    steps.x = inc;
                }
                if (steps.y < 0 && moons.Select((m, i) => new { moon = m, idx = i }).All(v => v.moon.Position.Y == defPos[v.idx].Y && v.moon.Velocity.Y == defVel[v.idx].Y))
                {
                    steps.y = inc;
                }
                if (steps.z < 0 && moons.Select((m, i) => new { moon = m, idx = i }).All(v => v.moon.Position.Z == defPos[v.idx].Z && v.moon.Velocity.Z == defVel[v.idx].Z))
                {
                    steps.z = inc;
                }
                ++inc;
            } while (steps.x < 0 || steps.y < 0 || steps.z < 0);
            return lcm(steps.x, lcm(steps.y, steps.z));
        }
        #endregion

        static void Main(string[] args)
        {
            Moon[] moons = GetMoons(Advent2019.Utils.GetInput(args));
            Console.WriteLine("Part 1: " + GetEnergyAfterTime(moons, 1000));
            Console.WriteLine("Part 2: " + GetStepsToReset(moons));
        }
    }
}
