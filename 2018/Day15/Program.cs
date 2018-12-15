using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Text;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");

            var game = new Game(input);
            Console.WriteLine(game);

        }



        class Game
        {
            private (int Row, int Col)[] _eligibleOffsets = new[] { (-1, 0), (0, -1), (0, 1), (1, 0) };
            private State _state;

            public Game(string[] input)
            {
                _state = Parse(input);
            }

            public override string ToString()
            {
                var sb = new StringBuilder(_state.Dimensions.Rows * (_state.Dimensions.Columns+2) + _state.Units.Count * 8);
                for (var r = 0; r < _state.Dimensions.Rows; r++)
                {
                    for (var c = 0; c < _state.Dimensions.Columns; c++)
                    {
                        sb.Append(_state.Grid[r, c]);
                    }

                    sb.AppendLine(" " + string.Join(", ",
                                      _state.Units.Where(u => u.Position.Row == r).Select(u => u.Summary)));
                }

                return sb.ToString();
            }


            private State Parse(string[] input)
            {
                var rows = input.Length;
                var columns = input[0].Length;
                var grid = new Block[rows, columns];
                var units = new List<Unit>();
                for (var row = 0; row < rows; row++)
                {
                    for(var col = 0; col < columns; col++)
                    {
                        switch (input[row][col])
                        {
                            case '#':
                                grid[row, col] = new Wall();
                                break;
                            case '.':
                                grid[row, col] = new Empty();
                                break;
                            case var u when u == 'E' || u == 'G':
                                var unit = (u == 'E') ? (Unit) new Elf() : new Goblin();
                                unit.Position = (row, col);
                                grid[row, col] = unit;
                                units.Add(unit);
                                break;
                        }
                    }
                }

                return new State()
                {
                    Dimensions = (rows, columns),
                    Grid = grid,
                    Units = units
                };
            }

            private bool IsValidPosition(int row, int column)
            {
                return !(row < 0 || row >= _state.Dimensions.Rows || column < 0 || column >= _state.Dimensions.Columns);
            }
        }

        class State
        {
            public (int Rows, int Columns) Dimensions { get; set; }
            public Block[,] Grid { get; set; }
            public List<Unit> Units { get; set; }
        }

        #region Game blocks

        abstract class Block { }

        class Empty : Block
        {
            public override string ToString()
            {
                return ".";
            }
        }

        class Wall : Block
        {
            public override string ToString()
            {
                return "#";
            }
        }

        class Unit : Block
        {
            public (int Row, int Column) Position { get; set; }

            public int HP { get; set; } = 200;

            public int AP { get; set; } = 3;

            public string Summary => $"{this.ToString()}({this.HP})";
        }

        class Elf : Unit
        {
            public override string ToString()
            {
                return "E";
            }
        }

        class Goblin : Unit
        {
            public override string ToString()
            {
                return "G";
            }
        }

        #endregion
    }
}
