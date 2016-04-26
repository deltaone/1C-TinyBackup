using System;
using System.IO;
using System.Collections.Generic;

//using System.Reflection;

namespace Core
{
    static class HelpersString
    {
        /// var i = task["depth"].TryParse(7);
        /// var i = task["depth"].TryParse<int>();
        public static T Parse<T>(this string input, T defaultValue = default(T))
        {
            try
            {
                return ((T)Convert.ChangeType(input, typeof(T)));                
            }
            catch
            {
                return (defaultValue);
            }
        }
        
        public static bool IsEmpty(this string input)
        {   // bool isTrulyEmpty = String.IsNullOrWhiteSpace(source); // DOTNET4        
            if (String.IsNullOrEmpty(input) || input.Trim().Length == 0) return (true);
            return (false);
        }
        
        public static string Clean(this string input, bool onlyCRLF = false)
        {
            input = input.Replace('\n', ' ').Replace("\r", "");
            if (onlyCRLF) return (input);
            return (input.Replace('\t', ' '));
        }

        public static string TrimCRLF(this string input)
        {
            return (input.Trim('\r', '\n')); //return (str.TrimEnd('\r', '\n'));
        }

        public static string Reverse(this string input)
        {
            char[] a = input.ToCharArray();
            Array.Reverse(a);
            return (new string(a));
        }

        public static bool IsASCII(this string input)
        {
            foreach (char ch in input)
            {
                if (ch > 0xff)
                {
                    return false;
                }
            }
            return true;
        }
        
        public static bool WildcardMatch(this string input, string wildcompare, bool ignoreCase)
        {
            if (ignoreCase)
                return input.ToLower().WildcardMatch(wildcompare.ToLower());
            else
                return input.WildcardMatch(wildcompare);
        }

        public static bool WildcardMatch(this string input, string wildcompare)
        {
            if (string.IsNullOrEmpty(wildcompare))
                return input.Length == 0;

            // workaround: *.* should get all
            wildcompare = wildcompare.Replace("*.*", "*");

            int pS = 0;
            int pW = 0;
            int lS = input.Length;
            int lW = wildcompare.Length;

            while (pS < lS && pW < lW && wildcompare[pW] != '*')
            {
                char wild = wildcompare[pW];
                if (wild != '?' && wild != input[pS])
                    return false;
                pW++;
                pS++;
            }

            int pSm = 0;
            int pWm = 0;
            while (pS < lS && pW < lW)
            {
                char wild = wildcompare[pW];
                if (wild == '*')
                {
                    pW++;
                    if (pW == lW)
                        return true;
                    pWm = pW;
                    pSm = pS + 1;
                }
                else if (wild == '?' || wild == input[pS])
                {
                    pW++;
                    pS++;
                }
                else
                {
                    pW = pWm;
                    pS = pSm;
                    pSm++;
                }
            }
            while (pW < lW && wildcompare[pW] == '*')
                pW++;
            return pW == lW && pS == lS;
        }
    }

    static class HelpersOther
    {
        public static void Swap<T>(this List<T> list, int index1, int index2)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        static public bool IsBetween(this TimeSpan time, TimeSpan startTime, TimeSpan endTime)
        {
            if (endTime == startTime) return (true);
            if (endTime < startTime) return (time <= endTime || time >= startTime);
            return (time >= startTime && time <= endTime);
        }
    }
}
