using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Day23
{
    class Program
    {
        static Regex locationRegex = new Regex(@"^pos=<(?<x>-?\d+),(?<y>-?\d+),(?<z>-?\d+)>, r=(?<r>\d+)$", RegexOptions.Compiled);
        static (int X, int Y, int Z) Zero = (0, 0, 0);

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Select(line => locationRegex.Match(line)).Select(m => new Bot()
            {
                Location = (int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value), int.Parse(m.Groups["z"].Value)),
                R = int.Parse(m.Groups["r"].Value)
            });

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        static int Part1(IEnumerable<Bot> bots)
        {
            var strongest = bots.OrderByDescending(bot => bot.R).First();
            return bots.Count(bot => ManhattanDistance(strongest.Location, bot.Location) <= strongest.R);
        }

        static (int X, int Y, int Z) Part2(IEnumerable<Bot> bots)
        {
            //(int minX, int maxX) dx = (bots.Min(bot => bot.Location.X), bots.Max(bot => bot.Location.X));
            //(int minY, int maxY) dy = (bots.Min(bot => bot.Location.Y), bots.Max(bot => bot.Location.Y));
            //(int minZ, int maxZ) dz = (bots.Min(bot => bot.Location.Z), bots.Max(bot => bot.Location.Z));
            //Console.WriteLine($"{dx}, {dy}, {dz}");

            var grid = bots.ToDictionary(bot => bot.Location, bot => 1L);

            foreach (var bot in bots)
            {
                for (int x = bot.Location.X - bot.R; x <= bot.Location.X + bot.R; x++)
                {
                    for (int y = bot.Location.Y - bot.R; y <= bot.Location.Y + bot.R; y++)
                    {
                        for (int z = bot.Location.Z - bot.R; z <= bot.Location.Z + bot.R; z++)
                        {
                            var location = (x, y, z);
                            if (WithinRange(bot.Location, location, bot.R))
                            {
                                if (!grid.ContainsKey(location))
                                {
                                    grid.Add(location, 1);
                                }
                                else
                                {
                                    grid[location]++;
                                }
                            }
                        }
                    }
                }
            }

            var nearest = grid.OrderByDescending(kvp => kvp.Value).GroupBy(g => g.Value).First().Select(g => g);
            //.ThenBy(kvp => ManhattanDistance((0, 0, 0), kvp.Key));

            return nearest.First().Key;
        }

        static int ManhattanDistance((int X, int Y, int Z) current, (int X, int Y, int Z) other)
        {
            return Math.Abs(current.X - other.X) +
                   Math.Abs(current.Y - other.Y) +
                   Math.Abs(current.Z - other.Z);
        }

        static bool WithinRange((int X, int Y, int Z) current, (int X, int Y, int Z) other, int R)
        {
            var sum = Math.Abs(current.X - other.X);
            if (sum > R)
                return false;
            sum += Math.Abs(current.Y - other.Y);
            if (sum > R)
                return false;
            return (sum + Math.Abs(current.Z - other.Z)) <= R;
        }


        class Bot
        {
            public (int X, int Y, int Z) Location { get; set; }

            public int R { get; set; }
        }
    }
}
