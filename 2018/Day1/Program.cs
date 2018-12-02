using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt").Select(line => int.Parse(line)).ToArray();
            Console.WriteLine(Part1(lines));
            Console.WriteLine(Part2(lines));
        }

        static int Part1(int[] lines)
        {
            return lines.Sum();
        }

        static int Part2(int[] lines)
        {
            var distinctSums = new HashSet<int>() { 0 };
            var runningSum = 0;
            while (lines
                .Select(v => distinctSums.Add(runningSum += v))
                .All(v => v)) ;
            return runningSum;
        }
    }
}
