using System;
using System.Collections.Generic;

namespace Excalibur.Algorithms
{
    /*
    * KMP㷨Knuth-Morris-Pratt㷨һַƥ㷨һıвһģʽĳλáصַƥ㷨KMP㷨иߵЧʣرڴıͳģʽƥ䡣
    KMP㷨ĺ˼ģʽϢⲻҪַȽϡͨһƥPartial Match TablePMTnext飩ǰģʽÿλõǰ׺׺ȡͨƥ㷨ܹƥַٵصģʽıȽλãӶٲҪַȽϴ

    KMP㷨ϸ裺

    ƥnext飩
    ƥ¼ģʽÿλõǰ׺׺ȡ幹£

    ʼnext飬next[0] = -1next[1] = 0
    λ2ʼģʽάָ룺iʾǰλãjʾǰ׺׺ĳȣ
    ģʽĵiַǰjַƥ䣬next[i] = j + 1Ȼi++j++
    ģʽĵiַǰjַƥ䣺
    jΪ0next[i] = 0i++
    jΪ0jΪnext[j-1]ƥ䡣
    ıвģʽ
    ùõĲƥıģʽƥ̣岽£

    ʼıָiģʽָjֱָһַ
    ıĵiַģʽĵjַƥ䣬i++j++
    ģʽָjﵽĩβȫƥ䣩ҵһƥλã
    ıĵiַģʽĵjַƥ䣺
    jΪ0ıָiһλƥ䣻
    jΪ0ģʽָjΪnext[j-1]ƥ䡣
    ظ2ֱҵеƥλûı

    KMP㷨ʱ临ӶΪO(m+n)mΪģʽĳȣnΪıĳȡصַƥ㷨KMP㷨˲ҪַȽϣƥЧʡˣKMP㷨ַƥеõ㷺Ӧá
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
     Sunday㷨һָЧַƥ㷨ıвģʽĳλá˼ֱۣͨԤģʽıַȷÿαȽϵλãӶٱȽϴƥЧʡ
    Sunday㷨ԭ
    Ԥģʽ
    ¼ģʽÿַҳֵλá
    ַģʽжγ֣¼ұߵλá
    ıвģʽ
    ıʼλÿʼȽģʽıĶӦַ
    ƥɹȽһַ
    ƥʧܣıеǰȽϵַһַһαȽϵλã
    ǰַһַģʽвڣģʽƶǰַһλá
    򣬽ģʽƣʹģʽұַıеĵǰַ롣
    ظ2ֱҵеƥλûı
    Sunday㷨ʱ临ӶΪO(n+m)nΪıĳȣmΪģʽĳȡнϺõƽܣʵб㷺Ӧ
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
     Boyer-Moore㷨һָЧַ㷨һıвһģʽĳλáصַƥ㷨Boyer-Moore㷨ͨиߵЧʣرڴıͳģʽƥ䡣

    Boyer-Moore㷨ĺ˼ǸģʽַȽϽʽıеַӶٱȽϴ

    Boyer-Moore㷨Ҫ裺

    Ԥģʽ

    ģʽÿַҳλãrightmost occurrence
    ģʽĺú׺good suffix ruleԱƥܹһЩַ
    ıвģʽ

    ıĩβʼģʽĩβбȽϡ
    ǰַƥ䣬ǰȽϣֱȫƥƥ䡣
    ƥ䣬ݺú׺ҳλַ
    Boyer-Moore㷨ͨԤģʽҳλúͺú׺ܹıеַӶƥЧʡʱ临ӶͨΪO(n+m)nΪıĳȣmΪģʽĳȡ

    Boyer-Moore㷨ʵб㷺Ӧãһַ㷨

    ע⣬Boyer-Moore㷨ľʵֽΪӣ˶ģʽԤú׺ҳλõļȡϸʵִ볬˱شƪԲ׻οеBoyer-Moore㷨ʵֿϡ
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