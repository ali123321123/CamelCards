using System;
using System.Collections.Generic;
using System.Linq;

public class Advent2023
{
    private static readonly char[] Faces = { 'T', 'J', 'Q', 'K', 'A' };
    private static readonly int[] Strength = { 10, 11, 12, 13, 14 };
    private static readonly int[] StrengthWithJoker = { 10, 1, 12, 13, 14 };

    private class CardStrength
    {
        public static int Value(char c, bool joker)
        {
            if (char.IsDigit(c)) { return c - '0'; }
            int i = 0;
            while (c != Faces[i]) { ++i; }
            return joker ? StrengthWithJoker[i] : Strength[i];
        }
    }

    private class Hand : IComparable<Hand>
    {
        public static readonly int HandSize = 5;

        public char[] Cards = new char[HandSize];
        public long Bid;

        public Tuple<long, long> CardGroups;
        public bool WithJoker;

        public long Winnings(int i) => i * Bid;

        public int CompareTo(Hand other)
        {
            var (n, m) = CardGroups;
            var (u, v) = other.CardGroups;

            if (n != u) { return n > u ? -1 : 1; }
            if (m == v)
            {
                return Cards.Zip(other.Cards, (x, y) =>
                    CardStrength.Value(x, WithJoker).CompareTo(CardStrength.Value(y, other.WithJoker)))
                    .FirstOrDefault(result => result != 0);
            }
            return m < v ? -1 : 1;
        }
    }

    private static Tuple<long, long> CountGroups(string s, bool withJoker)
    {
        var sorted = s.OrderBy(c => c).ToArray();
        var ndup = 0;
        var gmax = 0;
        var njok = withJoker ? s.Count(c => c == 'J') : 0;
        if (njok == s.Length) { return Tuple.Create(1L, 5L); }

        for (int i = 0; i < s.Length - 1;)
        {
            var c = 0;
            var j = i + 1;
            if (withJoker)
            {
                while (j < s.Length && s[i] != 'J' && s[i] == s[j]) { ++j; ++c; }
            }
            else
            {
                while (j < s.Length && s[i] == s[j]) { ++j; ++c; }
            }
            ndup += c;
            gmax = Math.Max(c + 1, gmax);
            i = j;
        }
        return Tuple.Create((long)(s.Length - ndup - njok), (long)(gmax + njok));
    }


    private static List<Hand> Parse(List<string> input)
    {
        var hands = new List<Hand>(input.Count);
        foreach (var s in input)
        {
            var i = s.IndexOf(' ');
            var cards = s.Substring(0, i).ToCharArray();
            long bid = 0;
            long.TryParse(s.Substring(i + 1), out bid);
            hands.Add(new Hand { Cards = cards, Bid = bid });
        }
        return hands;
    }

    public static Tuple<long, long> Day07()
    {
        var input = System.IO.File.ReadLines("./source/2023/07/input.txt").ToList();
        var hands = Parse(input);

        long Winnings(List<Hand> handsList, bool withJoker)
        {
            foreach (var hand in handsList)
            {
                hand.WithJoker = withJoker;
                hand.CardGroups = CountGroups(new string(hand.Cards), withJoker);
            }
            handsList.Sort();
            return handsList.Select((h, i) => h.Winnings(i + 1)).Sum();
        }

        var p1 = Winnings(hands, false);
        var p2 = Winnings(hands, true);
        return Tuple.Create(p1, p2);
    }

    public static void Main()
    {
        var result = Day07();
        Console.WriteLine($"Part 1: {result.Item1}");
        Console.WriteLine($"Part 2: {result.Item2}");
    }
}
