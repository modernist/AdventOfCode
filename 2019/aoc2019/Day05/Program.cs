using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Day05
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse);

            Console.WriteLine(Part1(input.ToArray()));

            Console.Read();
        }

        static int Part1(int[] program)
        {
            var output = new List<int>();
            var computer = new IntcodeComputer(program, 0, () => 1, (o) => output.Add(o));
            computer.Run();
            return output.Last();
        }
    }

    public class IntcodeComputer
    {
        public int[] Program { get; }

        public int PC { get; set; }

        public Func<int> InputFunc { get; set; }

        public Action<int> OutputFunc { get; set; }

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

        enum OpCode
        {
            Add = 1,
            Multiply = 2,
            Input = 3,
            Output = 4,
            Terminate = 99
        }

        private Dictionary<OpCode, Action> _opCodes = new Dictionary<OpCode, Action>();

        public IntcodeComputer(int[] program, int pc = 0, Func<int> inputFunc = null, Action<int> outputFunc = null)
        {
            Program = program;
            PC = pc;
            InputFunc = inputFunc;
            OutputFunc = outputFunc;
            InitOpCodes();
        }

        private void InitOpCodes()
        {
            _opCodes.Add(OpCode.Add, () => { Value(3) = Value(1) + Value(2); PC += 4; });
            _opCodes.Add(OpCode.Multiply, () => { Value(3) = Value(1) * Value(2); PC += 4; });
            _opCodes.Add(OpCode.Input, () => { Value(1) = InputFunc(); PC += 2; });
            _opCodes.Add(OpCode.Output, () => { OutputFunc(Value(1)); PC += 2;});
            _opCodes.Add(OpCode.Terminate, () => { });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int Value(int index)
        {
            var m = Program[PC] / (int) Math.Pow(10, index + 1) % 10;
            return ref (m == 1)
                ? ref Program[PC + index]
                : ref Program[Program[PC + index]];
        }
    }
}
