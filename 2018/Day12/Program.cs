using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day12
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            var state = input[0].Substring(input[0].IndexOf(':') + 1).Trim();
            var rules = input.Skip(2)
                .Select(rule => rule.Split(new[] {' ', '=', '>'}, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(r => r[0], r => r[1]);

            var current = new Generation(state, state.IndexOf('#'));

            var part1 = Timelapse(current, 20, rules);
            Console.WriteLine(part1);
        }

        static int Timelapse(Generation current, int generations, Dictionary<string, string> rules)
        {
            var state = current;
            for (int gen = 1; gen <= generations; gen++)
            {
                state = Next(state, rules);
            }

            return state.State.Select((p, i) => p == '#' ? i + state.First : 0).Sum();
        }

        static Generation Next(Generation current, Dictionary<string, string> rules)
        {
            var currentState = "....." + current.State + ".....";
            var newState = string.Join("", Enumerable.Range(2, currentState.Length - 4).Select(i =>
                rules.TryGetValue(currentState.Substring(i - 2, 5), out var r) ? r : "."));

            var leftmost = newState.IndexOf('#');
            var newFirst = leftmost + current.First - 3;
            newState = newState.Substring(leftmost, newState.LastIndexOf('#') - leftmost + 1);
            return new Generation(newState, newFirst);
        }

        class Generation
        {
            public string State { get; set; }

            public int First { get; set; }

            public Generation(string state, int first)
            {
                State = state;
                First = first;
            }
        }
    }
}
