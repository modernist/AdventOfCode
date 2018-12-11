using System;
using System.Collections.Generic;
using System.Linq;

namespace Day11
{
    class Program
    {
        static void Main(string[] args)
        {
            int serial = 9424, width = 300, height = 300;
            var squares = new Dictionary<(int x, int y, int s), long>();
            var grid = BuildGrid(squares, width, height, serial);
            Console.WriteLine(Part1(grid, squares, width, height));
            Console.WriteLine(Part2(grid, squares, width, height));
        }

        static long[,] BuildGrid(Dictionary<(int x, int y, int s), long> squares, int width, int height, int serial)
        {
            long[,] grid = new long[width + 1, height + 1];
            for (int x = 1; x <= width; x++)
            {
                for (int y = 1; y <= height; y++)
                {
                    var rackId = x + 10L;
                    var power = ((rackId * y + serial) * rackId) % 1000 / 100 - 5;
                    grid[x, y] = power;
                    squares[(x, y, 1)] = power;
                }
            }
            return grid;
        }

        static (int, int) Part1(long[,] grid, Dictionary<(int x, int y, int s), long> squares, int width, int height)
        {
            for (int s = 1; s <= 3; s++)
            {
                Power(grid, squares, width, height, s);
            }

            var max = squares.Where(s => s.Key.s == 3).OrderByDescending(s => s.Value).Select(s => s.Key).First();
            return (max.x, max.y);
        }

        static (int x, int y, int s) Part2(long[,] grid, Dictionary<(int x, int y, int s), long> squares, int width, int height)
        {
            var maxLocation = (x: 0, y: 0, s: 0);
            long maxPower = long.MinValue;

            for (int s = 1; s <= width; s++)
            {
                var square = Power(grid, squares, width, height, s);
                if (square.p > maxPower)
                {
                    maxLocation = (square.x, square.y, square.s);
                    maxPower = square.p;
                }
            }

            return maxLocation;
        }

        static (int x, int y, int s, long p) Power(long[,] grid, Dictionary<(int x, int y, int s), long> squares, int width, int height, int edge)
        {
            var maxLocation = (x: 0, y: 0, s: 0);
            long maxPower = long.MinValue;
            for (int x = 1; x < width - edge; x++)
            {
                for (int y = 1; y < height - edge; y++)
                {
                    var location = (x, y, edge);
                    var power = 0L;
                    if (edge > 1)
                    {
                        power += squares[(x + 1, y + 1, edge - 1)];
                        for (int i = x; i < x + edge; i++)
                        {
                            power += grid[i, y];
                        }

                        for (int j = y + 1; j < y + edge; j++)
                        {
                            power += grid[x, j];
                        }
                    }

                    squares[location] = power;
                    if (power > maxPower)
                    {
                        maxPower = power;
                        maxLocation = location;
                    }
                }
            }

            return (maxLocation.x, maxLocation.y, maxLocation.s, maxPower);
        }

    }
}
