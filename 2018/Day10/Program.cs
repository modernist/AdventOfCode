using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day10
{
    class Program
    {
        static Regex regex = new Regex(@"^position=\<\s?(?<x>-?\d+),\s*(?<y>-?\d+)\> velocity=\<\s?(?<dx>-?\d+),\s*(?<dy>-?\d+)\>.*$", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var points = input.Select(line => regex.Match(line))
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
                var currentArea = (long)rectangle.Width * rectangle.Height;
                if (currentArea > area)
                {
                    //time to go back
                    foreach (var point in points)
                    {
                        point.Step(false);
                    }
                    rectangle = Rectangle(points);
                    message = BuildMessage(points, rectangle);
                    break;
                }
                area = currentArea;
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
            //todo: if points is a SortedList<Point> every point mapped can be removed from the list and it will probably
            //be the first in the list
            var result = new StringBuilder(rectangle.Width * rectangle.Height);
            for (var y = rectangle.Top; y <= rectangle.Top + rectangle.Height; y++)
            {
                for (var x = rectangle.Left; x <= rectangle.Left + rectangle.Width; x++)
                {
                    result.Append(points.Any(p => p.X == x && p.Y == y) ? '#' : ' ');
                }

                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }

        class Point
        {
            public int X { get; private set; }

            public int Y { get; private set; }

            private readonly int _speedX;

            private readonly int _speedY;

            public void Step(bool forward = true)
            {
                if (forward)
                {
                    X += _speedX;
                    Y += _speedY;
                }
                else
                {
                    X -= _speedX;
                    Y -= _speedY;
                }
            }

            public Point(int x, int y, int speedX, int speedY)
            {
                X = x;
                Y = y;
                _speedX = speedX;
                _speedY = speedY;
            }
        }
    }
}
