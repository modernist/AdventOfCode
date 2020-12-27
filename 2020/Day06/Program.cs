using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day06
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt")
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(IEnumerable<string> input)
        {
            var groups = input.Select(group => group.Replace("\n", "").ToHashSet());

            return groups.Sum(group => group.Count);
        }

        static int Part2(IEnumerable<string> input)
        {
            return input.Select(group =>
                    group.Split("\n", StringSplitOptions.RemoveEmptyEntries)
                        .Select(answers => answers.ToCharArray())
                )
                .Select(group => group.Aggregate((a, b) => a.Intersect(b).ToArray()))
                .Sum(group => group.Length);
        }
    }
}
