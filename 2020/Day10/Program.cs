using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Select(int.Parse).OrderBy(j => j).ToList();
            input.Insert(0, 0);
            input.Add(input.Last() + 3);

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(List<int> input)
        {
            var diffs = Enumerable.Range(1, input.Count - 1).Select(i => input[i] - input[i-1]).ToList();

            return diffs.Count(d => d == 1) * diffs.Count(d => d == 3);
        }

        static long Part2(List<int> input)
        {
            var paths = new Dictionary<int, long>();
            var current = input.Count - 1;
            paths[current] = 1;

            do
            {
                current--;
                var count = 0L;
                for (var next = 1; next <= 3 && current + next < input.Count; next++)
                {
                    var diff = input[current + next] - input[current];

                    if (diff >= 1 && diff <= 3)
                    {
                        count += paths[current + next];
                    }

                    paths[current] = count;
                }
            } while (current > 0);

            return paths[0];
        }
    }
}
