using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Day07
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse);

            Console.WriteLine(Part1(input.ToArray()));

            Console.WriteLine(Part2(input.ToArray()));

            Console.Read();
        }

        static int Part1(int[] program)
        {
            var input = new[] { 0, 1, 2, 3, 4 };
            int max = 0;

            foreach (var sequence in Permute(input))
            {
                var output = new[] { 0, 0, 0, 0, 0 };

                IEnumerable<int> InputFactory(int index)
                {
                    yield return sequence[index];
                    yield return output[(index == 0) ? 0 : index - 1];
                }

                void Output(int index, int value)
                {
                    output[index] = value;
                }

                for (var i = 0; i < 5; i++)
                {
                    var stage = new IntcodeComputer((int[])program.Clone(), 0, () => InputFactory(i), (o) => Output(i, o));
                    stage.Run();
                }

                var result = output[4];
                if (result > max)
                {
                    max = result;
                }
            }

            return max;
        }

        static int Part2(int[] program)
        {
            var phases = new[] { 5, 6, 7, 8, 9 };
            var pipes = Enumerable.Range(0, 5).Select(i => new BlockingCollection<int>(new ConcurrentQueue<int>())).ToArray(); // connect a stage's output to the input of the next one
            var max = 0;

            IEnumerable<int> Input(int index)
            {
                while (true)
                    yield return pipes[(index == 0) ? 4 : index - 1].Take();
            }

            void Output(int index, int value)
            {
                pipes[index].Add(value);
            }

            var stages = Enumerable.Range(0, 5).Select(i => new IntcodeComputer((int[])program.Clone(), 0, () => Input(i), (o) => Output(i, o))).ToArray();

            foreach (var sequence in Permute(phases))
            {
                for (var i = 0; i < sequence.Length; i++)
                {
                    stages[i].Program = (int[]) program.Clone();
                    stages[i].PC = 0;
                    while(pipes[(i + sequence.Length - 1) % sequence.Length].TryTake(out _)); // clear queue
                    pipes[(i + sequence.Length - 1) % sequence.Length].Add(sequence[i]); // feed the phase to each amp
                }

                pipes[sequence.Length - 1].Add(0); // initial input for first amp

                //var stepsRun = true;
                //do
                //{
                //    stepsRun = false;
                //    foreach (var stage in stages)
                //    {
                //        stepsRun |= stage.Step();
                //    }
                //} while (stepsRun);

                Parallel.ForEach(stages, (stage, _, l) =>
                {
                    stage.Run();
                });

                var result = pipes[4].Single();
                if (result > max)
                {
                    max = result;
                }
            }

            return max;
        }

        private static IEnumerable<int[]> Permute(int[] input)
        {
            if (input.Length > 1)
            {
                int n = input[0];
                foreach (int[] subPermute in Permute(input.Skip(1).ToArray()))
                {
                    for (int index = 0; index <= subPermute.Length; index++)
                    {
                        int[] pre = subPermute.Take(index).ToArray();
                        int[] post = subPermute.Skip(index).ToArray();

                        if (post.Contains(n))
                            continue;

                        yield return pre.Concat(new[] { n }).Concat(post).ToArray();
                    }
                }
            }
            else
            {
                yield return input;
            }
        }
    }

    public class IntcodeComputer
    {
        public int[] Program { get; set; }

        public int PC { get; set; }

        public Func<IEnumerable<int>> InputFunc { get; set; }

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
            Terminate = 99
        }

        private Dictionary<OpCode, Action> _opCodes = new Dictionary<OpCode, Action>();
        private IEnumerator<int> _inputEnumerator;
        private bool _inputAvailable = true;

        public IntcodeComputer(int[] program, int pc = 0, Func<IEnumerable<int>> inputFunc = null, Action<int> outputFunc = null)
        {
            Program = program;
            PC = pc;
            InputFunc = inputFunc;
            OutputFunc = outputFunc;
            _inputEnumerator = inputFunc().GetEnumerator();
            InitOpCodes();
        }

        private int NextInput()
        {
            _inputAvailable = _inputEnumerator.MoveNext();
            return _inputEnumerator.Current;
        }

        private void InitOpCodes()
        {
            _opCodes.Add(OpCode.Add, () => { Value(3) = Value(1) + Value(2); PC += 4; });
            _opCodes.Add(OpCode.Multiply, () => { Value(3) = Value(1) * Value(2); PC += 4; });
            _opCodes.Add(OpCode.Input, () => { if(_inputAvailable) { Value(1) = NextInput(); PC += 2; }});
            _opCodes.Add(OpCode.Output, () => { OutputFunc(Value(1)); PC += 2; });
            _opCodes.Add(OpCode.JumpNonZero, () => { PC = (Value(1) != 0) ? Value(2) : PC + 3; });
            _opCodes.Add(OpCode.JumpZero, () => { PC = (Value(1) == 0) ? Value(2) : PC + 3; });
            _opCodes.Add(OpCode.LessThan, () => { Value(3) = (Value(1) < Value(2)) ? 1 : 0; PC += 4; });
            _opCodes.Add(OpCode.Equals, () => { Value(3) = (Value(1) == Value(2)) ? 1 : 0; PC += 4; });
            _opCodes.Add(OpCode.Terminate, () => { });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int Value(int index)
        {
            var m = Program[PC] / (int)Math.Pow(10, index + 1) % 10;
            return ref (m == 1)
                ? ref Program[PC + index]
                : ref Program[Program[PC + index]];
        }
    }
}
