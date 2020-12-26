using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Day09
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse);

            Console.WriteLine(Part1(input.ToArray()));

            Console.WriteLine(Part2(input.ToArray()));

            Console.Read();
        }

        static long Part1(long[] program)
        {
            var output = new List<long>();

            var boost = new IntcodeComputer(program, 0, 0, () => new [] { 1L }, (o) => output.Add(o));
            boost.Run();

            return output.Single();
        }

        static long Part2(long[] program)
        {
            var output = new List<long>();

            var boost = new IntcodeComputer(program, 0, 0, () => new[] { 2L }, (o) => output.Add(o));
            boost.Run();

            return output.Single();
        }
    }

    public class IntcodeComputer
    {
        public long[] Program { get; set; }

        public int PC { get; set; }

        public int RelativeBase { get; set; }

        public Func<IEnumerable<long>> InputFunc { get; set; }

        public Action<long> OutputFunc { get; set; }

        public void Run()
        {
            while (true)
            {
                var op = (OpCode) (Program[PC] % 100);
                if (op == OpCode.Terminate)
                {
                    break;
                }

                _opCodes[op]();
            }
        }

        public bool Step()
        {
            var op = (OpCode) (Program[PC] % 100);
            if (op == OpCode.Terminate)
            {
                return false;
            }

            _opCodes[op]();
            return true;
        }

        enum OpCode
        {
            Add = 1,
            Multiply = 2,
            Input = 3,
            Output = 4,
            JumpNonZero = 5,
            JumpZero = 6,
            LessThan = 7,
            Equals = 8,
            SetRelativeBase = 9,
            Terminate = 99
        }

        private Dictionary<OpCode, Action> _opCodes = new Dictionary<OpCode, Action>();
        private IEnumerator<long> _inputEnumerator;
        private bool _inputAvailable = true;

        public IntcodeComputer(long[] program, int pc = 0, int rb = 0, Func<IEnumerable<long>> inputFunc = null,
            Action<long> outputFunc = null)
        {
            Program = new long[1048576];
            Array.Copy(program, Program, program.Length);
            PC = pc;
            RelativeBase = rb;
            InputFunc = inputFunc;
            OutputFunc = outputFunc;
            _inputEnumerator = inputFunc().GetEnumerator();
            InitOpCodes();
        }

        private long NextInput()
        {
            _inputAvailable = _inputEnumerator.MoveNext();
            return _inputEnumerator.Current;
        }

        private void InitOpCodes()
        {
            _opCodes.Add(OpCode.Add, () => { Value(3) = Value(1) + Value(2); PC += 4; });
            _opCodes.Add(OpCode.Multiply, () => { Value(3) = Value(1) * Value(2); PC += 4; });
            _opCodes.Add(OpCode.Input, () => {
                if (_inputAvailable)
                {
                    Value(1) = NextInput();
                    PC += 2;
                }
            });
            _opCodes.Add(OpCode.Output, () => { OutputFunc(Value(1)); PC += 2; });
            _opCodes.Add(OpCode.JumpNonZero, () => { PC = (Value(1) != 0) ? (int)Value(2) : PC + 3; });
            _opCodes.Add(OpCode.JumpZero, () => { PC = (Value(1) == 0) ? (int)Value(2) : PC + 3; });
            _opCodes.Add(OpCode.LessThan, () => { Value(3) = (Value(1) < Value(2)) ? 1 : 0; PC += 4; });
            _opCodes.Add(OpCode.Equals, () => { Value(3) = (Value(1) == Value(2)) ? 1 : 0; PC += 4; });
            _opCodes.Add(OpCode.SetRelativeBase, () => { RelativeBase += (int)Value(1); PC += 2; });
            _opCodes.Add(OpCode.Terminate, () => { });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref long Value(int index)
        {
            var mode = Program[PC] / (int) Math.Pow(10, index + 1) % 10;

            switch (mode)
            {
                case 0:
                    return ref Program[Program[PC + index]];

                case 1:
                    return ref Program[PC + index];

                case 2:
                    return ref Program[RelativeBase + Program[PC + index]];

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
