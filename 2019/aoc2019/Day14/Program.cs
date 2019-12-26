using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day14
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt").Select(Parse);

            Console.WriteLine(Part1(input));

            Console.WriteLine(Part2(input));

            Console.Read();
        }

        static long Part1(IEnumerable<(IEnumerable<(string chemical, long quantity)> reactants, (string chemical, long quantity) product)> input)
        {
            return Produce(input, 1);
        }

        static long Part2(IEnumerable<(IEnumerable<(string chemical, long quantity)> reactants, (string chemical, long quantity) product)> input)
        {
            var availableOre = 1_000_000_000_000L;
            var orePerFuel = Produce(input, 1);
            var lowerBound = (long)((double)availableOre / orePerFuel);
            var upperBound = lowerBound * 2;
            while (lowerBound < upperBound)
            {
                var middle = (upperBound + lowerBound) / 2;
                var consumedOre = Produce(input, middle);
                if (consumedOre > availableOre)
                {
                    upperBound = middle;
                }
                else
                {
                    lowerBound = middle;
                    if (Produce(input, middle + 1) > availableOre)
                        break;
                }
            }

            return lowerBound;

            //var fuel = 1L;
            //while (true)
            //{
            //    var fuelProduced = (long)Math.Round((double)availableOre / Produce(input, fuel) * fuel);
            //    if (fuelProduced == fuel)
            //    {
            //        // converged
            //        return fuel;
            //    }
            //    fuel = fuelProduced;
            //}
        }

        private static long Produce(IEnumerable<(IEnumerable<(string chemical, long quantity)> reactants, (string chemical, long quantity) product)> input, long requestedQuantity)
        {
            var reactions = input.ToDictionary(reaction => reaction.product.chemical, reaction => reaction);
            var ore = 0L;
            var reactantStock = reactions.ToDictionary(r => r.Key, r => 0L);
            var chemicalsToProduce = new Queue<(string chemical, long quantity)>();
            chemicalsToProduce.Enqueue(("FUEL", requestedQuantity));

            while (chemicalsToProduce.Any())
            {
                var (chemical, quantity) = chemicalsToProduce.Dequeue();
                if (chemical == "ORE")
                {
                    ore += quantity;
                    continue;
                }

                var reaction = reactions[chemical];
                var existingStockUsed = reactantStock[chemical] < quantity ? reactantStock[chemical] : quantity;
                quantity -= existingStockUsed;
                reactantStock[chemical] -= existingStockUsed;

                if (quantity > 0)
                {
                    var factor = (long)Math.Ceiling((decimal)quantity / reaction.product.quantity);
                    var produced = factor * reaction.product.quantity;

                    foreach (var reactant in reaction.reactants)
                    {
                        chemicalsToProduce.Enqueue((reactant.chemical, reactant.quantity * factor));
                    }

                    reactantStock[chemical] = produced - quantity;
                }
            }

            return ore;
        }

        private static (IEnumerable<(string chemical, long quantity)> reactants, (string chemical, long quantity) product) Parse(string line)
        {
            var inout = line.Split(" => ");
            var reactants = inout[0].Split(", ", StringSplitOptions.RemoveEmptyEntries).Select(ParseReactant);
            var product = ParseReactant(inout[1]);
            return (reactants.ToArray(), product);
        }

        private static (string chemical, long quantity) ParseReactant(string input)
        {
            var parts = input.Split(' ');
            return (parts[1], long.Parse(parts[0]));
        }
    }
}
