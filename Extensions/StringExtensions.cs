using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true if the strings are equal. Ignores case, and either string may be null.
        /// </summary>
        /// <param name="firstString">The first string to compare.</param>
        /// <param name="secondString">The second string to compare.</param>
        public static bool IgnoreCaseEquals(this string firstString, string secondString)
        {
            if (firstString == null && secondString == null) return true;
            if (firstString == null || secondString == null) return false;

            bool result = firstString.Equals(secondString, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Returns true if the first string starts with the second string. Ignores case.
        /// </summary>
        /// <param name="firstString">The first string to compare.</param>
        /// <param name="secondString">The second string to compare.</param>
        public static bool IgnoreCaseStartsWith(this string firstString, string secondString)
        {
            bool result = firstString.StartsWith(secondString, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Returns true if the first string ends with the second string. Ignores case.
        /// </summary>
        /// <param name="firstString">The first string to compare.</param>
        /// <param name="secondString">The second string to compare.</param>
        public static bool IgnoreCaseEndsWith(this string firstString, string secondString)
        {
            bool result = firstString.EndsWith(secondString, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

        /// <summary>
        /// Returns true if a string contains the given substring, ignoring case
        /// </summary>
        /// <param name="str">The string</param>
        /// <param name="substr">The substring</param>
        /// <returns>True if the string contains the substring (ignoring case), false otherwise.</returns>
        public static bool IgnoreCaseContains(this string str, string substr)
        {
            return str.IndexOf(substr, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        /// <summary>
        /// Easy method to replace ' ' and '-' with '_' for Column Field Names
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string FormatStringForColumnFieldName(this string s)
        {
            if (String.IsNullOrWhiteSpace(s))
            {
                return "UNKNOWN";
            }
            else
            {
                s = Regex.Replace(s, @"[^\w]", "_");
                if (Regex.IsMatch(s, @"^[^a-zA-Z]"))
                {
                    s = "_" + s;
                }
                return s.ToLower();
            }
        }

        /// <summary>
        /// Returns true if a string is null or empty.
        /// </summary>
        public static bool IsEmpty(this string s)
        {
            return String.IsNullOrEmpty(s);
        }

        public static bool IsLetter(this string s)
        {
            if (s.IsEmpty()) return false;
            return Regex.IsMatch(s, @"^[a-zA-Z]+");
        }

        /// <summary>
        /// Returns true if a string is not null and not empty.
        /// </summary>
        public static bool IsNotEmpty(this string s)
        {
            return !String.IsNullOrEmpty(s);
        }

        /// <summary>
        /// I wrote this because one of my daily development goals is to make people
        /// smile when they read my code...
        /// </summary>
        public static bool IsNotNull(this object o)
        {
            return !(o == null);
        }

        public static bool IsNull(this object o)
        {
            return (o == null || o == DBNull.Value);
        }
        /// <summary>
        /// If the main string is not null or empty, returns the main string.
        /// Otherwise, returns the alternative.
        /// </summary>
        /// <param name="alternative">The string that will be used if the main string is null or empty.</param>
        public static string OrIfEmpty(this string main, string alternative)
        {
            if (!String.IsNullOrEmpty(main)) return main;

            return alternative;
        }
        /// <summary>
        /// Checks if a value type(struct) is equal to its default value.
        /// This makes life easy to apply null check on struct types like KeyValuePair"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefault<T>(this T value) where T : struct
        {
            bool isDefault = value.Equals(default(T));

            return isDefault;
        }
        /// <summary>
        /// Gets the a substring consisting of the last N chars of the string.
        /// </summary>
        /// <param name="numChars">The number of chars to take from the end.</param>
        public static string Right(this string s, int numChars)
        {
            if (s == null) return null;
            if (s.Length <= numChars) return s;
            string right = s.Substring(s.Length - numChars, numChars);
            return right;
        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified word are replaced with another word.
        /// </summary>
        /// <param name="str">The string containing the word(s) to be replaced.</param>
        /// <param name="oldWord">The word to be replaced.</param>
        /// <param name="newWord">The replacement word.</param>
        /// <param name="ignoreCase">True to ignore case during the comparison; otherwise, false. (Optional. Default value: false.)</param>
        /// <returns>The original string with all occurences of the old word replaced by the new.</returns>
        public static string ReplaceWord(this string str, string oldWord, string newWord, bool ignoreCase = false)
        {
            return Regex.Replace(str,
                                 "\\b" + oldWord + "\\b",
                                 newWord,
                                 RegexOptions.CultureInvariant | (ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None));
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in a string of any
        /// character in a specified string of characters.</summary>
        /// <param name="s">The string being searched.</param>
        /// <param name="anyOf">A string containing one or more characters to seek.</param>
        /// <returns>The zero-based index position of the first occurrence in the string where
        ///          any character in anyOf was found; -1 if no character in anyOf was found.</returns>
        public static int IndexOfAny(this string s, string anyOf)
        {
            int foundIndex = -1;

            for (int anyOfIndex = 0; anyOfIndex < anyOf.Length; anyOfIndex++)
            {
                int thisIndex = s.IndexOf(anyOf[anyOfIndex]);

                if (thisIndex == 0)
                {
                    foundIndex = 0;
                    break;
                }
                else if (thisIndex > 0)
                {
                    foundIndex = foundIndex < 0
                                    ? thisIndex
                                    : Math.Min(foundIndex, thisIndex);
                }
            }

            return foundIndex;
        }

        /// <summary>
        /// Removes whitespace characters from a string
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>The string with whitespace characters removed</returns>
        public static string RemoveWhiteSpace(this string s)
        {
            if (s != null)
            {
                StringBuilder sb = new StringBuilder();

                foreach (char c in s)
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        sb.Append(c);
                    }
                }

                s = sb.ToString();
            }

            return s;
        }

        /// <summary>
        /// Replace whitespace characters in a string with another character.
        /// </summary>
        /// <param name="s">The string containing whitespace characters to be replaced.</param>
        /// <param name="replacementChar">[Optional] The character to substitute for whitespace characters.
        ///                               If omitted, the underscore ('_') character is used.</param>
        /// <returns>The string with the whitespace characters replaced by replacementChar.</returns>
        public static string ReplaceWhiteSpace(this string s, char replacementChar = '_')
        {
            if (s != null)
            {
                StringBuilder sb = new StringBuilder();

                foreach (char c in s)
                {
                    sb.Append(char.IsWhiteSpace(c) ? replacementChar : c);
                }

                s = sb.ToString();
            }

            return s;
        }

        /// <summary>
        /// Parses an enum value from a string
        /// </summary>
        /// <typeparam name="TEnum">The data type of the enum</typeparam>
        /// <param name="enumValueName">The representation of the enumeration name or underlying value to convert</param>
        /// <param name="defaultValue">[Optional] The value to return if the parse operation fails.
        ///                                       If omitted, the default value for TEnum is used.</param>
        /// <param name="ignoreCase">[Optional] true to ignore case, false to consider case.
        ///                                     If omitted, false is used.</param>
        /// <returns>The parsed enum value</returns>
        public static TEnum ParseEnum<TEnum>(this string enumValueName,
                                             TEnum defaultValue = default(TEnum),
                                             bool ignoreCase = false)
            where TEnum : struct
        {
            TEnum parsedValue = defaultValue;

            TEnum tempValue;

            if (Enum.TryParse(enumValueName, ignoreCase, out tempValue))
            {
                parsedValue = tempValue;
            }

            return parsedValue;
        }

        public static Guid ConvertToGuid(this string str)
        {
            // we just need to create a deterministic guid from the string
            byte[] newGuidValues = new byte[16];
            for (int i = 0; (i <= 15) && (i < str.Length); i++)
            {
                newGuidValues[i] = Convert.ToByte(str[i]);
            }
            Guid ret = new Guid(newGuidValues);
            return ret;
        }
    }
}
