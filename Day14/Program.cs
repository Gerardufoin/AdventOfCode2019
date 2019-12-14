using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Day14
{
    class Program
    {
        /// <summary>
        /// Get the ingredients of a specific reaction.
        /// </summary>
        /// <param name="ingredientStr">String of ingredients directely taken from the input</param>
        /// <returns>A list of incredients comprised of a name and a quantity</returns>
        static List<(string chemical, int amount)> GetIngredients(string ingredientStr)
        {
            MatchCollection matches = Regex.Matches(ingredientStr, @"(\d+) (\w+)");
            List<(string chemical, int amount)> ingredients = new List<(string chemical, int amount)>();
            foreach (Match match in matches)
            {
                for (int i = 1; i < match.Groups.Count; i += 2)
                {
                    if (match.Groups[i].Value != "" && match.Groups[i + 1].Value != "")
                    {
                        ingredients.Add((match.Groups[i + 1].Value, int.Parse(match.Groups[i].Value)));
                    }
                }
            }
            return ingredients;
        }

        /// <summary>
        /// Parse the input and return a dictionary of chemical reactions.
        /// </summary>
        /// <param name="input">The text found in the input file</param>
        /// <returns>A dictionary of all the chemical reactions</returns>
        static Dictionary<string, (int amount, List<(string chemical, int amount)> ingredients)> GetReactions(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"(\d+ \w+(?:, \d+ \w+)*) => (\d+) (\w+)");
            var reactions = new Dictionary<string, (int amount, List<(string chemical, int amount)> ingredients)>();
            foreach (Match match in matches)
            {
                string result = match.Groups[3].Value;
                if (reactions.ContainsKey(result))
                {
                    Advent2019.Utils.FatalError("Two formulas given to create '" + result + "'.");
                }
                reactions.Add(result, (int.Parse(match.Groups[2].Value), GetIngredients(match.Groups[1].Value)));
            }
            return reactions;
        }

        #region Part 1
        /// <summary>
        /// Recursive. Give the amount of ore needed to get an amount of component based on its chemical formula.
        /// </summary>
        /// <param name="component">The component we are trying to create</param>
        /// <param name="amount">The amount of component we need</param>
        /// <param name="stocks">The leftovers ingredients of previous recursions</param>
        /// <param name="reactions">The list of chemical reactions</param>
        /// <returns>The amount of ore needed to create this component</returns>
        static double GetOreForComponent(string component, double amount, Dictionary<string, double> stocks, Dictionary<string,(int amount, List<(string chemical, int amount)> ingredients)> reactions)
        {
            double ore = 0;
            if (!reactions.ContainsKey(component))
            {
                Advent2019.Utils.FatalError("No chemical reaction found to create '" + component + "'.");
            }
            if (!stocks.ContainsKey(component))
            {
                stocks.Add(component, 0);
            }
            if (amount == 0 || stocks[component] >= amount)
            {
                stocks[component] -= amount;
                return 0;
            }
            amount -= stocks[component];
            double mult = Math.Ceiling(amount / reactions[component].amount);
            stocks[component] = 0;
            foreach ((string chemical, int amount) needs in reactions[component].ingredients)
            {
                ore += (needs.chemical == "ORE" ? needs.amount * mult : GetOreForComponent(needs.chemical, needs.amount * mult, stocks, reactions));
            }
            stocks[component] = (int)(reactions[component].amount * mult - amount);
            return ore;
        }
        #endregion

        #region Part 2
        /// <summary>
        /// Compute how much fuel can be created from a specific amount of ore.
        /// As all the reactions have leftovers that have to be used in order to be efficient, it's easier to check how much fuel you get from a set amount of ore and compare it with the result.
        /// To optimize it, we first take the amount of fuel created by a simple division between searched ore and default ore as a base. It will always be lower than the actual result due to the leftovers not being used correctly.
        /// Then we decide on an arbitrary amount of steps the fuel is going to be incremented by with each loop (1000000 produced good results here).
        /// Based on if the amount of ore needed to process this amount of fuel is greater or lower than the amount asked, we increment/decrement the fuel and change the step accordingly.
        /// The amount of steps is going to vary between an upper and lower bound getting smaller and smaller. Once it reaches 1 and the amount of ore produced is lower than the searched amount, we have the most fuel possible.
        /// </summary>
        /// <param name="reactions"></param>
        /// <param name="ore"></param>
        /// <param name="defaultOre"></param>
        /// <returns></returns>
        static int GetFuelForOre(Dictionary<string, (int amount, List<(string chemical, int amount)> ingredients)> reactions, double ore, double defaultOre)
        {
            int fuel = (int)(ore / defaultOre);
            int step = 1000000;
            double currentOre = 0;
            int resultSign = 0;
            while ((currentOre = GetOreForComponent("FUEL", fuel, new Dictionary<string, double>(), reactions)) < ore || step > 1)
            {
                if (currentOre < ore)
                {
                    step = (resultSign > 0 ? (step + 1) / 2 : step);
                    fuel += step;
                    resultSign = -1;
                }
                else if (currentOre > ore)
                {
                    step = (resultSign < 0 ? (step + 1) / 2 : step);
                    fuel -= step;
                    resultSign = 1;
                }
                else
                {
                    return fuel;
                }
            }
            return fuel - 1;
        }
        #endregion

        static void Main(string[] args)
        {
            var reactions = GetReactions(Advent2019.Utils.GetInput(args));
            Dictionary<string, double> stocks = new Dictionary<string, double>();
            double defaultOre = GetOreForComponent("FUEL", 1, stocks, reactions);
            Console.WriteLine("Part 1: " + defaultOre);
            Console.WriteLine("Part 2: " + GetFuelForOre(reactions, 1000000000000, defaultOre));
        }
    }
}
