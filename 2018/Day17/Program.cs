using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Day17
{
    class Program
    {
        static Regex coordRegex = new Regex(@"^(?:x=(?<x>[0-9.]+),\sy=(?<y>[0-9.]+))|(?:y=(?<y>[0-9.]+),\sx=(?<x>[0-9.]+))$");

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Select(line => coordRegex.Match(line).Groups)
                .Select(g => (x: g["x"].Value, y: g["y"].Value));

            Console.WriteLine(Part1(input, (0, 500)));
        }

        static int Part1(IEnumerable<(string, string)> input, (int row, int col) source)
        {
            var simulation = new Simulation(input);
            return 0;
        }

        class Simulation
        {
            private Block[,] _grid;
            private (int left, int right, int top, int bottom) _borders;
            private int Width => _borders.right - _borders.left + 1;
            private int Height => _borders.bottom - _borders.top + 1;

            public Simulation(IEnumerable<(string x, string y)> input)
            {
                Parse(input);
            }

            void Run((int row, int col) source)
            {
                var sourceCol = source.col - _borders.left;

            }

            private void Parse(IEnumerable<(string x, string y)> input)
            {
                var coords = new HashSet<(int row, int col)>();
                foreach (var area in input)
                {
                    var xcoords = area.x.Split("..", StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
                    var ycoords = area.y.Split("..", StringSplitOptions.RemoveEmptyEntries).Select(y => int.Parse(y)).ToArray();

                    if (xcoords.Length == 2)
                    {
                        xcoords = Enumerable.Range(xcoords[0], xcoords[1] - xcoords[0] + 1).ToArray();
                    }

                    if (ycoords.Length == 2)
                    {
                        ycoords = Enumerable.Range(ycoords[0], ycoords[1] - ycoords[0] + 1).ToArray();
                    }

                    foreach (var x in xcoords)
                    {
                        foreach (var y in ycoords)
                        {
                            coords.Add((y, x));
                        }
                    }
                }

                var minX = coords.Min(p => p.col);
                var maxX = coords.Max(p => p.col);
                var minY = coords.Min(p => p.row);
                var maxY = coords.Max(p => p.row);

                _grid = new Block[maxY - minY + 1, maxX - minX + 1];
                for (var row = minY; row <= maxY; row++)
                {
                    for (var col = minX; col <= maxX; col++)
                    {
                        _grid[row - minY, col - minX] = coords.Contains((row, col)) ? (Block) new Clay() : new Sand();
                    }
                }

                _borders = (minX, maxX, minY, maxY);
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                for (int row = 0; row < Height; row++)
                {
                    for (int col = 0; col < Width; col++)
                    {
                        sb.Append(_grid[row, col]);
                    }

                    sb.AppendLine();
                }

                return sb.ToString();
            }
        }

        #region Block classes

        abstract class Block
        {
            public (int X, int Y) Position { get; set; }
        }

        class Sand : Block
        {
            public override string ToString()
            {
                return ".";
            }
        }

        class Clay : Block
        {
            public override string ToString()
            {
                return "#";
            }
        }

        class Water : Block
        {
            public override string ToString()
            {
                return "~";
            }
        }

        class Reachable : Block
        {
            public override string ToString()
            {
                return "|";
            }
        }

        #endregion
    }
}
