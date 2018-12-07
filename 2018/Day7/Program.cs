using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day7
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var edges = input.Select(line => line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(tokens => (Before: tokens[1], After: tokens[7]));
            var vertices = edges.SelectMany(edge => new[] {edge.Before, edge.After}).ToHashSet();

            var part1 = string.Join("", Part1(vertices, edges.ToHashSet()));
            Console.WriteLine(part1);

        }

        static IEnumerable<string> Part1(HashSet<string> vertices, HashSet<(string Before, string After)> edges)
        {
            var sorted = new List<string>();
            //find all vertices with no incoming edges
            var eligible = new SortedSet<string>(vertices.Where(vertex => edges.All(edge => edge.After != vertex)));
            while (eligible.Any())
            {
                var vertex = eligible.First();
                eligible.Remove(vertex);
                sorted.Add(vertex);

                foreach (var edge in edges.Where(e => e.Before == vertex).ToList())
                {
                    var dependent = edge.After;
                    edges.Remove(edge);

                    //if dependent vertex has no other incoming edges
                    if (edges.All(e => e.After != dependent))
                    {
                        eligible.Add(dependent);
                    }
                }
            }

            if (edges.Any())
            {
                // graph contains cycles
                return new [] { "cycle detected"};
            }

            return sorted;
        }

    }
}
