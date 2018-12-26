using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day24
{
    class Program
    {
        //20 units each with 1333 hit points (immune to radiation, slashing; weak to bludgeoning) with an attack that does 508 fire damage at initiative 3
        static Regex inputRegex = new Regex(@"^(?<units>\d+) units each with (?<hp>\d+) hit points (?:\((?<immuneweak>.*)\) )?with an attack that does (?<damage>\d+) (?<damagetype>\w+) damage at initiative (?<initiative>\d+)", RegexOptions.Compiled);
        static Regex immuneweakRegex = new Regex(@"(?<category>immune|weak) to (?:(?<type>\w+),?\s?;?)+", RegexOptions.Compiled);


        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Skip(1).ToList();

            Console.WriteLine(Part1(input));
            Console.WriteLine(Part2(input));
        }

        private static List<Group> Parse(List<string> input)
        {
            var split = input.IndexOf("Infection:");
            var immune = input.Take(split - 1).Select(line => (type: GroupType.Immune, match: inputRegex.Match(line)));
            var infection = input.Skip(split + 1).Select(line => (Type: GroupType.Infection, match: inputRegex.Match(line)));
            var groups = new List<Group>();
            int id = 0;

            foreach (var (type, im) in immune.Union(infection))
            {
                var group = new Group()
                {
                    ID = id++,
                    GroupType = type,
                    AttackType = Enum.Parse<AttackType>(im.Groups["damagetype"].Value, true),
                    Units = int.Parse(im.Groups["units"].Value),
                    HitPoints = int.Parse(im.Groups["hp"].Value),
                    AttackDamage = int.Parse(im.Groups["damage"].Value),
                    Initiative = int.Parse(im.Groups["initiative"].Value)
                };

                var iwmc = immuneweakRegex.Matches(im.Groups["immuneweak"].Value)
                    .ToDictionary(m => m.Groups["category"].Value,
                        m => m.Groups[2].Captures.Select(t => Enum.Parse<AttackType>(t.Value, true)));

                foreach (var iwm in iwmc)
                {
                    if (iwm.Key == "immune")
                        group.Immunities.AddRange(iwm.Value);
                    else
                        group.Weaknesses.AddRange(iwm.Value);
                }

                groups.Add(group);
            }

            return groups;
        }

        static int Part1(List<string> input)
        {
            var groups = Parse(input);
            while (groups.GroupBy(g => g.GroupType).Count() == 2)
            {
                Fight(groups);
            }

            return groups.Sum(g => g.Units);
        }

        static int Part2(List<string> input)
        {
            var groups = new List<Group>();
            var boost = 0;

            do
            {
                groups = Parse(input);
                boost++;
                var totalUnits = 0;

                foreach (var group in groups.Where(g => g.GroupType == GroupType.Immune))
                {
                    group.AttackDamage += boost;
                }

                while (groups.GroupBy(g => g.GroupType).Count() == 2)
                {
                    Fight(groups);
                    var currentTotal = groups.Sum(g => g.Units);
                    if (currentTotal == totalUnits)
                        break;
                    totalUnits = currentTotal;
                }
            } while (groups.Any(g => g.GroupType == GroupType.Infection && !g.IsExtinct));

            return groups.Sum(g => g.Units);

        }

        static void Fight(List<Group> groups)
        {
            var selectedTargets = SelectTargets(groups).OrderByDescending(x => x.Attacker.Initiative).ToList();

            foreach (var battle in selectedTargets)
            {
                battle.Attacker.Attack(battle.Target);
            }

            groups.RemoveAll(g => g.IsExtinct);
        }

        static List<(Group Attacker, Group Target)> SelectTargets(List<Group> attackers)
        {
            attackers = attackers.OrderByDescending(g => g.EffectivePower * 1000 + g.Initiative).ToList();
            var eligibleTargets = new List<Group>(attackers);
            var selectedTargets = new List<(Group Attacker, Group Target)>();

            foreach (var attacker in attackers)
            {
                var possibleTargets = new List<Group>();
                var maxDamage = int.MinValue;

                foreach (var target in eligibleTargets.Where(g => g.GroupType != attacker.GroupType).ToList())
                {
                    var possibleDamage = attacker.EffectiveDamage(target);
                    if (possibleDamage == maxDamage)
                    {
                        possibleTargets.Add(target);
                    }
                    else if (possibleDamage > maxDamage)
                    {
                        possibleTargets.Clear();
                        possibleTargets.Add(target);
                        maxDamage = possibleDamage;
                    }
                }

                if (maxDamage > 0)
                {
                    var finalTarget = possibleTargets
                        .OrderByDescending(g => g.EffectivePower * 1000 + g.Initiative)
                        .First();
                    selectedTargets.Add((attacker, finalTarget));
                    eligibleTargets.Remove(finalTarget);
                }
            }

            return selectedTargets;
        }

        class Group
        {
            public int ID { get; set; }
            public GroupType GroupType { get; set; }
            public int Units { get; set; }
            public int HitPoints { get; set; }
            public int AttackDamage { get; set; }
            public AttackType AttackType { get; set; }
            public int Initiative { get; set; }
            public List<AttackType> Immunities { get; } = new List<AttackType>();
            public List<AttackType> Weaknesses { get; } = new List<AttackType>();
            public bool IsExtinct => Units <= 0;

            public int EffectivePower => Units * AttackDamage;

            public int EffectiveDamage(Group enemy)
            {
                if (enemy.Immunities.Contains(AttackType))
                {
                    return 0;
                }

                if (enemy.Weaknesses.Contains(AttackType))
                {
                    return 2 * EffectivePower;
                }

                return EffectivePower;
            }

            public void Attack(Group enemy)
            {
                enemy.Defend(EffectiveDamage(enemy));
            }

            public void Defend(int damage)
            {
                Units -= damage / HitPoints;

                if (Units < 0)
                    Units = 0;
            }
        }

        enum GroupType
        {
            Immune,
            Infection
        }

        enum AttackType
        {
            Bludgeoning,
            Cold,
            Fire,
            Radiation,
            Slashing
        }
    }
}
