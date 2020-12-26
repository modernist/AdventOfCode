using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day02
{
    class Program
    {
        private static readonly Regex _parser = new Regex("^(?<min>\\d+)-(?<max>\\d+) (?<c>\\w): (?<password>\\w+)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt")
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(Parse);

            Console.WriteLine(Part1(input));

            Console.WriteLine(Part2(input));
        }

        static (int min, int max, char c, string password) Parse(string input)
        {
            var result = _parser.Match(input);

            return (int.Parse(result.Groups["min"].Value), int.Parse(result.Groups["max"].Value),
                result.Groups["c"].Value[0], result.Groups["password"].Value);
        }

        public static int Part1(IEnumerable<(int min, int max, char c, string password)> input)
        {
            return input.Count(line =>
                Enumerable.Range(line.min, line.max - line.min + 1).Contains(line.password.Count(c => c == line.c)));
        }

        public static int Part2(IEnumerable<(int min, int max, char c, string password)> input)
        {
            return input.Count(line =>
                (new[] {line.min, line.max}).Count(pos => line.password[pos - 1] == line.c) == 1);
        }
    }
}
