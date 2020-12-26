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

            Part2(images, 25);

            Console.Read();
        }

        public static int Part1(IEnumerable<int>[] images)
        {
            var targetImage = images.OrderBy(i => i.Count(px => px == 0)).First();

            return targetImage.Count(px => px == 1) * targetImage.Count(px => px == 2);
        }

        static void Part2(IEnumerable<int>[] images, int width)
        {
            var final = new List<Pixel>();
            var size = images[0].Count();
            final.AddRange(Enumerable.Range(0, size).Select(_ => new Pixel()));

            for (var i = images.Length - 1; i >= 0; i--)
            {
                var image = images[i].ToArray();
                for (var px = 0; px < size; px++)
                {
                    final[px].Apply(image[px]);
                }
            }

            var finalImage = Partition(final, width);
            foreach (var line in finalImage)
            {
                Console.WriteLine(string.Join("", line));
            }
        }

        class Pixel
        {
            private char _color = ' ';

            public void Apply(int layerColor)
            {
                switch (layerColor)
                {
                    case 0:
                        _color = '.';
                        break;

                    case 1:
                        _color = 'W';
                        break;
                }
            }

            public override string ToString()
            {
                return _color.ToString();
            }
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>(IEnumerable<T> items, int partitionSize)
        {
            return items.Select((item, index) => new { item, index })
                .GroupBy(x => x.index / partitionSize)
                .Select(g => g.Select(x => x.item));
        }
    }
}
