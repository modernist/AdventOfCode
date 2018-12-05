using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day4
{
    class Program
    {
        private static readonly Regex regex = new Regex(@"^\[(?<date>.*)\]\s(?:(?<event>(?:(?:wakes up)|(?:falls asleep)))|(?:Guard #(?<guard>\d+)\s(?<event>(?:begins shift))))$", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            var lines = File.ReadAllLines("input.txt");
            Array.Sort(lines);
            var timeline = lines.Select(ParseEntry).ToArray();

            var guards = CalculateSleepIntervals(timeline);

            Console.WriteLine(Part1(guards));
            Console.WriteLine(Part2(guards));
        }

        private static int Part1(Dictionary<int, int[]> guards)
        {
            var sleepiestGuard = guards.Select(guard =>
                (Guard: guard.Key,
                TotalAsleep: guard.Value.Sum(),
                MostSleepyMinute: Array.IndexOf(guard.Value, guard.Value.Max())))
                .OrderByDescending(g => g.TotalAsleep)
                .First();

            return sleepiestGuard.Guard * sleepiestGuard.MostSleepyMinute;
        }

        private static int Part2(Dictionary<int, int[]> guards)
        {
            var steadiestGuard = guards.Select(g => (Guard: g.Key, MostFrequent: g.Value.Max(), Minutes: g.Value))
                .OrderByDescending(g => g.MostFrequent)
                .Select(g => (Guard: g.Guard, Minute: Array.IndexOf(g.Minutes, g.MostFrequent))).First();

            return steadiestGuard.Guard * steadiestGuard.Minute;
        }

        private static Dictionary<int, int[]> CalculateSleepIntervals(TimelineEntry[] timeline)
        {
            var guards = timeline.Where(entry => entry.Guard > 0)
                .Select(entry => entry.Guard)
                .Distinct()
                .ToDictionary(guardId => guardId, guardId => new int[60]);

            var guard = timeline.First().Guard;
            for (var i = 1; i < timeline.Length; i++)
            {
                var entry = timeline[i];
                if (entry.Event == "falls asleep")
                {
                    var start = entry.Date;
                    var end = timeline[i + 1].Date;
                    for (int minute = start.Minute; minute < end.Minute; minute++)
                    {
                        guards[guard][minute]++;
                    }
                }

                guard = timeline[i].Guard > 0 ? timeline[i].Guard : guard;
            }

            return guards;
        }

        private static TimelineEntry ParseEntry(string line)
        {
            var match = regex.Match(line);
            return new TimelineEntry()
            {
                Date = DateTime.ParseExact(match.Groups["date"].Value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                Event = match.Groups["event"].Value,
                Guard = (match.Groups["guard"].Length > 0) ? int.Parse(match.Groups["guard"].Value) : 0
            };
        }

        class TimelineEntry
        {
            public DateTime Date { get; set; }

            public int Guard { get; set; }

            public string Event { get; set; }
        }
    }
}
