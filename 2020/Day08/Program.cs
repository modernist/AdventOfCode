using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day08
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt")
                .Select(line => (op: line[..3], arg: int.Parse(line[4..])))
                .ToArray();

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1((string op, int arg)[] input)
        {
            var (acc, completed) = Run(input);
            return acc;
        }

        static int Part2((string op, int arg)[] input)
        {
            var alternatives = Enumerable.Range(0, input.Length)
                .Where(pc => input[pc].op != "acc")
                .Select(pc =>
                    new[] {input.Take(pc), new[] {ToggleInstruction(input[pc])}, input.Skip(pc + 1)}
                        .SelectMany(instruction => instruction).ToArray());

            var result = alternatives
                .Select(program => Run(program))
                .First(run => run.completed);

            return result.accumulator;
        }

        static (string op, int arg) ToggleInstruction((string op, int arg) instruction)
        {
            var newOp = instruction.op switch
            {
                "nop" => "jmp",
                "jmp" => "nop",
                _ => instruction.op
            };

            return (newOp, instruction.arg);
        }

        static (int accumulator, bool completed) Run((string op, int arg)[] program)
        {
            var accumulator = 0;
            var pc = 0;
            var executed = new HashSet<int>();

            do
            {
                if (executed.Add(pc))
                {
                    var (op, arg) = program[pc];

                    switch (op)
                    {
                        case "acc":
                            pc++;
                            accumulator += arg;
                            break;

                        case "jmp":
                            pc += arg;
                            break;

                        case "nop":
                            pc++;
                            break;

                        default:
                            throw new ArgumentException("invalid op");
                    }
                }
                else
                {
                    return (accumulator, false);
                }
            } while (pc < program.Length);

            return (accumulator, true);
        }
    }
}
