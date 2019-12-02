using System;
using System.IO;
using System.Linq;

namespace Day02
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToArray();

            Console.WriteLine(Part1(input));

            Console.Read();
        }

        static int Part1(int[] input)
        {
            //prepare input
            var pos1 = input[1]; 
            var pos2 = input[2];
            input[1] = 12;
            input[2] = 2;

            // test input = new[] {1, 1, 1, 4, 99, 5, 6, 0, 99};

            int opCodePos = 0;
            while (input[opCodePos] != 99)
            {
                Step(input, ref opCodePos);
            }

            return input[0];
        }

        static void Step(int[] input, ref int opCodePos)
        {
            switch (input[opCodePos])
            {
                case 1: //add
                    input[input[opCodePos + 3]] = input[input[opCodePos + 1]] + input[input[opCodePos + 2]];
                    opCodePos += 4;
                    break;

                case 2: //multiply
                    input[input[opCodePos + 3]] = input[input[opCodePos + 1]] * input[input[opCodePos + 2]];
                    opCodePos += 4;
                    break;
            }
        }
    }
}
