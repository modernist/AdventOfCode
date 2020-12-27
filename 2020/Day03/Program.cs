using System;
using System.IO;
using System.Linq;

namespace Day03
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(string[] input)
        {
            return CountTrees(input, (3, 1));
        }

        static long Part2(string[] input)
        {
            var slopes = new[]
            {
                (1, 1),
                (3, 1),
                (5, 1),
                (7, 1),
                (1, 2)
            };

            return slopes.Aggregate(1L, (product, slope) => product * CountTrees(input, slope));
        }

        static int CountTrees(string[] input, (int right, int down) slope)
        {
            var rows = input.Length;
            var columns = input[0].Length;
            var trees = 0;
            var position = (row: 0, column: 0);
            var (right, down) = slope;
            while (true)
            {
                position = (position.row + down, (position.column + right) % columns);
                if (position.row >= rows)
                {
                    break;
                }

                if (input[position.row][position.column] == '#')
                {
                    trees++;
                }
            }

            return trees;
        }
    }
}
