using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day25
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt")
                .Select(line => line.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(numbers => new Point()
                {
                    X = int.Parse(numbers[0]),
                    Y = int.Parse(numbers[1]),
                    Z = int.Parse(numbers[2]),
                    T = int.Parse(numbers[3])
                });

            Console.WriteLine(Part1(input));
        }

        static int Part1(IEnumerable<Point> input)
        {
            var constellations = input.Select(point => new HashSet<Point>(new[] {point})).ToList();

            foreach (var set in constellations.ToList())
            {
                var pt = set.First();
                var closeSets = new List<HashSet<Point>>();
                foreach (var otherSet in constellations)
                {
                    foreach (var other in otherSet)
                    {
                        if (pt.InRange(other))
                        {
                            closeSets.Add(otherSet);
                        }
                    }
                }
                var mergedSet = new HashSet<Point>();
                foreach (var closeSet in closeSets)
                {
                    foreach (var other in closeSet)
                    {
                        mergedSet.Add(other);
                    }
                    constellations.Remove(closeSet);
                }
                constellations.Add(mergedSet);
            }

            //foreach (var c in constellations)
            //{
            //    Console.WriteLine(c.Count);
            //}
            //Console.WriteLine($"1=>{constellations.Count(c=>c.Count == 1)}");

            return constellations.Count;
        }

        class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }
            public int T { get; set; }

            public bool InRange(Point other)
            {
                var sum = Math.Abs(X - other.X);
                if (sum > 3)
                    return false;
                sum += Math.Abs(Y - other.Y);
                if (sum > 3)
                    return false;
                sum += Math.Abs(Z - other.Z);
                if (sum > 3)
                    return false;
                sum += Math.Abs(T - other.T);
                return (sum <= 3);
            }
        }
    }
}
