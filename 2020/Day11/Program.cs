using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day11
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var grid = Parse(input);
            Console.WriteLine(Part1(grid));
            Console.WriteLine(Part2(grid));
        }

        static readonly (int dx, int dy)[] _neighbors = { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };

        static IEnumerable<char> Adjacent(char[,] grid, int row, int column, bool expand)
        {
            var rows = grid.GetLength(0);
            var columns = grid.GetLength(1);

            foreach (var (dx, dy) in _neighbors)
            {
                var nr = row + dy;
                var nc = column + dx;
                while (nr >= 0 && nr < rows && nc >= 0 && nc < columns)
                {
                    var seat = grid[nr, nc];
                    if (!expand)
                    {
                        yield return seat;
                        break;
                    }

                    if (seat != '.')
                    {
                        yield return seat;
                        break;
                    }

                    nr += dy;
                    nc += dx;
                }
            }
        }

        static char[,] Parse(string[] input)
        {
            var rows = input.Length;
            var columns = input[0].Length;

            var grid = new char[rows, columns];
            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    grid[row, column] = input[row][column];
                }
            }

            return grid;
        }

        static string Flatten(char[,] grid)
        {
            return string.Concat(grid.Cast<char>());
        }

        static string Pretty(char[,] grid)
        {
            var rows = grid.GetLength(0);
            var columns = grid.GetLength(1);
            var chars = grid.Cast<char>();
            StringBuilder sb = new StringBuilder(grid.Length + rows * 2);
            for (var row = 0; row < rows; row++)
            {
                sb.Append(chars.Skip(row * columns).Take(columns).ToArray());
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        static int Part1(char[,] input)
        {
            var grid = input.Clone() as char[,];
            var changed = false;

            do
            {
                changed = false;
                (grid, changed) = Step(grid, false, 4);
                //Console.WriteLine(Pretty(grid));
            } while (changed);

            return Flatten(grid).Count(c => c == '#');
        }

        static int Part2(char[,] input)
        {
            var grid = input.Clone() as char[,];
            var changed = false;

            do
            {
                changed = false;
                (grid, changed) = Step(grid, true, 5);
                //Console.WriteLine(Pretty(grid));
            } while (changed);

            return Flatten(grid).Count(c => c == '#');
        }

        static (char[,] grid, bool changed) Step(char[,] input, bool expand, int threshold)
        {
            var state = input.Clone() as char[,];
            var changed = false;

            var rows = state.GetLength(0);
            var columns = state.GetLength(1);

            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < columns; col++)
                {
                    (state[row, col], changed) = input[row, col] switch
                    {
                        'L' => Adjacent(input, row, col, expand).ToArray().Count(c => c == '#') == 0 ? ('#', true) : ('L', changed),
                        '#' => Adjacent(input, row, col, expand).ToArray().Count(c => c == '#') >= threshold ? ('L', true) : ('#', changed),
                        _ => (input[row, col], changed)
                    };
                }
            }

            return (state, changed);
        }
    }
}
