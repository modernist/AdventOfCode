using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            var regex = new Regex(@"^#(?<id>\d+)\s*@\s*(?<left>\d+),(?<top>\d+):\s*(?<width>\d+)x(?<height>\d+)$", RegexOptions.Compiled);

            var claims = lines.Select(line =>
            {
                var match = regex.Match(line);
                return new Claim()
                {
                    Id = match.Groups["id"].Value,
                    Left = int.Parse(match.Groups["left"].Value),
                    Top = int.Parse(match.Groups["top"].Value),
                    Width = int.Parse(match.Groups["width"].Value),
                    Height = int.Parse(match.Groups["height"].Value)
                };
            });

            var dimensions = (Width: claims.Select(claim => claim.Left + claim.Width).Max(),
                Height: claims.Select(claim => claim.Top + claim.Height).Max());

            var counts = new Dictionary<int, int>(Enumerable.Range(0, dimensions.Width * dimensions.Height)
                .Select(i => new KeyValuePair<int, int>(i, 0)));

            foreach (var claim in claims)
            {
                foreach (var block in claim.Blocks(dimensions.Width))
                {
                    counts[block]++;
                }
            }

            Console.WriteLine(counts.Count(count => count.Value > 1));

            var noConflict = claims.FirstOrDefault(claim => claim.Blocks(dimensions.Width).All(index => counts[index] == 1));

            Console.WriteLine(noConflict?.Id ?? "not found");
        }

        class Claim
        {
            public string Id { get; set; }
            public int Left { get; set; }
            public int Top { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public IEnumerable<int> Blocks(int columnsPerLine)
            {
                for (int row = Top; row < Top + Height; row++)
                {
                    for (int column = Left; column < Left + Width; column++)
                    {
                        yield return row * columnsPerLine + column;
                    }
                }
            }
        }
    }
}
