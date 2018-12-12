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

            var next = Next(new Generation(state, state.IndexOf('#')), rules);
        }

        static Generation Next(Generation current, Dictionary<string, string> rules)
        {
            var currentState = "....." + current.State + ".....";
            var newState = string.Join("", Enumerable.Range(2, currentState.Length - 4).Select(i =>
                rules.TryGetValue(currentState.Substring(i - 2, 5), out var r) ? r : "."));

            return new Generation(newState, 0);
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
