using System;

namespace Day05
{
    class Program
    {
        static void Main(string[] args)
        {
            Day05.IntcodeComputer computer = new Day05.IntcodeComputer(Advent2019.Utils.GetInput(args));
            Console.WriteLine("Part 1:");
            computer.Execute(1);
            Console.WriteLine("Part 2:");
            computer.Execute(5);
        }
    }
}
