using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var processedInput = input.Select(c => (int) c).ToArray();

            var part1 = Part1(processedInput);
            Console.WriteLine(part1);

            var part2 = Part2(processedInput);
            Console.WriteLine(part2);
        }

        private static int Part1(IEnumerable<int> input)
        {
            return Collapse(input);
        }

        private static int Part2(IEnumerable<int> input)
        {
            int A = 'A';
            var distinctUnits = Enumerable.Range(0, 26).Select(i => new[] {A + i, A + i + 32});
            return distinctUnits
                .Select(unit => Collapse(input.Where(i => !unit.Contains(i))))
                .Min();
        }

        private static int Collapse(IEnumerable<int> input)
        {
            var dirty = new Stack<int>(input);
            var clean = new Stack<int>();
            clean.Push(dirty.Pop());

            while (dirty.TryPop(out var left))
            {
                if (!clean.TryPop(out var right))
                {
                    clean.Push(left);
                }
                else
                {
                    if (!Reacts(left, right))
                    {
                        clean.Push(right);
                        clean.Push(left);
                    }
                }
            }

            return clean.Count;
        }

        private static bool Reacts(int left, int right) => Math.Abs(left - right) == 32;
    }
}
