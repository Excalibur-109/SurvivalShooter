using System;
using System.Collections.Generic;

namespace Excalibur.Algorithms
{
    /*
    * KMP�㷨��Knuth-Morris-Pratt�㷨����һ���ַ���ƥ���㷨��������һ���ı����в���һ��ģʽ���ĳ���λ�á���������ص��ַ���ƥ���㷨��KMP�㷨���и��ߵ�Ч�ʣ��ر������ڴ��ı��ͳ�ģʽ����ƥ�䡣
    KMP�㷨�ĺ���˼��������ģʽ���������Ϣ�����ⲻ��Ҫ���ַ��Ƚϡ���ͨ������һ������ƥ���Partial Match Table�����PMT��next���飩������ǰ����ģʽ����ÿ��λ�õ������ǰ׺��׺���ȡ�ͨ���������ƥ����㷨�ܹ�������ƥ����ַ������ٵص���ģʽ���ıȽ�λ�ã��Ӷ����ٲ���Ҫ���ַ��Ƚϴ�����

    ������KMP�㷨����ϸ���裺

    ��������ƥ���next���飩��
    ����ƥ����¼��ģʽ����ÿ��λ�õ������ǰ׺��׺���ȡ����幹���������£�

    ��ʼ��next���飬next[0] = -1��next[1] = 0��
    ��λ��2��ʼ����ģʽ����ά������ָ�룺i��ʾ��ǰλ�ã�j��ʾ�����ǰ׺��׺�ĳ��ȣ�
    ��ģʽ���ĵ�i���ַ���ǰj���ַ�ƥ�䣬��next[i] = j + 1��Ȼ��i++��j++��
    ��ģʽ���ĵ�i���ַ���ǰj���ַ���ƥ�䣺
    ��jΪ0����next[i] = 0��i++��
    ��j��Ϊ0����j����Ϊnext[j-1]������ƥ�䡣
    ���ı����в���ģʽ����
    ���ù����õĲ���ƥ��������ı�����ģʽ����ƥ����̣����岽�����£�

    ��ʼ���ı���ָ��i��ģʽ��ָ��j���ֱ�ָ���һ���ַ���
    ���ı����ĵ�i���ַ���ģʽ���ĵ�j���ַ�ƥ�䣬��i++��j++��
    ��ģʽ����ָ��j�ﵽĩβ������ȫƥ�䣩�����ҵ���һ��ƥ���λ�ã�
    ���ı����ĵ�i���ַ���ģʽ���ĵ�j���ַ���ƥ�䣺
    ��jΪ0�����ı���ָ��i����һλ������ƥ�䣻
    ��j��Ϊ0����ģʽ��ָ��j����Ϊnext[j-1]������ƥ�䡣
    �ظ�����2��ֱ���ҵ����е�ƥ��λ�û�����������ı�����

    KMP�㷨��ʱ�临�Ӷ�ΪO(m+n)������mΪģʽ���ĳ��ȣ�nΪ�ı����ĳ��ȡ���������ص��ַ���ƥ���㷨��KMP�㷨�����˲���Ҫ���ַ��Ƚϣ������ƥ��Ч�ʡ���ˣ�KMP�㷨���ַ���ƥ�������еõ��㷺Ӧ�á�
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
     Sunday�㷨��һ�ָ�Ч���ַ���ƥ���㷨���������ı����в���ģʽ���ĳ���λ�á�����˼���ֱ�ۣ�ͨ��Ԥ����ģʽ�����ı������ַ���ȷ��ÿ�αȽϵ�λ�ã��Ӷ����ٱȽϴ��������ƥ��Ч�ʡ�
    ������Sunday�㷨������ԭ��
    Ԥ����ģʽ����
    ��¼ģʽ����ÿ���ַ����ҳ��ֵ�λ�á�
    ���ַ���ģʽ���ж�γ��֣����¼���ұߵ�λ�á�
    ���ı����в���ģʽ����
    ���ı�������ʼλ�ÿ�ʼ������Ƚ�ģʽ�����ı����Ķ�Ӧ�ַ���
    ��ƥ��ɹ���������Ƚ���һ���ַ���
    ��ƥ��ʧ�ܣ�������ı����е�ǰ�Ƚϵ��ַ�����һ���ַ���������һ�αȽϵ�λ�ã�
    ����ǰ�ַ�����һ���ַ���ģʽ���в����ڣ���ģʽ���ƶ�����ǰ�ַ�����һ��λ�á�
    ���򣬽�ģʽ�����ƣ�ʹģʽ�������ұ��ַ����ı����еĵ�ǰ�ַ����롣
    �ظ�����2��ֱ���ҵ����е�ƥ��λ�û�����������ı�����
    Sunday�㷨��ʱ�临�Ӷ�ΪO(n+m)������nΪ�ı����ĳ��ȣ�mΪģʽ���ĳ��ȡ������нϺõ�ƽ�����ܣ���ʵ���б��㷺Ӧ��
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
     Boyer-Moore�㷨��һ�ָ�Ч���ַ��������㷨��������һ���ı����в���һ��ģʽ���ĳ���λ�á���������ص��ַ���ƥ���㷨��Boyer-Moore�㷨ͨ�����и��ߵ�Ч�ʣ��ر������ڴ��ı��ͳ�ģʽ����ƥ�䡣

    Boyer-Moore�㷨�ĺ���˼���Ǹ���ģʽ�����ַ��ȽϽ����������������ʽ������������������ı����е��ַ����Ӷ����ٱȽϴ�����

    ������Boyer-Moore�㷨����Ҫ���裺

    Ԥ����ģʽ����

    ����ģʽ����ÿ���ַ������ҳ���λ�ã�rightmost occurrence����
    ����ģʽ���ĺú�׺����good suffix rule�����Ա���ƥ��������ܹ�����һЩ�ַ���
    ���ı����в���ģʽ����

    ���ı�����ĩβ��ʼ��ģʽ����ĩβ���бȽϡ�
    ����ǰ�ַ�ƥ�䣬�������ǰ�Ƚϣ�ֱ����ȫƥ���ƥ�䡣
    ����ƥ�䣬����ݺú�׺��������ҳ���λ���������������ַ�����
    Boyer-Moore�㷨ͨ��Ԥ����ģʽ�����������ҳ���λ�úͺú�׺�����ܹ�������������ı����е��ַ����Ӷ����ƥ��Ч�ʡ�����ʱ�临�Ӷ�ͨ��ΪO(n+m)������nΪ�ı����ĳ��ȣ�mΪģʽ���ĳ��ȡ�

    Boyer-Moore�㷨��ʵ���б��㷺Ӧ�ã���һ������������ַ��������㷨��

    ��ע�⣬Boyer-Moore�㷨�ľ���ʵ�ֽ�Ϊ���ӣ������˶�ģʽ����Ԥ�����ú�׺��������ҳ���λ�õļ���ȡ���ϸ��ʵ�ִ��볬���˱��ش��ƪ���������Բ���������׻�ο����е�Boyer-Moore�㷨��ʵ�ֿ�����ϡ�
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