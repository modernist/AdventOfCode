using System;
using System.IO;
using System.Linq;

namespace Day01
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Select(int.Parse).ToArray();

            Console.WriteLine(Part1(input));

            Console.Read();
        }

        static int Part1(int[] modules)
        {
            return modules.Aggregate(0, (total, current) => total + ModuleFuel(current));
        }

        static int ModuleFuel(int i)
        {
            return (int)Math.Floor(i / 3.0) - 2;
        }
    }
}
