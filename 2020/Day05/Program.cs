using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day05
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt")
                .Replace('F', '0')
                .Replace('B', '1')
                .Replace('L', '0')
                .Replace('R', '1')
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(seat => Convert.ToInt32(seat, 2))
                .ToHashSet();

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(HashSet<int> seats)
        {
            return seats.Max();
        }

        static int Part2(HashSet<int> seats)
        {
            return Enumerable.Range(seats.Min(), seats.Max()).First(seat => !seats.Contains(seat));
        }
    }
}
