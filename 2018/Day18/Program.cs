using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day18
{
    class Program
    {
        private static (int dx, int dy)[] neighbors = { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(string[] input)
        {
            var grid = Parse(input);
            for (int i = 0; i < 10; i++)
            {
                grid = Step(grid);
            }

            var state = grid.Cast<char>();
            return state.Count(c => c == '|') * state.Count(c => c == '#');
        }

        static int Part2(string[] input)
        {
            var grid = Parse(input);
            var seen = new Dictionary<string, int>(); // state => step
            var step = 0;
            var flat = Flatten(grid);
            while (!seen.ContainsKey(flat))
            {
                seen.Add(flat, step);
                grid = Step(grid);
                flat = Flatten(grid);
                step++;
            }

            var finalState = Flatten(grid);
            var loopStart = seen[finalState];
            var loopLength = step - loopStart;
            var totalLoops = (1_000_000_000 - loopStart) % loopLength + loopStart;
            var final = seen.First(s => s.Value == totalLoops).Key;
            return final.Count(c => c == '|') * final.Count(c => c == '#');
        }

        static string Flatten(char[,] grid)
        {
            return string.Join("", grid.Cast<char>());
        }

        static char[,] Step(char[,] grid)
        {
            var state = (char[,])grid.Clone();

            for (int row = 0; row < 50; row++)
            {
                for (int col = 0; col < 50; col++)
                {
                    switch (grid[row, col])
                    {
                        case '.':
                            state[row, col] = Adjacent(grid, row, col).Count(c => c == '|') >= 3 ? '|' : '.';
                            break;
                        case '|':
                            state[row, col] = Adjacent(grid, row, col).Count(c => c == '#') >= 3 ? '#' : '|';
                            break;
                        case '#':
                            var totals = (trees: 0, lumberyards: 0);
                            totals = Adjacent(grid, row, col).Aggregate(totals, (t, c) =>
                            {
                                if (c == '|') t.trees++;
                                if (c == '#') t.lumberyards++;
                                return t;
                            });
                            state[row, col] = (totals.lumberyards >= 1 && totals.trees >= 1) ? '#' : '.';
                            break;
                    }
                }
            }

            return state;
        }

        static char[,] Parse(string[] input)
        {
            var grid = new char[50, 50];
            for (int row = 0; row < 50; row++)
            {
                for (int col = 0; col < 50; col++)
                {
                    grid[row, col] = input[row][col];
                }
            }

            return grid;
        }

        static bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < 50 && col >= 0 && col < 50;
        }

        static IEnumerable<char> Adjacent(char[,] grid, int row, int col)
        {
            foreach (var neighbor in neighbors)
            {
                var ar = row + neighbor.dy;
                var ac = col + neighbor.dx;
                if (IsValidPosition(ar, ac))
                {
                    yield return grid[ar, ac];
                }
            }
        }
    }
}
