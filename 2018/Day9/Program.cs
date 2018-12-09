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

            //Console.WriteLine(Game(9, 25));
            Console.WriteLine(Game(players, rounds));

            Console.WriteLine(Game(players, 100 * rounds));
        }

        static long Game(int players, int rounds)
        {
            var marbles = new LinkedList<long>();
            var scores = Enumerable.Range(1, players).ToDictionary(i => i, i => 0l);
            var round = 1;
            var current = marbles.AddFirst(0);

            while (round < rounds)
            {
                var player = (round - 1) % players + 1;
                if (round % 23 == 0)
                {
                    var target = Rotate(current, -7);
                    scores[player] += round;
                    scores[player] += target.Value;
                    current = target.Next;
                    marbles.Remove(target);
                }
                else
                {
                    current = marbles.AddAfter(Rotate(current, 1), round);
                }
                round++;

                //Console.WriteLine(string.Join(' ', marbles));
            }

            return scores.Values.Max();
        }

        static LinkedListNode<T> Rotate<T>(LinkedListNode<T> node, int count)
        {
            while (count < 0)
            {
                node = node.Previous ?? node.List.Last;
                count++;
            }

            while (count-- > 0)
            {
                node = node.Next ?? node.List.First;
            }

            return node;
        }
    }
}
