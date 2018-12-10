using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day10
{
    class Program
    {
        static Regex regex = new Regex(@"^position=\<\s?(?<x>-?\d+),\s*(?<y>-?\d+)\> velocity=\<\s?(?<dx>-?\d+),\s*(?<dy>-?\d+)\>.*$", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var points = input.Select(line => regex.Match(line.Trim()))
                .Select(match => new Point(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value),
                    int.Parse(match.Groups["dx"].Value), int.Parse(match.Groups["dy"].Value)))
                .ToArray();

            var (part1, part2) = Timelapse(points);
            Console.WriteLine(part1);
            Console.WriteLine(part2);
        }

        static (string message, int seconds) Timelapse(IEnumerable<Point> points)
        {
            var second = 0;
            var message = string.Empty;
            var area = long.MaxValue;

            while (true)
            {
                foreach (var point in points)
                {
                    point.Step();
                }

                var rectangle = Rectangle(points);
                var currentArea = rectangle.Width * rectangle.Height;
                if (currentArea > area)
                {
                    foreach (var point in points)
                    {
                        point.Step(false);
                    }
                    rectangle = Rectangle(points);
                    second--;

                    break;
                }
                else
                {
                    area = currentArea;
                }

                second++;
            }


            return (message, second);
        }

        static (int Left, int Top, int Width, int Height) Rectangle(IEnumerable<Point> points)
        {
            var left = points.Min(p => p.X);
            var right = points.Max(p => p.X);
            var top = points.Min(p => p.Y);
            var bottom = points.Max(p => p.Y);
            return (left, top, right - left + 1, bottom - top + 1);
        }

        static string BuildMessage(IEnumerable<Point> points, (int Left, int Top, int Width, int Height) rectangle)
        {
            return string.Empty;
        }

        class Point
        {
            public int X { get; private set; }

            public int Y { get; private set; }

            public int SpeedX { get; }

            public int SpeedY { get; }

            public (int X, int Y) Step(bool forward = true)
            {
                if (forward)
                {
                    X += SpeedX;
                    Y += SpeedY;
                }
                else
                {
                    X -= SpeedX;
                    Y -= SpeedY;
                }

                return (X: X, Y: Y);
            }

            public Point(int x, int y, int speedX, int speedY)
            {
                X = x;
                Y = y;
                SpeedX = speedX;
                SpeedY = speedY;
            }
        }
    }
}
