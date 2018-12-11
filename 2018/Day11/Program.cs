using System;
using System.Collections.Generic;

namespace Day11
{
    class Program
    {
        static void Main(string[] args)
        {
            int serial = 9424, width = 300, height = 300;
            var grid = BuildGrid(width, height, serial);
            Console.WriteLine(Part1(grid, width, height, 3));
        }

        static long[,] BuildGrid(int width, int height, int serial)
        {
            long[,] grid = new long [width + 1, height + 1];
            for (long x = 1; x <= width; x++)
            {
                for (long y = 1; y <= height; y++)
                {
                    var rackId = x + 10;
                    grid[x, y] = ((rackId * y + serial) * rackId) % 1000 / 100 - 5;
                }
            }
            return grid;
        }

        static (int x, int y) Part1(long[,] grid, int width, int height, int edge)
        {
            var squares = new Dictionary<(int x, int y), long>();
            var maxLocation = (x: 0, y: 0);
            long maxPower = long.MinValue;
            for (int x = 1; x <= width - 2; x++)
            {
                for (int y = 1; y <= height - 2; y++)
                {
                    var location = (x, y);
                    squares[location] = 0;
                    for (int i = x; i < x + edge; i++)
                    {
                        for (int j = y; j < y + edge; j++)
                        {
                            squares[(x, y)] += grid[i, j];
                        }
                    }

                    if (squares[location] > maxPower)
                    {
                        maxPower = squares[location];
                        maxLocation = location;
                    }
                }
            }

            return maxLocation;
        }

    }
}
