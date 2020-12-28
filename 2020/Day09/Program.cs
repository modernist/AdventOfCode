using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day09
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Select(long.Parse).ToArray();

            var part1 = Part1(input);
            Console.WriteLine(part1);
            Console.WriteLine(Part2(input, part1));
        }

        private static long Part1(long[] input)
        {
            var pairs = GeneratePairs(25).ToArray();

            var position = Enumerable.Range(25, input.Length - 25).First(offset =>
                pairs.All(pair => input[offset] != input[offset - pair.first - 1] + input[offset - pair.second - 1]));

            return input[position];
        }

        private static IEnumerable<(int first, int second)> GeneratePairs(int length)
        {
            return Enumerable.Range(0, length - 1)
                .SelectMany(i => Enumerable.Range(i + 1, 24 - i).Select(j => (i, j)));
        }

        private static long Part2(long[] input, long target)
        {
            foreach (var n in Enumerable.Range(0, input.Length))
            {
                var sum = input[n];
                foreach (var k in Enumerable.Range(n + 1, input.Length - n))
                {
                    sum += input[k];
                    if (sum > target)
                    {
                        break;
                    }

                    if (sum == target)
                    {
                        var slice = input[n..(k+1)];
                        return slice.Min() + slice.Max();
                    }
                }
            }
            
            return 0L;
        }
    }
}
