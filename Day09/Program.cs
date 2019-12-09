using System;
using System.Collections.Generic;
using System.Numerics;

namespace Day09
{
    class Program
    {
        static void Main(string[] args)
        {
            IntcodeComputer computer = new IntcodeComputer(Advent2019.Utils.GetInput(args));
            List<BigInteger> output = computer.Execute(1);
            Console.WriteLine("Part 1: " + output[0]);
            output = computer.Execute(2);
            Console.WriteLine("Part 2: " + output[0]);
        }
    }
}
