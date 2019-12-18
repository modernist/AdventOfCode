using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Day11
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

            Console.Read();
        }

        static int Part1(long[] program)
        {
            return 0;
        }
    }

    public class HullPainter
    {
        private IntcodeComputer _computer;
        private Point _currentLocation = new Point(0, 0);
        private int _currentColor = 0; // 0: black, 1: white
        private char _currentDirection = '^';
        private Dictionary<Point, int> _grid = new Dictionary<Point, int>(); // location -> color

        private static Dictionary<char, (int xdiff, int ydiff)> BuildOffsets()
        {
            var offsets = new Dictionary<char, (int xdiff, int ydiff)>
            {
                {'<', (-1, 0)}, {'>', (1, 0)}, {'^', (0, 1)}, {'v', (0, -1)}
            };
            return offsets;
        }

        private static Dictionary<(char, int), char> BuildTurns()
        {
            // 0 = left, 1 = right
            var turns = new Dictionary<(char, int), char>
            {
                {('^', 0), '<'}, {('^', 1), '>'},
                {('v', 0), '>'}, {('v', 1), '<'},
                {('<', 0), 'v'}, {('<', 1), '^'},
                {('>', 0), '^'}, {('>', 1), 'v'}
            };

            return turns;
        }
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

        public void Run()
        {
            while (true)
            {
                var op = (OpCode)(Program[PC] % 100);
                if (op == OpCode.Terminate)
                {
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

    class Point : IEquatable<Point>
    {
        public int X { get; }

        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int ManhattanDistance(int x, int y)
        {
            return Math.Abs(X - x) + Math.Abs(Y - y);
        }

        public int ManhattanDistance(Point other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public bool Equals(Point other)
        {
            return other != null && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return X * 65535 + Y;
            }
        }
    }
}
