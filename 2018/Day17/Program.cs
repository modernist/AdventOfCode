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
        static Regex coordRegex = new Regex(@"^(?:x=(?<x>[0-9.]+),\sy=(?<y>[0-9.]+))|(?:y=(?<y>[0-9.]+),\sx=(?<x>[0-9.]+))$", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Select(line => coordRegex.Match(line).Groups)
                .Select(g => (x: g["x"].Value, y: g["y"].Value));

            var simulation = new Simulation(input, (0, 500));

            Console.WriteLine(Part1(simulation));
            Console.WriteLine(Part2(simulation));
        }

        static int Part1(Simulation simulation)
        {
            return simulation.Count(block => block is Water || block is Reachable);
        }

        static int Part2(Simulation simulation)
        {
            return simulation.Count(block => block is Water);
        }

        class Simulation
        {
            private Block[,] _grid;
            private (int left, int right, int top, int bottom) _borders;
            private int Width => _borders.right - _borders.left + 1;
            private int Height => _borders.bottom - _borders.top + 1;

            private bool IsValidGridPosition(int row, int col)
            {
                return row >= 0 && row < Height && col >= 0 && col < Width;
            }

            public Simulation(IEnumerable<(string x, string y)> input, (int row, int col) source)
            {
                Parse(input);
                //Console.WriteLine(simulation);
                var sourceCol = source.col - _borders.left;
                Flow(0, sourceCol);
                //Console.WriteLine("---------------------------------");
                //Console.WriteLine(simulation);
            }

            public int Count(Func<Block, bool> predicate)
            {
                return _grid.Cast<Block>().Count(predicate);
            }

            private void Flow(int row, int col)
            {
                if (!(_grid[row, col] is Sand))
                {
                    return;
                }

                var blockRow = _borders.top + row;
                _grid[row, col] = new Reachable();
                if (blockRow == _borders.bottom)
                {
                    return;
                }

                Flow(row + 1, col);

                if (_grid[row + 1, col] is Clay || _grid[row + 1, col] is Water)
                {
                    if (col > 0)
                    {
                        Flow(row, col - 1);
                    }

                    if (col < Width - 1)
                    {
                        Flow(row, col + 1);
                    }
                }

                if (CanRetainWater(row, col))
                {
                    for (var x = col; IsValidGridPosition(row, x) && _grid[row, x] is Reachable; x--)
                    {
                        _grid[row, x] = new Water();
                    }
                    for (var x = col; IsValidGridPosition(row, x) && _grid[row, x] is Reachable; x++)
                    {
                        _grid[row, x] = new Water();
                    }
                }
            }

            bool CanRetainWater(int row, int col)
            {
                for (var x = col; IsValidGridPosition(row, x) && !(_grid[row, x] is Clay); x--)
                {
                    if (_grid[row, x] is Sand || _grid[row + 1, x] is Reachable)
                    {
                        return false;
                    }
                }
                for (var x = col; IsValidGridPosition(row, x) && !(_grid[row, x] is Clay); x++)
                {
                    if (_grid[row, x] is Sand || _grid[row + 1, x] is Reachable)
                    {
                        return false;
                    }
                }

                return true;
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

                var minX = coords.Min(p => p.col) - 1; // expand 1 to the left
                var maxX = coords.Max(p => p.col) + 1; // expand 1 to the right
                var minY = coords.Min(p => p.row);
                var maxY = coords.Max(p => p.row);

                _grid = new Block[maxY - minY + 1, maxX - minX + 1];
                for (var row = minY; row <= maxY; row++)
                {
                    for (var col = minX; col <= maxX; col++)
                    {
                        _grid[row - minY, col - minX] = coords.Contains((row, col))
                            ? (Block) new Clay()
                            : new Sand();
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
