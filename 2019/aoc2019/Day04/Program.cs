using System;
using System.Linq;

namespace Day04
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = 264793;
            var end = 803935;

            Console.WriteLine(Part1(start, end));

            Console.Read();
        }

        static int Part1(int min, int max)
        {
            var numbers = Enumerable.Range(min, max - min + 1)
                .Select(n => n.ToString())
                .Where(ns =>
                {
                    var nsa = ns.ToArray();
                    Array.Sort(nsa);

                    return (new string(nsa) == ns) && nsa.Distinct().Count() <= 5;
                });


            return numbers.Count();
        }
    }
}
