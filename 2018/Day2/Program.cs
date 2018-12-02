using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        private static int Part1(IEnumerable<string> lines)
        {
            var ids = lines.Select(line => line.GroupBy(c => c).ToDictionary(cg => cg.Key, cg => cg.Count()))
                .Select(groups => new { Pairs = groups.Any(g => g.Value == 2), Triples = groups.Any(g => g.Value == 3) });

            return ids.Count(id => id.Pairs) * ids.Count(id => id.Triples);
        }

        private static string Part2(IEnumerable<string> lines)
        {
            var pairs = lines.SelectMany(line => lines.Except(new[] {line}).Select(other => (Left: line, Right: other)));

            foreach (var pair in pairs)
            {
                if (pair.Left.Zip(pair.Right, (l, r) => l != r).Count(different => different) == 1)
                {
                    return string.Join("", pair.Left.Zip(pair.Right, (l, r) => (l == r) ? l.ToString() : null));
                }
            }

            return string.Empty;
        }
    }
}
