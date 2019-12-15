using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Numerics;

namespace Day15
{
    /// <summary>
    /// Class used to interpret a serie of opcodes.
    /// </summary>
    class IntcodeComputer
    {
        // Set to true to run the computer in debug mode
        public bool DebugMode = false;

        // Default value of the opcodes in order to execute multiple times
        private Dictionary<BigInteger, BigInteger> _baseOpcodes = new Dictionary<BigInteger, BigInteger>();
        // Default size of the memory
        private BigInteger _baseMemorySize;
        // Current opcodes being interpreted by the Execute method
        private Dictionary<BigInteger, BigInteger> _opcodes = new Dictionary<BigInteger, BigInteger>();
        // Current size of the memory
        private BigInteger _memorySize;
        // Current index of the instruction being executed
        private BigInteger _idx;
        // State of the current computation
        private bool _running;
        public bool Running
        {
            get
            {
                return _running;
            }
        }
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
            { 9, (ic, m) => ic.SetRelativeBase(m) },
            { 99, (ic, m) => ic.Halt(m) },
        };
        // List of the parameters modes
        private Dictionary<int, Func<IntcodeComputer, BigInteger, bool, BigInteger>> _modes = new Dictionary<int, Func<IntcodeComputer, BigInteger, bool, BigInteger>>()
        {
            { 0, (ic, v, w) => ic.PositionMode(v, w) },
            { 1, (ic, v, w) => ic.ImmediateMode(v, w) },
            { 2, (ic, v, w) => ic.RelativeMode(v, w) },
        };

        // Inputs passed to the Execute method
        private BigInteger[] _inputs;
        // Current input being used
        private int _inpIdx;
        // List of output values, returned at the end of Execute
        private List<BigInteger> _outputs = new List<BigInteger>();

        // Relative base for the relative mode
        private BigInteger _relativeBase;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="opcodes">A string containing the opcodes of the computer</param>
        public IntcodeComputer(string opcodes)
        {
            MatchCollection matches = Regex.Matches(opcodes, @"-?(\d+)", RegexOptions.Singleline);
            for (int i = 0; i < matches.Count; ++i)
            {
                _baseOpcodes.Add(i, BigInteger.Parse(matches[i].Value));
            }
            _baseMemorySize = matches.Count;
        }

        /// <summary>
        /// CPY CTOR
        /// </summary>
        /// <param name="ic">IntcodeComputer to copy.</param>
        public IntcodeComputer(IntcodeComputer ic)
        {
            this._baseOpcodes = ic._baseOpcodes;
            this._baseMemorySize = ic._baseMemorySize;
        }

        /// <summary>
        /// Main loop of the program's execution.
        /// </summary>
        private void Process()
        {
            int code = 0;
            List<int> modes = new List<int>();
            do
            {
                code = DecodeInstruction(Read(_idx), modes);
                if (!_instructions.ContainsKey(code))
                {
                    Advent2019.Utils.FatalError("'" + code + "' is not a valid instruction.");
                }
            } while (_instructions[code](this, modes) && _idx < _memorySize);
            if (_idx >= _memorySize)
            {
                _running = false;
            }
        }

        /// <summary>
        /// Reset the intcode computer to its default state
        /// </summary>
        private void Reset()
        {
            _opcodes.Clear();
            _opcodes = _baseOpcodes.ToDictionary(entry => entry.Key, entry => entry.Value);
            _memorySize = _baseMemorySize;
            _idx = 0;
            _relativeBase = 0;
        }

        /// <summary>
        /// Reset the opcodes and start the program with the given inputs. If the program is already running, resume it to when it last stopped because an input was missing.
        /// </summary>
        /// <param name="inputs">An unknown number of ints to feed the Input instruction</param>
        /// <returns>The outputs value created during the execution</returns>
        public List<BigInteger> Execute(params BigInteger[] inputs)
        {
            if (!Running)
            {
                Reset();
                _running = true;
            }
            _inputs = inputs;
            _inpIdx = 0;
            _outputs.Clear();
            Process();
            return _outputs;
        }

        /// <summary>
        /// Take an instruction and separate the instruction code from the parameters modes
        /// </summary>
        /// <param name="inst">The instruction</param>
        /// <param name="modes">The list of modes to reset and populate</param>
        /// <returns>The code of the instruction</returns>
        private int DecodeInstruction(BigInteger inst, List<int> modes)
        {
            int code = (int)(inst % 100);
            modes.Clear();
            inst /= 100;
            while (inst > 0)
            {
                modes.Add((int)(inst % 10));
                inst /= 10;
            }
            return code;
        }

        /// <summary>
        /// Read a value at the given index in the opcodes memory. If the address does not exist, 0 is returned.
        /// </summary>
        /// <param name="idx">Where to read in the memory</param>
        /// <returns>The value read from the memory at index idx</returns>
        private BigInteger Read(BigInteger idx)
        {
            if (idx < 0)
            {
                Advent2019.Utils.FatalError("Tried to access memory at a negative address.");
            }
            if (_opcodes.ContainsKey(idx))
            {
                return _opcodes[idx];
            }
            return 0;
        }

        /// <summary>
        /// Write a value at a specific index in the opcodes memory. If the address does not exist, a new entry is created with the value.
        /// </summary>
        /// <param name="value">Value to add in memory</param>
        /// <param name="idx">Index in memory where to write the value</param>
        private void Write(BigInteger value, BigInteger idx)
        {
            if (idx < 0)
            {
                Advent2019.Utils.FatalError("Tried to access memory at a negative address.");
            }
            if (_opcodes.ContainsKey(idx))
            {
                _opcodes[idx] = value;
            }
            else
            {
                _opcodes.Add(idx, value);
                _memorySize = (idx > _memorySize ? idx : _memorySize);
            }
        }

        /// <summary>
        /// Get a value from the opcode memory based on the parameter mode
        /// </summary>
        /// <param name="mode">Parameter mode to use to get the value</param>
        /// <param name="idx">Id of the opcode being processed</param>
        /// <param name="write">Is the value used for writing in memory</param>
        /// <returns>The processed value</returns>
        private BigInteger GetValue(int mode, BigInteger idx, bool write = false)
        {
            if (!_modes.ContainsKey(mode))
            {
                Advent2019.Utils.FatalError("'" + mode + "' is not a supported mode.");
            }
            return _modes[mode](this, Read(idx), write);
        }

        /// <summary>
        /// Get a value from the opcode memory based on a specific parameter and its mode
        /// </summary>
        /// <param name="modes">The list of modes for this instruction</param>
        /// <param name="offset">The offset of the parameter in the modes list</param>
        /// <param name="write">Is the value used for writing in memory</param>
        /// <returns>The processed value</returns>
        private BigInteger GetValue(List<int> modes, int offset, bool write = false)
        {
            return GetValue((offset < modes.Count ? modes[offset] : 0), _idx + offset + 1, write);
        }

        /// <summary>
        /// The instructions take a list of modes and return a boolean.
        /// </summary>
        /// <param name="modes">The modes for this instruction.</param>
        /// <returns>False if the execution has to stop, true otherwise.</returns>
        #region Instructions
        private bool Add(List<int> modes)
        {
            Debug("Add", 4, modes, true);
            Write(GetValue(modes, 0) + GetValue(modes, 1), GetValue(modes, 2, true));
            _idx += 4;
            return true;
        }

        private bool Multiply(List<int> modes)
        {
            Debug("Mult", 4, modes, true);
            Write(GetValue(modes, 0) * GetValue(modes, 1), GetValue(modes, 2, true));
            _idx += 4;
            return true;
        }

        // Input return false and pause the execution if there is no more input to process when needed.
        private bool Input(List<int> modes)
        {
            Debug("Input", 2, modes, true);
            if (_inpIdx >= _inputs.Length)
            {
                return false;
            }
            Write(_inputs[_inpIdx++], GetValue(modes, 0, true));
            _idx += 2;
            return true;
        }

        private bool Output(List<int> modes)
        {
            Debug("Output", 2, modes);
            _outputs.Add(GetValue(modes, 0));
            _idx += 2;
            return true;
        }

        private bool JumpIfTrue(List<int> modes)
        {
            Debug("JumpIfTrue", 3, modes);
            _idx = (GetValue(modes, 0) != 0 ? GetValue(modes, 1) : _idx + 3);
            return true;
        }
        private bool JumpIfFalse(List<int> modes)
        {
            Debug("JumpIfFalse", 3, modes);
            _idx = (GetValue(modes, 0) == 0 ? GetValue(modes, 1) : _idx + 3);
            return true;
        }
        private bool LessThan(List<int> modes)
        {
            Debug("LessThan", 4, modes, true);
            Write((GetValue(modes, 0) < GetValue(modes, 1) ? 1 : 0), GetValue(modes, 2, true));
            _idx += 4;
            return true;
        }
        private bool Equals(List<int> modes)
        {
            Debug("Equals", 4, modes, true);
            Write((GetValue(modes, 0) == GetValue(modes, 1) ? 1 : 0), GetValue(modes, 2, true));
            _idx += 4;
            return true;
        }

        private bool SetRelativeBase(List<int> modes)
        {
            Debug("SetRelativeBase", 2, modes);
            _relativeBase += GetValue(modes, 0);
            _idx += 2;
            return true;
        }

        private bool Halt(List<int> modes)
        {
            Debug("Halt", 1, modes);
            _running = false;
            return false;
        }
        #endregion

        /// <summary>
        /// The parameter modes take a value as parameter and return a value based on the mode.
        /// </summary>
        /// <param name="value">Value in the opcodes memory</param>
        /// <param name="write">Is the value used to write in memory</param>
        /// <returns>Processed value</returns>
        #region Parameter modes
        private BigInteger PositionMode(BigInteger value, bool write)
        {
            return (write ? value : Read(value));
        }

        private BigInteger ImmediateMode(BigInteger value, bool write)
        {
            return value;
        }

        private BigInteger RelativeMode(BigInteger value, bool write)
        {
            return (write ? _relativeBase + value : Read(_relativeBase + value));
        }
        #endregion


        /// <summary>
        /// Used to debug an instruction if the program is in debug mode.
        /// </summary>
        /// <param name="name">Name of the instruction.</param>
        /// <param name="count">Number of parameter, instruction comprised.</param>
        /// <param name="modes">List of modes for this instruction.</param>
        /// <param name="write">Does the instruction being debbuged write in memory.</param>
        private void Debug(string name, int count, List<int> modes, bool write = false)
        {
            if (DebugMode)
            {
                string msg = name + ":";
                for (int i = 0; i < count; ++i)
                {
                    msg += " " + Read(_idx + i);
                    if (i > 0)
                    {
                        msg += "(" + GetValue(modes, i - 1, (write && i == count - 1)) + ")";
                    }
                }
                Console.WriteLine(msg);
            }
        }
    }
}
