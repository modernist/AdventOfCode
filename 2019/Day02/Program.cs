using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day02
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse);

            Console.WriteLine(Part1(input.ToArray(), 12, 2));

            Console.WriteLine(Part2(input));

            Console.Read();
        }

        static int Part1(int[] input, int noun = 12, int verb = 2)
        {
            input[1] = noun;
            input[2] = verb;

            int opCodePos = 0;
            while (input[opCodePos] != 99)
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

            return input[0];
        }

        static int Part2(IEnumerable<int> input)
        {
            for (var noun = 0; noun < 100; noun++)
            {
                for (var verb = 0; verb < 100; verb++)
                {
                    var result = Part1(input.ToArray(), noun, verb);
                    if (result == 19690720)
                        return noun * 100 + verb;
                }
            }

            return 0;
        }
    }
}
