using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Day15
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(string[] input)
        {
            var game = new Game(input, 3);
            return game.Play().Score;
        }

        static int Part2(string[] input)
        {
            var elfAP = 3;
            while (true)
            {
                var game = new Game(input, elfAP);
                var result = game.Play();
                if (result.NoElfCasualties)
                {
                    return result.Score;
                }

                elfAP++;
            }
        }

        class Game
        {
            private readonly (int Row, int Col)[] _eligibleOffsets = new[] { (-1, 0), (0, -1), (0, 1), (1, 0) }; //in reading order
            private State _state;

            public Game(string[] input, int elfAP)
            {
                _state = Parse(input);
                foreach (var elf in _state.Units.OfType<Elf>())
                {
                    elf.AP = elfAP;
                }
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

            public (bool NoElfCasualties, int Score) Play()
            {
                var rounds = 0;
                var elves = _state.Units.OfType<Elf>().Count();

                //Console.WriteLine(this);
                //Console.WriteLine();
                while (Step())
                {
                    rounds++;
                    //Console.WriteLine(this);
                }
                return ((_state.Units.OfType<Elf>().Count() == elves), (rounds - 1) * _state.Units.Sum(u => u.HP));
            }

            private bool Step()
            {
                var moved = false;
                foreach (var unit in _state.Units.OrderBy(u => u.Position))
                {
                    if (unit.HP <= 0)
                    {
                        continue;
                    }
                    if (Attack(unit))
                    {
                        moved = true;
                    }
                    else
                    {
                        moved |= Move(unit);
                        moved |= Attack(unit);
                    }
                }

                return moved;
            }

            private bool Move(Unit unit)
            {
                var targets = ClosestTargetsByDistance(unit);
                if (!targets.Any())
                {
                    return false;
                }

                var target = targets.OrderBy(t => t.target.Position).FirstOrDefault();
                var nextMove = targets.Where(t => t.target == target.target).Select(t => t.nextMove).OrderBy(t => t).First();
                var nextBlock = _state.Grid[nextMove.Row, nextMove.Col];
                _state.Grid[nextMove.Row, nextMove.Col] = unit;
                _state.Grid[unit.Position.Row, unit.Position.Column] = nextBlock;
                //(_state.Grid[nextMove.Row, nextMove.Col], _state.Grid[unit.Position.Row, unit.Position.Column]) =
                //    (_state.Grid[unit.Position.Row, unit.Position.Column], _state.Grid[nextMove.Row, nextMove.Col]);
                unit.Position = nextMove;
                return true;
            }

            IEnumerable<(Unit target, (int Row, int Col) nextMove)> ClosestTargetsByDistance(Unit unit)
            {
                var minimumDistance = int.MaxValue;
                foreach (var (target, nextMove, distance) in FindTargetsByDistance(unit))
                {
                    if (distance > minimumDistance)
                    {
                        break; //no need to examine this path any further
                    }
                    else
                    {
                        minimumDistance = distance;
                        yield return (target, nextMove);
                    }
                }
            }

            private IEnumerable<(Unit target, (int Row, int Col) nextMove, int distance)> FindTargetsByDistance(Unit unit)
            {
                var seen = new HashSet<(int Row, int Column)>();
                seen.Add(unit.Position);
                //BFS, examine all the paths to the targets, break ties in reading order
                var eligiblePaths = new Queue<((int Row, int Col) targetPosition, (int Row, int Col) sourcePosition, int distance)>();

                //target adjacent blocks from which the attack can be performed
                foreach (var offset in _eligibleOffsets)
                {
                    var targetRow = unit.Position.Row + offset.Row;
                    var targetColumn = unit.Position.Column + offset.Col;
                    var targetPosition = (targetRow, targetColumn);
                    eligiblePaths.Enqueue((targetPosition, targetPosition, 1));
                }

                while (eligiblePaths.Any())
                {
                    var (position, source, distance) = eligiblePaths.Dequeue();
                    if (!IsValidPosition(position.Row, position.Col))
                    {
                        continue;
                    }

                    var target = _state.Grid[position.Row, position.Col];
                    if (target is Unit && target != unit && target.GetType() != unit.GetType())
                    {
                        yield return (target as Unit, source, distance);
                    }
                    else if (target is Empty)
                    {
                        //extend path
                        foreach (var offset in _eligibleOffsets)
                        {
                            var targetRow = position.Row + offset.Row;
                            var targetColumn = position.Col + offset.Col;
                            var targetPosition = (targetRow, targetColumn);
                            if (seen.Add(targetPosition))
                            {
                                eligiblePaths.Enqueue((targetPosition, source, distance + 1));
                            }
                        }
                    }
                }

            }

            private bool Attack(Unit unit)
            {
                var targets = new List<Unit>();
                foreach (var eligible in _eligibleOffsets)
                {
                    var targetRow = unit.Position.Row + eligible.Row;
                    var targetColumn = unit.Position.Column + eligible.Col;
                    if (!IsValidPosition(targetRow, targetColumn))
                    {
                        continue;
                    }

                    var target = _state.Grid[targetRow, targetColumn];
                    if (target is Unit && target.GetType() != unit.GetType())
                    {
                        targets.Add(target as Unit);
                    }
                }

                if (!targets.Any())
                {
                    return false;
                }

                var opponent = targets.OrderBy(t => t.HP).First();
                opponent.HP -= unit.AP;
                if (opponent.HP <= 0)
                {
                    _state.Units.Remove(opponent);
                    _state.Grid[opponent.Position.Row, opponent.Position.Column] = new Empty();
                }

                return true;
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
