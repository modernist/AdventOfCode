using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day03
{
    class Program
    {
        private static Dictionary<char, (int xdiff, int ydiff)> Offsets;
        
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Select(line =>
                line.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => new Segment(s))).ToArray();

            Offsets = BuildOffsets();
            var grid = BuildGrid(input[0], input[1]);

            Console.WriteLine(Part1(grid));

            Console.WriteLine(Part2(grid));

            Console.Read();
        }

        static int Part1(Dictionary<Point, (bool wire1, int wire1Steps, bool wire2, int wire2Steps)> grid)
        {
            return grid.Where(p => p.Value.wire1 && p.Value.wire2)
                .Select(p => p.Key.ManhattanDistance(0, 0))
                .OrderBy(p => p)
                .First();
        }

        static int Part2(Dictionary<Point, (bool wire1, int wire1Steps, bool wire2, int wire2Steps)> grid)
        {
            return grid.Where(p => p.Value.wire1 && p.Value.wire2)
                .Select(p => p.Value.wire1Steps + p.Value.wire2Steps).Min();
        }

        static Dictionary<char, (int xdiff, int ydiff)> BuildOffsets()
        {
            var offsets = new Dictionary<char, (int xdiff, int ydiff)>
            {
                {'L', (-1, 0)}, {'R', (1, 0)}, {'U', (0, 1)}, {'D', (0, -1)}
            };
            return offsets;
        }

        static Dictionary<Point, (bool wire1, int wire1Steps, bool wire2, int wire2Steps)> BuildGrid(IEnumerable<Segment> wire1,
            IEnumerable<Segment> wire2)
        {
            var grid = new Dictionary<Point, (bool wire1, int wire1Steps, bool wire2, int wire2Steps)>();

            Point current = new Point(0, 0);
            int steps = 0;
            foreach (var segment in wire1)
            {
                foreach (var point in segment.Follow(current))
                {
                    steps++;

                    if (!grid.TryGetValue(point, out var wires))
                    {
                        grid[point] = (true, steps, false, 0);
                    }
                    else
                    {
                        grid[point] = (true, wires.wire1Steps, false, 0);
                    }

                    current = point;
                }
            }

            current = new Point(0, 0);
            steps = 0;
            foreach (var segment in wire2)
            {
                foreach (var point in segment.Follow(current))
                {
                    steps++;

                    if (!grid.TryGetValue(point, out var wires))
                    {
                        grid[point] = (false, 0, true, steps);
                    }
                    else
                    {
                        grid[point] = (wires.wire1, wires.wire1Steps, true, wires.wire2Steps == 0 ? steps : wires.wire2Steps);
                    }

                    current = point;
                }
            }

            return grid;
        }

        class Segment
        {
            public char Direction { get; set; }

            public int Length { get; }

            public Segment(string input)
            {
                Direction = input[0];
                Length = int.Parse(input.AsSpan(1));
            }

            public IEnumerable<Point> Follow(Point offset)
            {
                var (xdiff, ydiff) = Offsets[Direction];
                var x = offset.X;
                var y = offset.Y;

                for (var i = 0; i < Length; i++)
                {
                    x += xdiff;
                    y += ydiff;
                    yield return new Point(x, y);
                }
            }
        }

        class Point : IEquatable<Point>
        {
            public int X { get; }

            public int Y { get; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int ManhattanDistance(int x, int y)
            {
                return Math.Abs(X - x) + Math.Abs(Y - y);
            }

            public int ManhattanDistance(Point other)
            {
                return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
            }

            public bool Equals(Point other)
            {
                return other != null && X == other.X && Y == other.Y;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return X * 65535 + Y;
                }
            }
        }
    }
}
