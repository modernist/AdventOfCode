using System;
using System.Collections.Generic;
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

            Console.WriteLine(Part2(input));

            Console.Read();
        }

        static int Part1(int[] modules)
        {
            return modules.Aggregate(0, (total, current) => total + ModuleFuel(current).First());
        }

        static int Part2(int[] modules)
        {
            return modules.Aggregate(0, (total, current) => total + ModuleFuel(current).Sum());
        }

        static IEnumerable<int> ModuleFuel(int mass)
        {
            var fuel = (int)Math.Floor(mass / 3.0) - 2;
            while(fuel > 0)
            {
                yield return fuel;
                fuel = (int)Math.Floor(fuel / 3.0) - 2;
            }
        }
    }
}
