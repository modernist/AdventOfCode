using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day04
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllText("input.txt")
                .Split("\n\n")
                .Select(passport => passport.Split(new[] {' ', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(field => field.Split(':'))
                    .ToDictionary(fields => fields[0], fields => fields[1]));
            
            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static int Part1(IEnumerable<Dictionary<string, string>> input)
        {
            return input.Count(passport => _rules.All(rule => passport.ContainsKey(rule.Key)));
        }

        private static int Part2(IEnumerable<Dictionary<string, string>> input)
        {
            return input.Count(passport => _rules.All(rule => passport.ContainsKey(rule.Key) && Regex.IsMatch(passport[rule.Key], $"^{rule.Value}$")));
        }

        private static readonly Dictionary<string, string> _rules = new Dictionary<string, string>()
        {
            {"byr", "19[2-9][0-9]|200[0-2]"},
            {"iyr", "201[0-9]|2020"},
            {"eyr", "202[0-9]|2030"},
            {"hgt", "1[5-8][0-9]cm|19[0-3]cm|59in|6[0-9]in|7[0-6]in"},
            {"hcl", "#[0-9a-f]{6}"},
            {"ecl", "amb|blu|brn|gry|grn|hzl|oth"},
            {"pid", "[0-9]{9}"}
        };
    }
}
