using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);

            var part1 = Part1(input);
            Console.WriteLine(part1);
            var part2 = Part2(input);
            Console.WriteLine(part2);
        }

        static string Part1(string[] input)
        {
            var (tracks, carts) = Parse(input);

            do
            {
                var nextTick = Tick(tracks, carts);
                var crash = nextTick.crashed.FirstOrDefault();
                if (crash != null)
                {
                    return $"{crash.Position.Col},{crash.Position.Row}";
                }
            } while (true);
        }

        static string Part2(string[] input)
        {
            var (tracks, carts) = Parse(input);

            while(carts.Count > 1)
            {
                var nextTick = Tick(tracks, carts);
                carts = nextTick.remaining;
            }
            return $"{carts[0].Position.Col},{carts[0].Position.Row}";
        }

        private static (char[,] tracks, List<Cart> carts) Parse(string[] input)
        {
            var dimensions = (rows: input.Length, columns: input[0].Length);
            var tracks = new char[dimensions.rows, dimensions.columns];
            var carts = new List<Cart>();
            for (var row = 0; row < dimensions.rows; row++)
            {
                for (var col = 0; col < dimensions.columns; col++)
                {
                    var c = input[row][col];
                    //no carts are initially on a curve or intersection
                    switch (c)
                    {
                        case '>':
                        case '<':
                            tracks[row, col] = '-';
                            carts.Add(new Cart((row, col), c));
                            break;

                        case '^':
                        case 'v':
                            tracks[row, col] = '|';
                            carts.Add(new Cart((row, col), c));
                            break;

                        default:
                            tracks[row, col] = c;
                            break;
                    }
                }
            }

            return (tracks, carts);
        }

        static (List<Cart> crashed, List<Cart> remaining) Tick(char[,] tracks, List<Cart> carts)
        {
            foreach (var cart in carts.OrderBy(c => c.Position))
            {
                cart.Step();
                var collisions = carts.GroupBy(c => c.Position).Where(g => g.Count() > 1).SelectMany(g => g);

                foreach (var involved in collisions)
                {
                    involved.Crashed = true;
                }

                switch (tracks[cart.Position.Row, cart.Position.Col])
                {
                    case '/':
                        if (cart.Speed.Cols != 0) // -->/ or /<--
                        {
                            cart.RotateLeft();
                        }
                        else if (cart.Speed.Rows != 0) // upwards or downwards and /
                        {
                            cart.RotateRight();
                        }
                        break;
                    case '\\':
                        if (cart.Speed.Cols != 0) // -->\ or \<--
                        {
                            cart.RotateRight();
                        }
                        else if (cart.Speed.Rows != 0) // upwards or downwards and \
                        {
                            cart.RotateLeft();
                        }
                        break;
                    case '+':
                        cart.Turn();
                        break;
                }
            }

            var crashed = carts.Where(c => c.Crashed).ToList();
            return (crashed, carts.Except(crashed).ToList());
        }

        class Cart
        {
            public (int Row, int Col) Position { get; private set; }

            public (int Rows, int Cols) Speed { get; private set; }

            private int _crossings = 0;

            public bool Crashed { get; set; }

            public Cart((int row, int col) position, char direction)
            {
                Position = position;
                switch (direction)
                {
                    case '^':
                        Speed = (-1, 0);
                        break;
                    case 'v':
                        Speed = (1, 0);
                        break;
                    case '>':
                        Speed = (0, 1);
                        break;
                    case '<':
                        Speed = (0, -1);
                        break;
                }
            }

            public void Step()
            {
                Position = (Position.Row + Speed.Rows, Position.Col + Speed.Cols);
            }

            public void RotateLeft()
            {
                Speed = (-Speed.Cols, Speed.Rows);
            }

            public void RotateRight()
            {
                Speed = (Speed.Cols, -Speed.Rows);
            }

            public void Turn()
            {
                switch (_crossings++ % 3)
                {
                    case 0: //turn left
                        RotateLeft();
                        break;
                    case 2: //turn right
                        RotateRight();
                        break;
                }
            }
        }
    }
}
