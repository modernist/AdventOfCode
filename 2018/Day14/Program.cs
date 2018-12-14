using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day14
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = 554401;

            Console.WriteLine(Part1(input));

            Console.WriteLine(Part2("554401"));
        }

        static string Part1(int input)
        {
            var recipes = new List<byte>() { 3, 7 };
            int elf1 = 0, elf2 = 1;

            while (recipes.Count < input + 10)
            {
                var sum = recipes[elf1] + recipes[elf2];
                if (sum >= 10)
                {
                    recipes.Add(1);
                }
                recipes.Add((byte)(sum % 10));
                elf1 = (elf1 + recipes[elf1] + 1) % recipes.Count;
                elf2 = (elf2 + recipes[elf2] + 1) % recipes.Count;
            }

            return string.Join("", recipes.Skip(input).Take(10));
        }

        static int Part2(string input)
        {
            var recipes = new List<byte>() { 3, 7 };
            int elf1 = 0, elf2 = 1;

            while (true)
            {
                var sum = recipes[elf1] + recipes[elf2];
                if (sum >= 10)
                {
                    recipes.Add(1);
                }
                recipes.Add((byte)(sum % 10));
                elf1 = (elf1 + recipes[elf1] + 1) % recipes.Count;
                elf2 = (elf2 + recipes[elf2] + 1) % recipes.Count;

                var position = string.Join("", recipes.Skip(recipes.Count - input.Length - 1)).IndexOf(input);
                if (position != -1)
                {
                    return recipes.Count - input.Length - 1 + position;
                }
            }
        }
    }
}
