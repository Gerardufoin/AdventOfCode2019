using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Day16
{
    class Program
    {
        /// <summary>
        /// Get the signal from the input and split it into a list of integer
        /// </summary>
        /// <param name="input">Input got from the file passed to the program</param>
        /// <returns>The list of each digit of the signal</returns>
        static List<int> GetSignal(string input)
        {
            return Regex.Matches(input, @"\d{1}").Cast<Match>().Select(c => int.Parse(c.Value)).ToList();
        }

        #region Part 1
        /// <summary>
        /// Apply the given pattern to the signal.
        /// </summary>
        /// <param name="signal">Current state of the signal</param>
        /// <param name="pattern">Pattern to apply</param>
        /// <returns>The new state of the signal</returns>
        static List<int> ApplyPattern(List<int> signal, int[] pattern)
        {
            List<int> newPattern = new List<int>();
            for (int i = 1; i < signal.Count + 1; ++i)
            {
                newPattern.Add(Math.Abs(signal.Select((s, idx) => s * pattern[(idx + 1) / i % pattern.Length]).Sum() % 10));
            }
            return newPattern;
        }

        /// <summary>
        /// Apply the pattern to the signal as many time as phases.
        /// </summary>
        /// <param name="signal">The signal to modify</param>
        /// <param name="pattern">The pattern to apply</param>
        /// <param name="phases">Number of time the pattern has to be applied</param>
        /// <returns>The modified signal</returns>
        static List<int> FlawedFrequencyTransmission(List<int> signal, int[] pattern, int phases)
        {
            for (int i = 0; i < phases; ++i)
            {
                signal = ApplyPattern(signal, pattern);
            }
            return signal;
        }

        #endregion

        #region Part 2 (slow ~20sec)
        /// <summary>
        /// Create a list of n times the signal.
        /// </summary>
        /// <param name="signal">Signal to duplicate</param>
        /// <param name="n">Number of time the signal has to be duplicated</param>
        /// <returns>A list containing n times the signal</returns>
        static List<int> RepeatSignal(List<int> signal, int n)
        {
            List<int> ret = new List<int>();
            for (int i = 0; i < n; ++i)
            {
                ret.AddRange(signal);
            }
            return ret;
        }

        /// <summary>
        /// A new signal is created by additioning all the values from the end of the signal to the start.
        /// </summary>
        /// <param name="signal">The current signal</param>
        /// <returns>The new signal</returns>
        static List<int> ApplyPatternForP2(List<int> signal)
        {
            LinkedList<int> newPattern = new LinkedList<int>();
            for (int i = signal.Count - 1; i >= 0; --i)
            {
                if (newPattern.Count == 0)
                {
                    newPattern.AddFirst(signal[i]);
                }
                else
                {
                    newPattern.AddFirst((newPattern.First() + signal[i]) % 10);
                }
            }
            return new List<int>(newPattern);
        }

        /// <summary>
        /// If we analyse the given pattern [0, 1, 0, -1] in our current situation, we can deduce 2 things.
        /// First, once we reach the addition of offset, all the value before will be multiplied by 0, same with all the following values.
        /// As such, we can simply ignore the beginning of the signal until the offset.
        /// Second, as the value we are dealing with is so big, the value of the pattern will always be 0 from the offset to the end. Meaning all the values after the offset will always be positive and non zero.
        /// That means that the value 0 of the offset is the addition of all the value since the end of the signal until the offset. The value 1 is the sum from the end to offset+1, and so on.
        /// </summary>
        /// <param name="signal"></param>
        /// <param name="phases"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        static List<int> FFTWithOffset(List<int> signal, int phases, int offset)
        {
            // Because our pattern begins with 0, all numbers before the offset will be 0 once we reach the offset
            signal.RemoveRange(0, offset);
            for (int i = 0; i < phases; ++i)
            {
                signal = ApplyPatternForP2(signal);
            }
            return signal;
        }
        #endregion

        #region Math Part 2 (failed to simplify the formula, unusable as it is, longer than the previous method)
        /// <summary>
        /// Tried to optimized the second part by finding a mathematical formula allowing you to directly calculate the value of the offset without the need to loop 100 times.
        /// The idea works in theory but to find the correct formula there is a need to loop through multiple value in a recursive manner which takes much more time than the simpler method.
        /// It may be possible to simplify the formula and prevent the need to loop recursively, but unfortunately that's beyond my mathematical knowledges :(
        /// </summary>

        static long DeepPhibonacci(int n, int depth, Dictionary<(int num, int depth), int> memory)
        {
            if (depth < 1)
            {
                return 1;
            }
            long val = 0;
            Stack<(int n, int d)> result = new Stack<(int n, int d)>();
            result.Push((n, depth - 1));
            while (result.Count > 0)
            {
                Console.WriteLine(result.Count);
                (int n, int d) first = result.First();
                result.Pop();
                if (first.d > 0)
                {
                    for (int i = 1; i <= (first.d == depth - 1 ? first.n - 1 : first.n); ++i)
                    {
                        result.Push((i, first.d - 1));
                    }
                }
                val += first.n;
            }
            return val;
        }

        static List<long> CreateMultiTable(int phases, int signalLength)
        {
            List<long> table = new List<long>();
            Dictionary<(int num, int depth), int> memory = new Dictionary<(int num, int depth), int>();
            for (int i = 0; i < signalLength; ++i)
            {
                table.Add(DeepPhibonacci(phases, i, memory));
            }
            return table;
        }

        static int OptiFFTWithOffset(List<int> signal, int phases, int offset)
        {
            // Because our pattern begins with 0, all numbers before the offset will be 0 once we reach the offset
            signal.RemoveRange(0, offset);
            List<long> test = CreateMultiTable(4, 10);// signal.Count);
            for (int i = 0; i < 10; ++i)
            {
                Console.WriteLine(test[i]);
            }
            return 0;
        }
        #endregion

        static void Main(string[] args)
        {
            List<int> signal = GetSignal(Advent2019.Utils.GetInput(args));
            int[] pattern = new int[] { 0, 1, 0, -1 };
            Console.WriteLine("Part 1: " + string.Join("", FlawedFrequencyTransmission(signal, pattern, 100).Take(8).Select(s => Math.Abs(s).ToString()).ToArray()));
            int offset = (int)signal.Take(7).Select((s, i) => s * Math.Pow(10, 6 - i)).Sum();
            Console.WriteLine("Part 2: " + string.Join("", FFTWithOffset(RepeatSignal(signal, 10000), 100, offset).Take(8).Select(s => Math.Abs(s).ToString()).ToArray()));
        }
    }
}
