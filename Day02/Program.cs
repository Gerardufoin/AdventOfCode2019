using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day02
{
    class Program
    {
        #region Part 1
        /// <summary>
        /// Transform the input text into an array of integer.
        /// </summary>
        /// <param name="input">Text input of the exercice. Here a list of int separated by commas.</param>
        /// <returns>An array of ints containing all opcodes.</returns>
        static int[] GetOpcodes(string input)
        {
            return Regex.Matches(input, @"(\d+)", RegexOptions.Singleline).Cast<Match>().Select(match => int.Parse(match.Value)).ToArray();
        }

        /// <summary>
        /// Apply a two arguments function passed as parameter to the opcodes.
        /// </summary>
        /// <param name="opcodes">The list of opcodes.</param>
        /// <param name="idx">The current command being executed.</param>
        /// <param name="operation">The operation to apply on the opcodes.</param>
        static void ApplyOperation(ref int[] opcodes, int idx, Func<int, int, int> operation)
        {
            for (int i = 1; i <= 3; ++i)
            {
                if (idx + i >= opcodes.Length || opcodes[idx + i] >= opcodes.Length)
                {
                    Advent2019.Utils.FatalError("Tried to access an opcode out of bond.");
                }
            }
            opcodes[opcodes[idx + 3]] = operation(opcodes[opcodes[idx + 1]], opcodes[opcodes[idx + 2]]);
        }

        /// <summary>
        /// Read the list of opcodes and execute the instructions contained within until the end or until an instruction 99 is uencountered.
        /// List of instructions:
        /// 1: Add the opcodes at i + 1 and i + 3 and place them at the index contained in i + 3
        /// 2: Multiply the opcodes at i + 1 and i + 3 and place them at the index contained in i + 3
        /// 99: Halt the execution.
        /// </summary>
        /// <param name="baseOpcodes">The default list of opcodes. Will be cloned to not be modified.</param>
        /// <param name="noun">The instruction placed at the index 1 of the list.</param>
        /// <param name="verb">The instruction placed at the index 2 of the list.</param>
        /// <returns>Return the value at the index 0 of the list once everything has been executed.</returns>
        static int ExecuteOpcodes(int[] baseOpcodes, int noun, int verb)
        {
            int[] opcodes = (int[])baseOpcodes.Clone();
            int idx = 0;
            if (opcodes.Length < 3)
            {
                Advent2019.Utils.FatalError("Not enough opcodes found in input file.");
            }
            // Applying inputs
            opcodes[1] = noun;
            opcodes[2] = verb;
            while (idx < opcodes.Length)
            {
                switch (opcodes[idx])
                {
                    case 1:
                        ApplyOperation(ref opcodes, idx, (x, y) => x + y);
                        break;
                    case 2:
                        ApplyOperation(ref opcodes, idx, (x, y) => x * y);
                        break;
                    case 99:
                        return opcodes[0];
                    default:
                        Advent2019.Utils.FatalError("'" + opcodes[idx] + "' is not a valid instruction.");
                        break;
                }
                idx += 4;
            }
            return opcodes[0];
        }
        #endregion

        #region Part 2
        /// <summary>
        /// Test multiple input values between 0 and 99 included until the desired output value is found.
        /// </summary>
        /// <param name="opcodes">The default list of opcodes.</param>
        /// <param name="output">The desired output to find.</param>
        /// <returns>The two values resulting in the desired output applied to the following formula: 100 * noun + verb.</returns>
        static int FindInputs(int[] opcodes, int output)
        {
            for (int noun = 0; noun <= 99; ++noun)
            {
                for (int verb = 0; verb <= 99; ++verb)
                {
                    if (ExecuteOpcodes(opcodes, noun, verb) == output)
                    {
                        return 100 * noun + verb;
                    }
                }
            }
            Advent2019.Utils.FatalError("No combination of noun/verb resulted into " + output + ".");
            return 0;
        }
        #endregion

        static void Main(string[] args)
        {
            int[] opcodes = GetOpcodes(Advent2019.Utils.GetInput(args));
            Console.WriteLine("Part 1: " + ExecuteOpcodes(opcodes, 12, 2));
            Console.WriteLine("Part 2: " + FindInputs(opcodes, 19690720));
        }
    }
}
