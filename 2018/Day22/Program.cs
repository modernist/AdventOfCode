using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day22
{
    class Program
    {
        private static int Depth = 0;
        private static (int X, int Y) Target = (0, 0);
        private static Dictionary<(int X, int Y), int> Erosion;

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt");
            Depth = int.Parse(input[0].Substring(input[0].IndexOf(' ')));
            var pos = input[1].Split(new[] {' ', ','}, StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(s => int.Parse(s))
                .ToArray();
            Target = (X: pos[0], Y: pos[1]);
            Erosion = new Dictionary<(int X, int Y), int>();
            Console.WriteLine(Part1());
        }

        static int Part1()
        {
            int risk = 0;
            for (int y = 0; y <= Target.Y; y++)
            {
                for (int x = 0; x <= Target.X; x++)
                {
                    risk += (int) GetRegionType((x, y));
                }
            }

            return risk;
        }

        static int GeoIndex((int X, int Y) region)
        {
            if (!Erosion.ContainsKey(region))
            {
                if (region.X == 0 && region.Y == 0)
                    return 0;
                if (region.X == Target.X && region.Y == Target.Y)
                    return 0;
                if (region.Y == 0)
                    return 16807 * region.X;
                if (region.X == 0)
                    return 48271 * region.Y;
                return Erosion[(region.X - 1, region.Y)] * Erosion[(region.X, region.Y - 1)];
            }

            return Erosion[region];
        }

        private static int ErosionLevel((int X, int Y) region)
        {
            if (!Erosion.ContainsKey(region))
            {
                Erosion[region] = (GeoIndex(region) + Depth) % 20183;
            }

            return Erosion[region];
        }

        static RegionType GetRegionType((int X, int Y) region) => (RegionType) (ErosionLevel(region) % 3);

        enum RegionType
        {
            Rocky = 0,
            Wet = 1,
            Narrow = 2
        }
    }
}
