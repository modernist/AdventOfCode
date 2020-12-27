using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day07
{
    class Program
    {
        private static Regex _parser = new Regex("(?<count>\\d+) (?<bag>\\w+ \\w+) bag", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Select(Parse);

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static (string bag, List<(int count, string bag)> contents) Parse(string line)
        {
            var length = line.IndexOf(" bags", StringComparison.InvariantCulture);
            var bag = line.Substring(0, length);

            var contents = _parser.Matches(line, length)
                .Select(match => (count: int.Parse(match.Groups["count"].Value), bag: match.Groups["bag"].Value))
                .ToList();

            return (bag, contents);
        }

        private static int Part1(IEnumerable<(string bag, List<(int count, string bag)> contents)> input)
        {
            var parents = new Dictionary<string, HashSet<string>>();
            foreach (var (container, contents) in input)
            {
                foreach (var (count, bag) in contents)
                {
                    if (!parents.ContainsKey(bag))
                    {
                        parents.Add(bag, new HashSet<string>());
                    }

                    parents[bag].Add(container);
                }
            }

            return Paths(parents, "shiny gold").ToHashSet().Count - 1;
        }

        private static IEnumerable<string> Paths(Dictionary<string, HashSet<string>> parents, string bag)
        {
            yield return bag;

            if (!parents.ContainsKey(bag))
            {
                yield break;
            }

            foreach (var container in parents[bag])
            {
                foreach (var bagContainer in Paths(parents, container))
                {
                    yield return bagContainer;
                }
            }
        }

        private static long Part2(IEnumerable<(string bag, List<(int count, string bag)> contents)> input)
        {
            var children = input.ToDictionary(line => line.bag, line => line.contents);

            return CountRecursive(children, "shiny gold") - 1;
        }

        private static long CountRecursive(Dictionary<string, List<(int count, string bag)>> children, string bag)
        {
            return children[bag].Sum(child => child.count * CountRecursive(children, child.bag)) + 1;
        }
    }
}
