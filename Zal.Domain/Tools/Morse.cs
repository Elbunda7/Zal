using System;
using System.Collections.Generic;
using System.Text;

namespace Zal.Domain.Tools
{
    public static class Morse
    {
        private static Dictionary<char, string> morseTable = new Dictionary<char, string>
        {
            {'a', ".-"},
            {'b', "-..."},
            {'c', "-.-."},
            {'d', "-.."},
            {'e', "."},
            {'f', "..-."},
            {'g', "--."},
            {'h', "...."},
            {'i', ".."},
            {'j', ".---"},
            {'k', "-.-"},
            {'l', ".-.."},
            {'m', "--"},
            {'n', "-."},
            {'o', "---"},
            {'p', "..--"},
            {'q', "--.-"},
            {'r', ".-."},
            {'s', "..."},
            {'t', "-"},
            {'u', "..-"},
            {'v', "...-"},
            {'w', ".--"},
            {'x', "-..-"},
            {'y', "-.--"},
            {'z', "--.."},
            {'1', ".----"},
            {'2', "..---"},
            {'3', "...--"},
            {'4', "....-"},
            {'5', "....."},
            {'6', "-...."},
            {'7', "--..."},
            {'8', "---.."},
            {'9', "----."},
            {'0', "-----"},
        };

        public static string ToMorse(this char ch)
        {
            ch = char.ToLower(ch);
            if (morseTable.ContainsKey(ch))
            {
                return morseTable[ch];
            }
            return "";
        }

        public static string ToMorse(this string str)
        {
            str = str.ToLower();
            StringBuilder sb = new StringBuilder();
            foreach (char ch in str)
            {
                sb.Append(ch.ToMorse()).Append('/');
            }
            return sb.ToString();
        }
    }
}
