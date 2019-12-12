using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day08
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt").Select(c => int.Parse($"{c}"));
            var images = Partition(input, 25 * 6).ToArray();

            Console.WriteLine(Part1(images));

            Console.Read();
        }

        public static int Part1(IEnumerable<int>[] images)
        {
            var targetImage = images.OrderBy(i => i.Count(px => px == 0)).First();

            return targetImage.Count(px => px == 1) * targetImage.Count(px => px == 2);
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>(IEnumerable<T> items, int partitionSize)
        {
            return items.Select((item, index) => new { item, index })
                .GroupBy(x => x.index / partitionSize)
                .Select(g => g.Select(x => x.item));
        }
    }
}
