using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day19
{
    class Program
    {
        static readonly Regex instructionRegex = new Regex(@"^(?<op>\w+)\s(?<A>\d+)\s(?<B>\d+)\s(?<C>\d+)", RegexOptions.Compiled);

        static readonly Dictionary<string, Func<int[], int[], int[]>> opCodes;

        static Program()
        {
            opCodes = new Dictionary<string, Func<int[], int[], int[]>>();
            opCodes.Add("addr", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] + res[instr[1]]; return res; });
            opCodes.Add("addi", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] + instr[1]; return res; });
            opCodes.Add("mulr", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] * res[instr[1]]; return res; });
            opCodes.Add("muli", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] * instr[1]; return res; });
            opCodes.Add("banr", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] & res[instr[1]]; return res; });
            opCodes.Add("bani", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] & instr[1]; return res; });
            opCodes.Add("borr", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] | res[instr[1]]; return res; });
            opCodes.Add("bori", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] | instr[1]; return res; });
            opCodes.Add("setr", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]]; return res; });
            opCodes.Add("seti", (state, instr) => { var res = state.ToArray(); res[instr[2]] = instr[0]; return res; });
            opCodes.Add("gtir", (state, instr) => { var res = state.ToArray(); res[instr[2]] = instr[0] > res[instr[1]] ? 1 : 0; return res; });
            opCodes.Add("gtri", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] > instr[1] ? 1 : 0; return res; });
            opCodes.Add("gtrr", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] > res[instr[1]] ? 1 : 0; return res; });
            opCodes.Add("eqir", (state, instr) => { var res = state.ToArray(); res[instr[2]] = instr[0] == res[instr[1]] ? 1 : 0; return res; });
            opCodes.Add("eqri", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] == instr[1] ? 1 : 0; return res; });
            opCodes.Add("eqrr", (state, instr) => { var res = state.ToArray(); res[instr[2]] = res[instr[0]] == res[instr[1]] ? 1 : 0; return res; });
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var ipIndex = int.Parse(input[0].Substring(3));
            var instructions = input.Skip(1).Select(line => instructionRegex.Match(line)).Select(match =>
            {
                return new Instruction()
                {
                    Op = match.Groups["op"].Value,
                    Parameters = (new[] {match.Groups["A"].Value, match.Groups["B"].Value, match.Groups["C"].Value})
                        .Select(i => int.Parse(i)).ToArray()
                };
            }).ToArray();

            Console.WriteLine(Part1(ipIndex, instructions));
            Console.WriteLine(Part2BruteForce(ipIndex, instructions));
            Console.WriteLine(Part2());
            Console.Read();
        }

        static int Part1(int ipIndex, Instruction[] instructions)
        {
            var programSize = instructions.Length;
            var state = new ExecutionState()
            {
                IPIndex = ipIndex,
                Registers = new int[6]
            };

            while (state.IP >= 0 && state.IP < programSize)
            {
                state.Execute(instructions[state.IP]);
            }

            return state.Registers[0];
        }

        static int Part2BruteForce(int ipIndex, Instruction[] instructions)
        {
            var programSize = instructions.Length;
            var state = new ExecutionState()
            {
                IPIndex = ipIndex,
                Registers = new int[6]
            };

            state.Registers[0] = 1;
            //int limit = 0;
            while (state.IP >= 0 && state.IP < programSize /*&& limit++ < 1000*/)
            {
                state.Execute(instructions[state.IP]);
                //Console.WriteLine(state);
            }

            return state.Registers[0];
        }

        static int Part2()
        {
            //find the sum of factors
            var input = 10551306;
            var result = 0;
            for (var i = 1; i <= input; i++)
            {
                if (input % i == 0)
                {
                    result += i;
                }
            }

            return result;
        }

        class ExecutionState
        {
            public int IP { get; set; }

            public int IPIndex { get; set; }

            public int[] Registers { get; set; }

            public void Execute(Instruction instruction)
            {
                Registers[IPIndex] = IP;
                Registers = opCodes[instruction.Op](Registers, instruction.Parameters);
                IP = Registers[IPIndex];
                IP++;
            }

            public override string ToString()
            {
                return $"IP: {IP}, Registers:[{string.Join(" ", Registers)}]";
            }
        }

        class Instruction
        {
            public string Op { get; set; }

            public int[] Parameters { get; set; }
        }

    }
}
