using System;
using System.IO;
using System.Linq;

namespace Day01
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt")
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(int.Parse)
                .OrderBy(num => num)
                .ToArray();

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        public static int Part1(int[] input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                for (int j = i + 1; j < input.Length; j++)
                {
                    if (input[i] + input[j] == 2020)
                    {
                        return input[i] * input[j];
                    }
                }
            }

            return -1;
        }

        public static int Part2(int[] input)
        {
            var s = input.SelectMany(x =>
                    input.Select(y => (x: x, y: y, z: 2020 - x - y))
                        .Where(triplet => input.Contains(triplet.z))).First();

            return s.x * s.y * s.z;
        }
    }
}
