using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Math = System.Math;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt")
                .SelectMany((line, y) => line.Select((c, x) => (X: x, Y: y, C: c)))
                .Where(p => p.C == '#')
                .Select(p => new Point(p.X, p.Y));

            var map = BuildAsteriskVisibilityMap(input);

            Console.WriteLine(Part1(map));

            Console.WriteLine(Part2(map));

            Console.Read();
        }

        static int Part1(Dictionary<Point, List<Point>> map)
        {
            return map.Max(asteroid => asteroid.Value.Count);
        }

        static int Part2(Dictionary<Point, List<Point>> map)
        {
            var (station, visible) = map.OrderByDescending(asteroid => asteroid.Value.Count).First();

            // adjust visible asterisks by offsetting around the station, generate all lines from the station
            var lines = visible.Select(other => new Line(new Point(0, 0), new Point(other.X - station.X, other.Y - station.Y))).ToList();

            //calculate polar coordinates direction for each line, assuming the station is at (0,0), order descending to move clockwise
            var targets = lines.OrderBy(line =>
            {
                var slope = line.Slope();
                return -Math.Atan2(slope.Dx, slope.Dy);
            });

            var last = targets.ElementAt(199);

            // adjust location based on station location
            return (last.B.X + station.X) * 100 + (last.B.Y + station.Y);
        }

        private static Dictionary<Point, List<Point>> BuildAsteriskVisibilityMap(IEnumerable<Point> input)
        {
            var asteroids = input.ToDictionary(p => p, p => new List<Point>());

            foreach (var origin in input)
            {
                foreach (var destination in input)
                {
                    if (origin.Equals(destination))
                        continue;

                    var originToDestination = new Line(origin, destination);
                    if (!input.Except(new[] {origin, destination}).Any(obstacle => originToDestination.Contains(obstacle)))
                    {
                        asteroids[origin].Add(destination);
                    }
                }
            }

            return asteroids;
        }
    }

    class Line
    {
        public Point A { get; }

        public Point B { get; }

        public Line(Point a, Point b)
        {
            A = a;
            B = b;
        }

        public bool IsCollinear(Point c)
        {
            return (A.X * (B.Y - c.Y) + B.X * (c.Y - A.Y) + c.X * (A.Y - B.Y)) == 0;
        }

        public bool Contains(Point c)
        {
            if (!IsCollinear(c))
                return false;

            var dx = B.X - A.X;
            var dy = B.Y - A.Y;

            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                return dx > 0
                    ? A.X <= c.X && c.X <= B.X
                    : B.X <= c.X && c.X <= A.X;
            }
            else
            {
                return dy > 0
                    ? A.Y <= c.Y && c.Y <= B.Y
                    : B.Y <= c.Y && c.Y <= A.Y;
            }
        }

        public (int Dx, int Dy) Slope()
        {
            var (dx, dy) = (B.X - A.X, B.Y - A.Y);
            var gcd = Math.Abs(Gcd(dx, dy));
            return (dx / gcd, dy / gcd);
        }

        private int Gcd(int a, int b)
        {
            while (b != 0)
            {
                (a, b) = (b, a % b);
            }

            return a;
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

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
