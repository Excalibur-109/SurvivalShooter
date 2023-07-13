using System;
using System.Collections.Generic;

namespace Excalibur.Algorithms
{
    /*
    * KMP算法（Knuth-Morris-Pratt算法）是一种字符串匹配算法，用于在一个文本串中查找一个模式串的出现位置。相比于朴素的字符串匹配算法，KMP算法具有更高的效率，特别适用于大文本和长模式串的匹配。
    KMP算法的核心思想是利用模式串本身的信息来避免不必要的字符比较。它通过构建一个部分匹配表（Partial Match Table，简称PMT或next数组），来提前计算模式串中每个位置的最长公共前缀后缀长度。通过这个部分匹配表，算法能够根据已匹配的字符，快速地调整模式串的比较位置，从而减少不必要的字符比较次数。

    下面是KMP算法的详细步骤：

    构建部分匹配表（next数组）：
    部分匹配表记录了模式串中每个位置的最长公共前缀后缀长度。具体构建过程如下：

    初始化next数组，next[0] = -1，next[1] = 0；
    从位置2开始遍历模式串，维护两个指针：i表示当前位置，j表示最长公共前缀后缀的长度；
    若模式串的第i个字符与前j个字符匹配，则next[i] = j + 1，然后i++，j++；
    若模式串的第i个字符与前j个字符不匹配：
    若j为0，则next[i] = 0，i++；
    若j不为0，则将j更新为next[j-1]，继续匹配。
    在文本串中查找模式串：
    利用构建好的部分匹配表，进行文本串与模式串的匹配过程，具体步骤如下：

    初始化文本串指针i和模式串指针j，分别指向第一个字符；
    若文本串的第i个字符与模式串的第j个字符匹配，则i++，j++；
    若模式串的指针j达到末尾（即完全匹配），则找到了一个匹配的位置；
    若文本串的第i个字符与模式串的第j个字符不匹配：
    若j为0，则文本串指针i后移一位，继续匹配；
    若j不为0，则将模式串指针j更新为next[j-1]，继续匹配。
    重复步骤2，直到找到所有的匹配位置或遍历完整个文本串。

    KMP算法的时间复杂度为O(m+n)，其中m为模式串的长度，n为文本串的长度。相比于朴素的字符串匹配算法，KMP算法减少了不必要的字符比较，提高了匹配效率。因此，KMP算法在字符串匹配问题中得到广泛应用。
    */
    public static class KMP
    {
        public static int Search(string text, string pattern)
        {
            int[] pmt = _CalculatePMT(pattern);

            int
                textIndex = 0,
                patternIndex = 0,
                textLength = text.Length,
                patternLength = pattern.Length;

            while (textIndex < textLength && patternIndex < patternLength)
            {
                if (patternIndex == -1 || text[textIndex] == pattern[patternIndex])
                {
                    textIndex++;
                    patternIndex++;
                }
                else
                {
                    patternIndex = pmt[patternIndex];
                }
            }

            if (patternIndex == patternLength)
            {
                return textIndex - pattern.Length;
            }

            return -1;
        }

        private static int[] _CalculatePMT(string pattern)
        {
            int[] pmt = new int[pattern.Length];
            pmt[0] = -1;

            int
                i = 0,
                j = -1;

            while (i < pattern.Length - 1)
            {
                if (j == -1 || pattern[i] == pattern[j])
                {
                    i++;
                    j++;

                    if (pattern[i] != pattern[j])
                    {
                        pmt[i] = j;
                    }
                    else
                    {
                        pmt[i] = pmt[j];
                    }
                }
                else
                {
                    j = pmt[j];
                }
            }

            return pmt;
        }
    }

    /*
     Sunday算法是一种高效的字符串匹配算法，用于在文本串中查找模式串的出现位置。它的思想简单直观，通过预处理模式串和文本串的字符来确定每次比较的位置，从而减少比较次数，提高匹配效率。
    下面是Sunday算法的理论原理：
    预处理模式串：
    记录模式串中每个字符最右出现的位置。
    若字符在模式串中多次出现，则记录最右边的位置。
    在文本串中查找模式串：
    从文本串的起始位置开始，逐个比较模式串和文本串的对应字符。
    若匹配成功，则继续比较下一个字符。
    若匹配失败，则根据文本串中当前比较的字符的下一个字符来决定下一次比较的位置：
    若当前字符的下一个字符在模式串中不存在，则将模式串移动到当前字符的下一个位置。
    否则，将模式串右移，使模式串的最右边字符与文本串中的当前字符对齐。
    重复步骤2，直到找到所有的匹配位置或遍历完整个文本串。
    Sunday算法的时间复杂度为O(n+m)，其中n为文本串的长度，m为模式串的长度。它具有较好的平均性能，在实践中被广泛应用
     */
    public class Sunday
    {
        public static List<int> SundaySearch(string text, string pattern)
        {
            List<int> indices = new List<int>();
            int n = text.Length;
            int m = pattern.Length;
            int[] rightmost = BuildRightmostTable(pattern);

            int i = 0;
            while (i <= n - m)
            {
                int j = 0;
                while (j < m && pattern[j] == text[i + j])
                {
                    j++;
                }

                if (j == m)
                {
                    indices.Add(i);
                }

                if (i + m >= n)
                {
                    break;
                }

                char nextChar = text[i + m];
                int shift = m - rightmost[nextChar];

                i += shift;
            }

            return indices;
        }

        private static int[] BuildRightmostTable(string pattern)
        {
            int[] rightmost = new int[256];
            int m = pattern.Length;

            for (int i = 0; i < rightmost.Length; i++)
            {
                rightmost[i] = -1;
            }

            for (int i = 0; i < m; i++)
            {
                rightmost[pattern[i]] = i;
            }

            return rightmost;
        }

        public static void Main(string[] args)
        {
            string text = "ABABDABACDABABCABAB";
            string pattern = "ABABCABAB";

            List<int> matchIndices = SundaySearch(text, pattern);

            if (matchIndices.Count > 0)
            {
                Console.WriteLine("Pattern found at indices:");
                foreach (int index in matchIndices)
                {
                    Console.WriteLine(index);
                }
            }
            else
            {
                Console.WriteLine("Pattern not found.");
            }
        }
    }

    /*
     Boyer-Moore算法是一种高效的字符串搜索算法，用于在一个文本串中查找一个模式串的出现位置。相比于朴素的字符串匹配算法，Boyer-Moore算法通常具有更高的效率，特别适用于大文本和长模式串的匹配。

    Boyer-Moore算法的核心思想是根据模式串的字符比较结果，利用两个启发式规则来跳过尽量多的文本串中的字符，从而减少比较次数。

    下面是Boyer-Moore算法的主要步骤：

    预处理模式串：

    计算模式串中每个字符的最右出现位置（rightmost occurrence）。
    计算模式串的好后缀规则（good suffix rule），以便在匹配过程中能够跳过一些字符。
    在文本串中查找模式串：

    从文本串的末尾开始与模式串的末尾进行比较。
    若当前字符匹配，则继续向前比较，直到完全匹配或不匹配。
    若不匹配，则根据好后缀规则和最右出现位置来决定跳过的字符数。
    Boyer-Moore算法通过预处理模式串，利用最右出现位置和好后缀规则，能够跳过尽量多的文本串中的字符，从而提高匹配效率。它的时间复杂度通常为O(n+m)，其中n为文本串的长度，m为模式串的长度。

    Boyer-Moore算法在实践中被广泛应用，是一种性能优异的字符串搜索算法。

    请注意，Boyer-Moore算法的具体实现较为复杂，包含了对模式串的预处理、好后缀规则和最右出现位置的计算等。详细的实现代码超出了本回答的篇幅，您可以查阅相关文献或参考现有的Boyer-Moore算法的实现库和资料。
    */
    //public static class BoyerMoore
    //{
    //    public static List<int> BoyerMooreSearch(string text, string pattern)
    //    {
    //        List<int> indices = new List<int>();
    //        int n = text.Length;
    //        int m = pattern.Length;
    //        int[] badCharTable = BuildBadCharTable(pattern);
    //        int[] goodSuffixTable = BuildGoodSuffixTable(pattern);

    //        int i = 0;
    //        while (i <= n - m)
    //        {
    //            int j = m - 1;
    //            while (j >= 0 && pattern[j] == text[i + j])
    //            {
    //                j--;
    //            }

    //            if (j < 0)
    //            {
    //                indices.Add(i);
    //                i += (i + m < n) ? m - badCharTable[text[i + m]] : 1;
    //            }
    //            else
    //            {
    //                i += Math.Max(goodSuffixTable[j], j - badCharTable[text[i + j]]);
    //            }
    //        }

    //        return indices;
    //    }

    //    private static int[] BuildBadCharTable(string pattern)
    //    {
    //        int[] table = new int[256];

    //        for (int i = 0; i < table.Length; i++)
    //        {
    //            table[i] = -1;
    //        }

    //        for (int i = 0; i < pattern.Length; i++)
    //        {
    //            table[pattern[i]] = i;
    //        }

    //        return table;
    //    }

    //    private static int[] BuildGoodSuffixTable(string pattern)
    //    {
    //        int m = pattern.Length;
    //        int[] table = new int[m];
    //        int[] suffixes = new int[m];

    //        Array.Fill(table, -1);

    //        ComputeSuffixes(pattern, suffixes);
    //        ComputeCase1(pattern, suffixes, table);
    //        ComputeCase2(pattern, suffixes, table);

    //        return table;
    //    }

    //    private static void ComputeSuffixes(string pattern, int[] suffixes)
    //    {
    //        int m = pattern.Length;
    //        int f = 0, g = m - 1;

    //        suffixes[m - 1] = m;

    //        for (int i = m - 2; i >= 0; i--)
    //        {
    //            if (i > g && suffixes[i + m - 1 - f] < i - g)
    //            {
    //                suffixes[i] = suffixes[i + m - 1 - f];
    //            }
    //            else
    //            {
    //                if (i < g)
    //                {
    //                    g = i;
    //                }

    //                f = i;
    //                while (g >= 0 && pattern[g] == pattern[g + m - 1 - f])
    //                {
    //                    g--;
    //                }

    //                suffixes[i] = f - g;
    //            }
    //        }
    //    }

    //    private static void ComputeCase1(string pattern, int[] suffixes, int[] table)
    //    {
    //        int m = pattern.Length;
    //        int lastPrefixIndex = -1;

    //        for (int i = m - 2; i >= 0; i--)
    //        {
    //            if (suffixes[i] == i + 1)
    //            {
    //                while (lastPrefixIndex >= m - 1 - i)
    //                {
    //                    if (table[m - 1 - lastPrefixIndex] == -1)
    //                    {
    //                        table[m - 1 - lastPrefixIndex] = m - 1 - i;
    //                    }

    //                    lastPrefixIndex--;
    //                }
    //            }
    //        }
    //    }

    //    private static void ComputeCase2(string pattern, int[] suffixes, int[] table)
    //    {
    //        int m = pattern.Length;
    //        int lastPrefixIndex = m - 1;

    //        for (int i = m - 2; i >= 0; i--)
    //        {
    //            if (suffixes[i] == i + 1)
    //            {
    //                lastPrefixIndex = i;
    //            }

    //            table[m - 1 - suffixes[i]] = m - 1 - i;
    //        }

    //        for (int i = 0; i <= m - 2; i++)
    //        {
    //            table[i] = Math.Min(table[i], m - 1 - lastPrefixIndex + i);
    //        }
    //    }

    //    public static void Main(string[] args)
    //    {
    //        string text = "ABABDABACDABABCABAB";
    //        string pattern = "ABABCABAB";

    //        List<int> matchIndices = BoyerMooreSearch(text, pattern);

    //        if (matchIndices.Count > 0)
    //        {
    //            Console.WriteLine("Pattern found at indices:");
    //            foreach (int index in matchIndices)
    //            {
    //                Console.WriteLine(index);
    //            }
    //        }
    //        else
    //        {
    //            Console.WriteLine("Pattern not found.");
    //        }
    //    }
    //}
}