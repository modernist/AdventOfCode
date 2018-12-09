using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day9
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var words = input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            var players = int.Parse(words[0]);
            var rounds = int.Parse(words[6]);

            Console.WriteLine(Part1(players, rounds));

            Console.WriteLine(Part1(players, 100 * rounds));
        }

        static int Part1(int players, int rounds)
        {
            var marbles = new List<int>( new [] { 0 });
            var scores = Enumerable.Range(1, players).ToDictionary(i => i, i => 0);
            var round = 1;
            var current = 0;
            var player = 0;

            while (round <= rounds)
            {
                player = ((round - 1) % players) + 1;
                if (round % 23 == 0)
                {
                    var next = (current - 7) % marbles.Count;
                    if (next < 0) next += marbles.Count;
                    scores[player] += round;
                    scores[player] += marbles[next];
                    marbles.RemoveAt(next);
                    current = next;
                }
                else
                {
                    var next = (current + 1) % marbles.Count + 1;
                    marbles.Insert(next, round);
                    current = next;
                }
                round++;

                //Console.WriteLine(string.Join(' ', marbles));
            }


             return scores.Values.Max();
        }
    }
}
