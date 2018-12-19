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
            });


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
                state = state.Execute(instructions[state.IP]);
            }

            return state.Registers[0];
        }

        class ExecutionState
        {
            public int IP { get; set; }

            public int IPIndex { get; set; }

            public int[] Registers { get; set; }

            public ExecutionState Execute(Instruction instruction)
            {
                return this;
            }
        }

        class Instruction
        {
            public string Op { get; set; }

            public int[] Parameters { get; set; }
        }

    }
}
