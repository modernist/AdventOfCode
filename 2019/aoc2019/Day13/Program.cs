using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse);

            Console.WriteLine(Part1(input.ToArray()));

            //Console.WriteLine(Part2(input.ToArray()));
        }

        static int Part1(long[] program)
        {
            var cp = new CarePackage(program);
            cp.Run();
            return cp.Blocks;
        }
    }

    public class CarePackage
    {
        private IntcodeComputer _computer;
        private Dictionary<(long row, long col), long> _grid; // location -> type

        public CarePackage(long[] program)
        {
            _computer = new IntcodeComputer(program);
            _grid = new Dictionary<(long row, long col), long>();
        }

        public void Run()
        {
            _computer.Run();
            while (_computer.Output.Count > 0)
            {
                var col = _computer.Output.Take();
                var row = _computer.Output.Take();
                var type = _computer.Output.Take();
                _grid[(row, col)] = type;
            }
        }

        public int Blocks => _grid.Where(p => p.Value == 2).Count();
    }

    public class IntcodeComputer
    {
        public long[] Program { get; set; }

        public int PC { get; set; }

        public int RelativeBase { get; set; }

        public Func<long> InputFunc { get; set; }

        public Action<long> OutputFunc { get; set; }

        public BlockingCollection<long> Input { get; } = new BlockingCollection<long>(new ConcurrentQueue<long>());

        public BlockingCollection<long> Output { get; } = new BlockingCollection<long>(new ConcurrentQueue<long>());

        public bool Halted { get; private set; }

        public void Run()
        {
            Halted = false;
            while (true)
            {
                var op = (OpCode)(Program[PC] % 100);
                if (op == OpCode.Terminate)
                {
                    Halted = true;
                    break;
                }

                _opCodes[op]();
            }
        }

        public bool Step()
        {
            var op = (OpCode)(Program[PC] % 100);
            if (op == OpCode.Terminate)
            {
                Halted = true;
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

        public IntcodeComputer(long[] program, int pc = 0, int rb = 0, Func<long> inputFunc = null,
            Action<long> outputFunc = null)
        {
            Program = new long[1048576];
            Array.Copy(program, Program, program.Length);
            PC = pc;
            RelativeBase = rb;
            InputFunc = inputFunc ?? Input.Take;
            OutputFunc = outputFunc ?? Output.Add;
            InitOpCodes();
            Halted = false;
        }

        private void InitOpCodes()
        {
            _opCodes.Add(OpCode.Add, () => { Value(3) = Value(1) + Value(2); PC += 4; });
            _opCodes.Add(OpCode.Multiply, () => { Value(3) = Value(1) * Value(2); PC += 4; });
            _opCodes.Add(OpCode.Input, () => { Value(1) = InputFunc(); PC += 2; });
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
            var mode = Program[PC] / (int)Math.Pow(10, index + 1) % 10;

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
