using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day06
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt")
                .Select(l => (Inner: l.Substring(0, 3), Outer: l.Substring(4)))
                .ToDictionary(orbit => orbit.Outer, orbit => orbit.Inner); ;

            Console.WriteLine(Part1(input));

            Console.Read();
        }

        static int Part1(IDictionary<string, string> orbits)
        {
            return orbits.Keys.Select(item => TraverseOrbits(orbits, item).Count()).Sum();
        }

        static IEnumerable<string> TraverseOrbits(IDictionary<string, string> orbits, string item)
        {
            for (var inner = orbits[item]; inner != null; orbits.TryGetValue(inner, out inner))
            {
                yield return inner;
            }
        }
    }
}
