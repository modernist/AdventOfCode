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
            Console.Read();
        }

        static int Part1(IEnumerable<Point> points)
        {
            var areas = points.ToDictionary(point => point, point => -1);


            return 0;
        }

        class Point
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
        }
    }
}
