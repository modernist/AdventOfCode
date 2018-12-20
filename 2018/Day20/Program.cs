using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day20
{
    class Program
    {
        static Dictionary<char, (int dx, int dy)> directions;
        static Dictionary<(int x, int y), List<(int x, int y)>> adjacentPositions;

        static Program()
        {
            directions = new Dictionary<char, (int dx, int dy)>();
            directions.Add('N', (0,-1));
            directions.Add('E', (1, 0));
            directions.Add('S', (0, 1));
            directions.Add('W', (-1, 0));

            adjacentPositions = new Dictionary<(int x, int y), List<(int x, int y)>>();
        }

        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Skip(1).ToArray();

            var (part1, part2) = Solve(input);
            Console.WriteLine(part1);
            Console.WriteLine(part2);
        }

        static (int part1, int part2) Solve(char[] input)
        {
            int index = 0;
            Parse(input, ref index, (0, 0));

            var visited = new HashSet<(int x, int y)>();
            var queue = new Queue<((int x, int y), int distance)>();
            queue.Enqueue(((0, 0), 0));
            var maxDistance = 0;
            var longPaths = 0;
            while (queue.Any())
            {
                var (pos, distance) = queue.Dequeue();
                if (visited.Add(pos))
                {
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }

                    if (distance >= 1000)
                    {
                        longPaths++;
                    }

                    foreach (var adjacent in adjacentPositions[pos])
                    {
                        queue.Enqueue((adjacent, distance + 1));
                    }
                }
            }

            return (maxDistance, longPaths);
        }

        static (int x, int y) Step((int x, int y) from, (int x, int y) to)
        {
            if (!adjacentPositions.TryGetValue(from, out var adjacentFrom))
            {
                adjacentFrom = new List<(int x, int y)>();
                adjacentPositions.Add(from, adjacentFrom);
            }
            adjacentFrom.Add(to);
            if (!adjacentPositions.TryGetValue(to, out var adjacentTo))
            {
                adjacentTo = new List<(int x, int y)>();
                adjacentPositions.Add(to, adjacentTo);
            }
            adjacentTo.Add(from);
            return to;
        }

        static void Parse(char[] input, ref int index, (int x, int y) start)
        {
            var initial = (x: start.x, y: start.y);
            var current = (x: start.x, y: start.y);
            while (true)
            {
                var instruction = input[index++];
                switch (instruction)
                {
                    case 'N':
                    case 'E':
                    case 'S':
                    case 'W':
                        var (dx, dy) = directions[instruction];
                        var next = (current.x + dx, current.y + dy);
                        current = Step(current, next);
                        break;

                    case '(':
                        Parse(input, ref index, current);
                        break;

                    case ')':
                        return;

                    case '|':
                        current = initial;
                        break;

                    case '$':
                        return;
                }
            }
        }
    }
}
