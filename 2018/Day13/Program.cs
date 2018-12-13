using System;
using System.Collections.Generic;
using System.IO;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var dimensions = (rows: input.Length, columns: input[0].Length);
            var tracks = new char[dimensions.rows, dimensions.columns];
            var carts = new List<Cart>();
            for (int row = 0; row < dimensions.rows; row++)
            {
                for (int col = 0; col < dimensions.columns; col++)
                {
                    var c = input[row][col];
                    //assume no carts are initially on a curve or intersection
                    switch (c)
                    {
                        case '>':
                            tracks[row, col] = '-';
                            carts.Add(new Cart() { X = col, Y = row, Direction = c});
                            break;

                        case '<':
                            tracks[row, col] = '-';
                            carts.Add(new Cart() { X = col, Y = row, Direction = c });
                            break;

                        case '^':
                            tracks[row, col] = '|';
                            carts.Add(new Cart() { X = col, Y = row, Direction = c });
                            break;

                        case 'V':
                            tracks[row, col] = '|';
                            carts.Add(new Cart() { X = col, Y = row, Direction = c });
                            break;

                        default:
                            tracks[row, col] = c;
                            break;
                    }
                }
            }


        }

        class Cart
        {
            public int X { get; set; }
            public int Y { get; set; }

            public char Direction { get; set; }

            public void Move(char[,] tracks)
            {
            }
        }
    }
}
