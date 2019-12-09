using System;
using System.Collections.Generic;

namespace Day07
{
    class Program
    {
        /// <summary>
        /// Run all the computers for the given settings
        /// </summary>
        /// <param name="computers">List of all the thrusters' computer</param>
        /// <param name="settings">List of the settings to test, one integer per computer</param>
        /// <returns>The signal returned once all the computers have finished computing</returns>
        static int ThrustersSequence(List<IntcodeComputer> computers, int[] settings)
        {
            int lastSignal = 0;
            bool begin = true;
            while (begin || computers[0].Running)
            {
                begin = false;
                for (int i = 0; i < settings.Length; ++i)
                {
                    List<int> outputs = (computers[i].Running ? computers[i].Resume(lastSignal) : computers[i].Execute(settings[i], lastSignal));
                    if (outputs.Count < 1)
                    {
                        Advent2019.Utils.FatalError("Thruster " + (i + 1) + " doesn't have a signal output for setting " + settings[0] + settings[1] + settings[2] + settings[3] + settings[4] + ".");
                    }
                    lastSignal = outputs[0];
                }
            }
            return lastSignal;
        }

        /// <summary>
        /// Swap the element at index a and b in the list passed as parameter.
        /// </summary>
        /// <param name="setting">The list to swap</param>
        /// <param name="a">Index of the first element</param>
        /// <param name="b">Index of the second element</param>
        static void SwapSetting(int[] setting, int a, int b)
        {
            int backup = setting[a];
            setting[a] = setting[b];
            setting[b] = backup;
        }

        /// <summary>
        /// Recursive fonction to test all the possible combination of settings.
        /// </summary>
        /// <param name="computers">List of the computers to run the settings on</param>
        /// <param name="settings">List of integers representing the current settings</param>
        /// <param name="idx">First index being swapped in the settings</param>
        /// <param name="end">Second index being swapped in the settings</param>
        /// <param name="highest">Current highest output returned by ThrustersSquence</param>
        /// <returns>The highest processed output</returns>
        static int SettingPermutation(List<IntcodeComputer> computers, int[] settings, int idx, int end, int highest)
        {
            if (idx == end)
            {
                int signal = ThrustersSequence(computers, settings);
                if (signal > highest)
                {
                    return signal;
                }
            }
            else
            {
                for (int i = idx; i <= end; ++i)
                {
                    SwapSetting(settings, idx, i);
                    highest = SettingPermutation(computers, settings, idx + 1, end, highest);
                    SwapSetting(settings, idx, i);
                }
            }
            return highest;
        }

        /// <summary>
        /// Initialize the recursive function to find the highest possible thruster value.
        /// </summary>
        /// <param name="computer">Default computer containing the opcodes</param>
        /// <param name="settings">Default settings</param>
        /// <returns>The highest thruster signal found</returns>
        static int FindHighestSignal(IntcodeComputer computer, int[] settings)
        {
            List<IntcodeComputer> computers = new List<IntcodeComputer>();
            for (int i = 0; i < settings.Length; ++i)
            {
                computers.Add(new IntcodeComputer(computer));
            }
            return SettingPermutation(computers, settings, 0, settings.Length - 1, 0);
        }

        static void Main(string[] args)
        {
            Day07.IntcodeComputer computer = new IntcodeComputer(Advent2019.Utils.GetInput(args));
            Console.WriteLine("Part 1: " + FindHighestSignal(computer, new int[] { 0, 1, 2, 3, 4 }));
            Console.WriteLine("Part 2: " + FindHighestSignal(computer, new int[] { 5, 6, 7, 8, 9 }));
        }
    }
}
