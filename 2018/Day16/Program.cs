using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day16
{
    class Program
    {
        static Regex instructionRegex = new Regex(@"^(?<op>\d+)\s(?<A>\d+)\s(?<B>\d+)\s(?<C>\d+)", RegexOptions.Compiled);
        static Regex registersRegex = new Regex(@"\[(?<A>\d+),\s(?<B>\d+),\s(?<C>\d+),\s(?<D>\d+)\]", RegexOptions.Compiled);
        private static Dictionary<string, Func<int[], int[], int[]>> opCodes;

        static Program()
        {
            opCodes = new Dictionary<string, Func<int[], int[], int[]>>();
            opCodes.Add("addr", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] + res[instr[2]]; return res; });
            opCodes.Add("addi", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] + instr[2]; return res; });
            opCodes.Add("mulr", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] * res[instr[2]]; return res; });
            opCodes.Add("muli", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] * instr[2]; return res; });
            opCodes.Add("banr", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] & res[instr[2]]; return res; });
            opCodes.Add("bani", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] & instr[2]; return res; });
            opCodes.Add("borr", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] | res[instr[2]]; return res; });
            opCodes.Add("bori", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] | instr[2]; return res; });
            opCodes.Add("setr", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]]; return res; });
            opCodes.Add("seti", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = instr[1]; return res; });
            opCodes.Add("gtir", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = instr[1] > res[instr[2]] ? 1 : 0; return res; });
            opCodes.Add("gtri", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] > instr[2] ? 1 : 0; return res; });
            opCodes.Add("gtrr", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] > res[instr[2]] ? 1 : 0; return res; });
            opCodes.Add("eqir", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = instr[1] == res[instr[2]] ? 1 : 0; return res; });
            opCodes.Add("eqri", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] == instr[2] ? 1 : 0; return res; });
            opCodes.Add("eqrr", (regs, instr) => { var res = regs.ToArray(); res[instr[3]] = res[instr[1]] == res[instr[2]] ? 1 : 0; return res; });
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var split = input.LastIndexOf(']') + 1;
            var input1 = input.Substring(0, split).Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var input2 = input.Substring(split).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine(Part1(input1));
        }

        static int Part1(string[] input)
        {
            var samples = ParseSamples(input);
            var sampleOps = samples.ToDictionary(s => s, s => 0);
            foreach (var sample in samples)
            {
                foreach (var op in opCodes)
                {
                    var result = op.Value(sample.Before, sample.Instruction);
                    if (result.SequenceEqual(sample.After))
                    {
                        sampleOps[sample]++;
                    }
                }
            }

            return sampleOps.Count(so => so.Value >= 3);
        }

        static List<Sample> ParseSamples(string[] input)
        {
            var samples = new List<Sample>();
            for (int i = 0; i < input.Length;)
            {
                var sample = new Sample();
                sample.Before = registersRegex.Match(input[i++]).Groups.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
                sample.Instruction = instructionRegex.Match(input[i++]).Groups.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
                sample.After = registersRegex.Match(input[i++]).Groups.Skip(1).Select(g => int.Parse(g.Value)).ToArray();
                samples.Add(sample);
            }

            return samples;
        }

        class Sample
        {
            public int[] Before { get; set; }
            public int[] Instruction { get; set; }
            public int[] After { get; set; }
        }
    }
}
