using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Welp, seems like it's going to be a reccuring theme. Time to create a proper intcode computer.
/// </summary>
namespace Day05
{
    /// <summary>
    /// Class used to interpret a serie of opcodes.
    /// </summary>
    class IntcodeComputer
    {
        // Default value of the opcodes in order to execute multiple times
        private int[] _baseOpcodes;
        // Current opcodes being interpreted by the Execute method
        private int[] _opcodes;
        // Current index of the instruction being executed
        private int _idx;
        // List of the instructions
        private Dictionary<int, Func<IntcodeComputer, List<int>, bool>> _instructions = new Dictionary<int, Func<IntcodeComputer, List<int>, bool>>()
        {
            { 1, (ic, m) => ic.Add(m) },
            { 2, (ic, m) => ic.Multiply(m) },
            { 3, (ic, m) => ic.Input(m) },
            { 4, (ic, m) => ic.Output(m) },
            { 5, (ic, m) => ic.JumpIfTrue(m) },
            { 6, (ic, m) => ic.JumpIfFalse(m) },
            { 7, (ic, m) => ic.LessThan(m) },
            { 8, (ic, m) => ic.Equals(m) },
            { 99, (ic, m) => ic.Halt(m) },
        };
        // List of the parameters modes
        private Dictionary<int, Func<IntcodeComputer, int, int>> _modes = new Dictionary<int, Func<IntcodeComputer, int, int>>()
        {
            { 0, (ic, v) => ic.PositionMode(v) },
            { 1, (ic, v) => ic.ImmediateMode(v) }
        };

        // Inputs passed to the Execute method
        private int[] _inputs;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="opcodes">A string containing the opcodes of the computer</param>
        public IntcodeComputer(string opcodes)
        {
            _baseOpcodes = Regex.Matches(opcodes, @"-?(\d+)", RegexOptions.Singleline).Cast<Match>().Select(match => int.Parse(match.Value)).ToArray();
        }

        /// <summary>
        /// Reset the opcodes and run the program with the given inputs
        /// </summary>
        /// <param name="inputs">An unknown number of ints (but only the first one will be used for Day05)</param>
        public void Execute(params int[] inputs)
        {
            int code = 0;
            List<int> modes = new List<int>();
            _inputs = inputs;
            _opcodes = (int[])_baseOpcodes.Clone();
            _idx = 0;
            do
            {
                code = DecodeInstruction(_opcodes[_idx], modes);
                if (!_instructions.ContainsKey(code))
                {
                    Advent2019.Utils.FatalError("'" + code + "' is not a valid instruction.");
                }
            } while (_instructions[code](this, modes) && _idx < _opcodes.Length);
        }

        /// <summary>
        /// Take an instruction and separate the instruction code from the parameters modes
        /// </summary>
        /// <param name="inst">The instruction</param>
        /// <param name="modes">The list of modes to reset and populate</param>
        /// <returns>The code of the instruction</returns>
        private int DecodeInstruction(int inst, List<int> modes)
        {
            int code = inst % 100;
            modes.Clear();
            inst /= 100;
            while (inst > 0)
            {
                modes.Add(inst % 10);
                inst /= 10;
            }
            return code;
        }

        /// <summary>
        /// Get a value from the opcode memory based on the parameter mode
        /// </summary>
        /// <param name="mode">Parameter mode to use to get the value</param>
        /// <param name="idx">Id of the opcode being processed</param>
        /// <returns>The processed value</returns>
        private int GetValue(int mode, int idx)
        {
            if (idx >= _opcodes.Length)
            {
                Advent2019.Utils.FatalError("Accessing opcode outside of memory.");
            }
            if (!_modes.ContainsKey(mode))
            {
                Advent2019.Utils.FatalError("'" + mode + "' is not a supported mode.");
            }
            return _modes[mode](this, _opcodes[idx]);
        }

        /// <summary>
        /// Get a value from the opcode memory based on a specific parameter and its mode
        /// </summary>
        /// <param name="modes">The list of modes for this instruction</param>
        /// <param name="offset">The offset of the parameter in the modes list</param>
        /// <returns>The processed value</returns>
        private int GetValue(List<int> modes, int offset)
        {
            return GetValue((offset < modes.Count ? modes[offset] : 0), _idx + offset + 1);
        }

        /// <summary>
        /// Write a value in the opcodes memory at the given index
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <param name="idx">The index to write to</param>
        private void WriteValue(int value, int idx)
        {
            int widx = GetValue(1, idx);
            if (widx >= _opcodes.Length)
            {
                Console.WriteLine("Wrote outside of memory.");
            }
            _opcodes[widx] = value;
        }

        /// <summary>
        /// The instructions take a list of modes and return a boolean.
        /// </summary>
        /// <param name="modes">The modes for this instruction.</param>
        /// <returns>False if the execution has to stop, true otherwise.</returns>
        #region Instructions
        private bool Add(List<int> modes)
        {
            WriteValue(GetValue(modes, 0) + GetValue(modes, 1), _idx + 3);
            _idx += 4;
            return true;
        }

        private bool Multiply(List<int> modes)
        {
            WriteValue(GetValue(modes, 0) * GetValue(modes, 1), _idx + 3);
            _idx += 4;
            return true;
        }

        private bool Input(List<int> modes)
        {
            WriteValue(_inputs[0], _idx + 1);
            _idx += 2;
            return true;
        }

        private bool Output(List<int> modes)
        {
            Console.WriteLine("Output: " + GetValue(modes, 0));
            _idx += 2;
            return true;
        }

        private bool JumpIfTrue(List<int> modes)
        {
            _idx = (GetValue(modes, 0) != 0 ? GetValue(modes, 1) : _idx + 3);
            return true;
        }
        private bool JumpIfFalse(List<int> modes)
        {
            _idx = (GetValue(modes, 0) == 0 ? GetValue(modes, 1) : _idx + 3);
            return true;
        }
        private bool LessThan(List<int> modes)
        {
            WriteValue((GetValue(modes, 0) < GetValue(modes, 1) ? 1 : 0), _idx + 3);
            _idx += 4;
            return true;
        }
        private bool Equals(List<int> modes)
        {
            WriteValue((GetValue(modes, 0) == GetValue(modes, 1) ? 1 : 0), _idx + 3);
            _idx += 4;
            return true;
        }

        private bool Halt(List<int> modes)
        {
            return false;
        }
        #endregion

        /// <summary>
        /// The parameter modes take a value as parameter and return a value based on the mode.
        /// </summary>
        /// <param name="value">Value in the opcodes memory</param>
        /// <returns>Processed value</returns>
        #region Parameter modes
        private int PositionMode(int value)
        {
            if (_opcodes.Length <= value)
            {
                Advent2019.Utils.FatalError("Accessing a value outside of memory.");
            }
            return _opcodes[value];
        }

        private int ImmediateMode(int value)
        {
            return value;
        }
        #endregion


        /// <summary>
        /// Used to debug the instruction which write something in memory.
        /// </summary>
        /// <param name="name">Name of the instruction.</param>
        /// <param name="count">Number of parameter, instruction comprised.</param>
        /// <param name="modes">List of modes for this instruction.</param>
        private void DebugWrite(string name, int count, List<int> modes)
        {
            string msg = name + ":";
            for (int i = 0; i < count; ++i)
            {
                msg += " " + _opcodes[_idx + i];
                if (i > 0 && i < count - 1)
                {
                    msg += "(" + GetValue(modes, i - 1) + ")";
                }
            }
            Console.WriteLine(msg);
        }
    }
}
