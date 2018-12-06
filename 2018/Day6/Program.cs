using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var points = input.Select(line => line.Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries))
                .Select(coords => new Point(int.Parse(coords[0]), int.Parse(coords[1])));

            Console.WriteLine(Part1(points));
            Console.WriteLine(Part2(points));
        }

        static int Part1(IEnumerable<Point> points)
        {
            var areas = points.ToDictionary(point => point, point => 0);
            //extend the grid by 1 in each direction
            var left = points.Min(point => point.X) - 1;
            var right = points.Max(point => point.X) + 1;
            var top = points.Min(point => point.Y) - 1;
            var bottom = points.Max(point => point.Y) + 1;

            for (var x = left; x <= right; x++)
            {
                for (var y = top; y <= bottom; y++)
                {
                    var minDistance = points.Select(point => point.ManhattanDistance(x, y)).Min();
                    var candidates = points.Where(point => point.ManhattanDistance(x, y) == minDistance).ToArray();

                    if (candidates.Length != 1)
                    {
                        continue; //break ties
                    }

                    //if the current location is adjacent to the end of the grid the point's area is infinite
                    if (x == left || x == right || y == top || y == bottom)
                    {
                        foreach (var candidate in candidates)
                        {
                            areas[candidate] = int.MaxValue;
                        }
                    }
                    else
                    {
                        foreach (var candidate in candidates)
                        {
                            if (areas[candidate] != int.MaxValue)
                            {
                                areas[candidate]++;
                            }
                        }
                    }
                }
            }

            return areas.Values.Where(v => v != int.MaxValue).Max();
        }

        static int Part2(IEnumerable<Point> points)
        {
            //extend the grid by 1 in each direction
            var left = points.Min(point => point.X) - 1;
            var right = points.Max(point => point.X) + 1;
            var top = points.Min(point => point.Y) - 1;
            var bottom = points.Max(point => point.Y) + 1;

            var region = 0;

            for (var x = left; x <= right; x++)
            {
                for (var y = top; y <= bottom; y++)
                {
                    var totalDistance = points.Select(point => point.ManhattanDistance(x, y)).Sum();
                    if (totalDistance < 10000)
                    {
                        //(x, y) should be included in the region
                        region++;
                    }
                }
            }

            return region;
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
