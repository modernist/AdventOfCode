using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day8
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt");
            var values = input.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => int.Parse(value));

            var numbers = new Queue<int>(values);
            var rootNode = Parse(numbers);

            Console.WriteLine(Part1(rootNode));
            Console.WriteLine(Part2(rootNode));
        }

        static Node Parse(Queue<int> values)
        {
            var numChildren = values.Dequeue();
            var numMeta = values.Dequeue();

            var node = new Node();
            node.Children.AddRange(Enumerable.Range(0, numChildren).Select(c => Parse(values)));
            node.Metadata.AddRange(Enumerable.Range(0, numMeta).Select(m => values.Dequeue()));
            return node;
        }

        static int Part1(Node rootNode)
        {
            return rootNode.TotalMetaSum;
        }

        static int Part2(Node rootNode)
        {
            return rootNode.Value;
        }

        class Node
        {
            public List<Node> Children { get; } = new List<Node>();

            public List<int> Metadata { get; } = new List<int>();

            public int MetaSum => Metadata.Sum();

            public int TotalMetaSum => Children.Select(child => child.TotalMetaSum).Sum() + MetaSum;

            public int Value => (Children.Count == 0)
                ? MetaSum
                : Metadata.Select(metaIndex => metaIndex - 1)
                    .Select(actualIndex =>
                    (actualIndex < 0 || actualIndex >= Children.Count) ? 0 : Children[actualIndex].Value)
                    .Sum();
        }
    }
}
