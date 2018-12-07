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
                .Select(tokens => (Before: tokens[1][0], After: tokens[7][0]));
            var vertices = edges.SelectMany(edge => new[] {edge.Before, edge.After}).ToHashSet();

            var part1 = new string(Part1(vertices, edges.ToHashSet()).ToArray());
            Console.WriteLine(part1);
            var part2 = Part2(vertices, edges, 5);
            Console.WriteLine(part2);
        }

        static IEnumerable<char> Part1(IEnumerable<char> vertices, HashSet<(char Before, char After)> edges)
        {
            var sorted = new List<char>();
            //find all vertices with no incoming edges
            var eligible = new SortedSet<char>(vertices.Where(vertex => edges.All(edge => edge.After != vertex)));
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
                return "cycle detected";
            }

            return sorted;
        }

        static int Part2(IEnumerable<char> vertices, IEnumerable<(char Before, char After)> edges, int workerCount)
        {
            var time = 0;
            var workers = new int[workerCount];
            var tasks = new SortedSet<char>(vertices);
            var done = new List<(char Task, int Finish)>();
            var dependencies = edges.ToList();

            while (tasks.Any() || workers.Any(w => w > time))
            {
                foreach (var d in done.Where(task => task.Finish <= time))
                {
                    dependencies.RemoveAll(dependency => dependency.Before == d.Task);
                }

                done.RemoveAll(task => task.Finish <= time);
                // find all tasks that can be started now (no dependencies)
                var eligible = tasks.Where(task => dependencies.All(d => d.After != task)).ToList();

                for (var worker = 0; worker < workerCount && eligible.Any(); worker++)
                {
                    if (workers[worker] <= time) //worker is idle
                    {
                        var task = eligible.First();
                        var finish = Cost(task) + time;
                        workers[worker] = finish;
                        tasks.Remove(task);
                        done.Add((task, finish));
                        eligible.Remove(task);
                    }
                }

                time++;
            }

            return time;
        }

        static int Cost(char step) => step - 4; // 'A' = 65, Cost('A') = 61
    }
}
