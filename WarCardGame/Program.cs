using System;
using LanguageExt;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace WarCardGame
{
  enum CardStrength
  {
    Two = 0,
    Three = 1,
    Four = 2,
    Five = 3,
    Six = 4,
    Seven = 5,
    Eight = 6,
    Nine = 7,
    Ten = 8,
    Jack = 9,
    Queen = 10,
    King = 11,
    Ace = 12
  }

  public enum CardSign
  {
    Diamonds,
    Spades,
    Clubs,
    Hearts
  }

  public static class Extension
  {
    public static void WriteToConsole(this int x)
    {
      switch (x)
      {
        case > 0:
          Console.WriteLine("Alpha Player Won ");
          break;
        case < 0:
          Console.WriteLine("Beta Player Won ");
          break;
        default:
          Console.WriteLine("Truce ");
          break;
      }
    }
  }
  class Program
  {
    static void Main(string[] args)
    {
      Random rng = new Random();
      Arr<CardStrength> allCardStrengths = (Arr<CardStrength>)Enum.GetValues(typeof(CardStrength));
      Arr<CardSign> allCardSigns = (Arr<CardSign>)Enum.GetValues(typeof(CardSign));

      var game = GenerateCards(allCardStrengths, allCardSigns).Match(
        Left => Console.WriteLine(Left),
        Right => Right.OrderBy(a => rng.Next())
                .ToArr()
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 2)
                .Select(x => x.ElementAt(0).Value.CompareTo(x.ElementAt(1).Value))
                .Sum().WriteToConsole()
        );
    }

    static Either<Arr<Card>, string> GenerateCards(Arr<CardStrength> allCardStrengths, Arr<CardSign> allCardSigns)
    {
      Arr<Card> deck = allCardStrengths.SelectMany(strength => allCardSigns.Select(sign => new Card(strength, sign)));
      if (deck.Length() < 1) return "No cards to play with";

      return deck;
    }

    class Card : IComparable<Card>
    {
      public CardStrength Strength { get; }
      public CardSign Sign { get; }
      public Card(CardStrength strength, CardSign sign)
      {
        Strength = strength;
        Sign = sign;
      }

      private class StrengthCompare : IComparer<CardStrength>
      {
        int IComparer<CardStrength>.Compare(CardStrength a, CardStrength b)
        {
          if (a > b) return 1;
          if (a < b) return -1;
          return 0;
        }
      }

      private class SignCompare : IComparer<CardSign>
      {
        public CardSign Trump { get; }
        public SignCompare(CardSign trump)
        {
          Trump = trump;
        }
        int IComparer<CardSign>.Compare(CardSign a, CardSign b)
        {
          if (a == Trump && b != Trump) return 1;
          if (a != Trump && b == Trump) return -1;
          return 0;
        }
      }

      private static IComparer<CardStrength> CompareStrengths() => (IComparer<CardStrength>)new StrengthCompare();

      private static IComparer<CardSign> CompareSigns(CardSign trump) => (IComparer<CardSign>)new SignCompare(trump);


      public int CompareTo(Card other)
      {
        var comparedSigns = CompareSigns(CardSign.Spades).Compare(this.Sign, other.Sign);
        if (comparedSigns > 0) return 1;
        if (comparedSigns < 0) return -1;

        return CompareStrengths().Compare(this.Strength, other.Strength);
      }
    }

    static void ShowCardPair(Card x, Card y)
    {
      Console.WriteLine($"{x.Strength} of {x.Sign} vs {y.Strength} of {y.Sign}");
    }
  }
}
