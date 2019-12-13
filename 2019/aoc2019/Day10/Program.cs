using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
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

            Console.WriteLine(Part1(input));

            Console.Read();
        }

        static int Part1(IEnumerable<Point> input)
        {
            var asteroids = input.ToDictionary(p => p, p => new List<Point>());

            foreach (var origin in input)
            {
                foreach (var destination in input)
                {
                    if(origin.Equals(destination))
                        continue;

                    var originToDestination = new Line(origin, destination);
                    if (!input.Except(new [] { origin, destination }).Any(obstacle => originToDestination.Contains(obstacle)))
                    {
                        asteroids[origin].Add(destination);
                    }
                }
            }

            return asteroids.Max(asteroid => asteroid.Value.Count);
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
