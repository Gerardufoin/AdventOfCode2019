using System;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// It is possible to do this exercice by bruteforcing and testing all the possible passwords, but where would be the fun in that ?
/// This program increments the password so it is always correct (at least for Part 1).
/// To do that, a few things have to be done:
/// 1.  The starting password has to be correct. To do that, we check every digit. If one digit is less than the previous one, this digit and all the next ones take the value of the previous digit (154665 becomes 155555).
///     We also check the previous to last digit for double if all digit are already in an incrementing order and no double have been found, so 123456 would become 123466.
/// 2.  Once we have the right starting password, we keep in memory the second offset of the double position. (155522 would be 2). As long as the digit as this position doesn't increment, we don't need to check for double.
/// 3.  We can now start to increment the last digit of the password. If a 9 is reached at a any position, the previous digit is incremented by one and all the next digit become this digit. (Example: 155999 + 1 becomes 156666)
///     If the digit of the double offset is changed, offset + 1 become the new offset to keep in memory. (Example: 155999 + 1 (offset 2) will become 156666 (offset 3))
///     If the digit before the offset is changed, the offset becomes this digit + 1 (Example: 159999 + 1 (offset 3) will become 1666666 (offset 2))
///     If the last digit is the offset, then we increment the previous digit by one and carry on from the begining of point 3. That's the main use of the offset. (123455 + 1 (offset 5) should become 123456, but that would not be valid, so it has to become 123466 (offset unchanged at 5))
/// 4. We loop point 3 until the password is equal or above the ending password.
/// </summary>
namespace Day04
{
    class Program
    {
        /// <summary>
        /// Get the two password from the input.
        /// </summary>
        /// <param name="input">Input got in the file passed as parameter.</param>
        /// <returns>An array of two strings, one for each password.</returns>
        static string[] GetBasePasswords(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"^(\d{6})-(\d{6})$", RegexOptions.Singleline);
            if (matches.Count < 1)
            {
                Advent2019.Utils.FatalError("Couldn't find the two passwords in the input file.");
            }
            return new string[] { matches[0].Groups[1].Value, matches[0].Groups[2].Value };
        }

        /// <summary>
        /// Convert the password from string to an array of integers
        /// </summary>
        /// <param name="pswd">Password as a string</param>
        /// <returns>Password as an array of integers</returns>
        static int[] ConvertPassword(string pswd)
        {
            return pswd.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray();
        }

        /// <summary>
        /// Compute the next valid password based on the summary at the begining of this file
        /// </summary>
        /// <param name="pswd">The password to compute</param>
        /// <param name="didx">The current double offset</param>
        /// <returns></returns>
        static int NextValidPassword(ref int[] pswd, int didx)
        {
            int i = pswd.Length - 1;
            i = (i == didx ? i - 1 : i);
            pswd[i] += 1;
            while (i > 0 && pswd[i] > 9)
            {
                i--;
                pswd[i] += 1;
            }
            for (int j = i + 1; j < pswd.Length; ++j)
            {
                pswd[j] = pswd[j - 1];
            }
            return (i == didx || i + 1 < didx ? i + 1 : didx);
        }

        /// <summary>
        /// Compute a correct start for the starting password
        /// </summary>
        /// <param name="cpswd">Password</param>
        /// <returns>The double offset</returns>
        static int SetValidStart(ref int[] cpswd)
        {
            int didx = -1;
            int prev = cpswd[0];
            for (int i = 1; i < cpswd.Length; ++i)
            {
                if (cpswd[i] < prev)
                {
                    for (int j = i; j < cpswd.Length; ++j)
                    {
                        cpswd[j] = prev;
                    }
                    didx = i;
                    break;
                }
                prev = cpswd[i];
            }
            return (didx >= 0 ? didx : NextValidPassword(ref cpswd, cpswd.Length - 1));
        }

        /// <summary>
        /// Check if the program has to stop
        /// </summary>
        /// <param name="cpswd">Current password</param>
        /// <param name="epswd">Password limit</param>
        /// <param name="p1">Reference to the number of right passwords for part 1</param>
        /// <param name="p2">Reference to the number of right passwords for part 2</param>
        /// <returns></returns>
        static bool StopCondition(int[] cpswd, int[] epswd, ref int p1, ref int p2)
        {
            for (int i = 0; i < cpswd.Length; ++i)
            {
                if (cpswd[i] != epswd[i])
                {
                    return (cpswd[i] > epswd[i]);
                }
            }
            // If the two passwords match, we have to end the program but also have to count them
            p1++;
            p2 += Part2Validator(ref cpswd);
            return true;
        }

        /// <summary>
        /// Get all the possible passwords between pswd1 and pswd2
        /// </summary>
        /// <param name="pswd1">The starting password</param>
        /// <param name="pswd2">The ending password</param>
        /// <param name="p1">Reference to the number of right passwords for part 1</param>
        /// <param name="p2">Reference to the number of right passwords for part 2</param>
        static void GetPossibleCombinationNumber(string pswd1, string pswd2, ref int p1, ref int p2)
        {
            int[] cpswd = ConvertPassword(pswd1);
            int[] epswd = ConvertPassword(pswd2);
            int didx = SetValidStart(ref cpswd);
            p1 = 0;
            p2 = 0;
            while (!StopCondition(cpswd, epswd, ref p1, ref p2))
            {
                p1++;
                p2 += Part2Validator(ref cpswd);
                didx = NextValidPassword(ref cpswd, didx);
            }
        }

        /// <summary>
        /// Check if a password form part 1 is also valid from part 2 (as all password from part 2 are included in part 1).
        /// </summary>
        /// <param name="cpswd">Current password.</param>
        /// <returns>1 if the password if valid, 0 otherwise.</returns>
        static int Part2Validator(ref int[] cpswd)
        {
            int d = 1;
            for (int i = 1; i < cpswd.Length; ++i)
            {
                if (cpswd[i] == cpswd[i - 1])
                {
                    d++;
                }
                else
                {
                    if (d == 2)
                    {
                        return 1;
                    }
                    d = 1;
                }
            }
            return (d == 2 ? 1 : 0);
        }

        static void Main(string[] args)
        {
            string[] passwords = GetBasePasswords(Advent2019.Utils.GetInput(args));
            int p1 = 0;
            int p2 = 0;
            GetPossibleCombinationNumber(passwords[0], passwords[1], ref p1, ref p2);
            Console.WriteLine("Part 1: " + p1);
            Console.WriteLine("Part 2: " + p2);
        }
    }
}
