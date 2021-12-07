using System;
using LanguageExt;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace WarCardGame
{
  public enum CardStrength
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

  public record Card(CardStrength strength, CardSign sign);

  public static class Extension
  {

    public static IComparer<Wider> contramap<Wider, Narrower>(this IComparer<Narrower> cmp, Func<Wider, Narrower> narrow) => Comparer<Wider>.Create((wider1, wider2) => cmp.Compare(narrow(wider1), narrow(wider2)));

    public static IComparer<A> AndThen<A>(this IComparer<A> cmp1, IComparer<A> cmp2)
    {
      Comparer<A> comparer = Comparer<A>.Create((a1, a2) =>
      {
        if (cmp1.Compare(a1, a2) == 0)
        {
          return cmp2.Compare(a1, a2);
        }
        return cmp1.Compare(a1, a2);
      });
      return comparer;
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
      Random rng = new Random();
      Arr<CardStrength> allCardStrengths = (Arr<CardStrength>)Enum.GetValues(typeof(CardStrength));
      Arr<CardSign> allCardSigns = (Arr<CardSign>)Enum.GetValues(typeof(CardSign));

      IComparer<CardStrength> cardStrengthComparer() => Comparer<CardStrength>.Create((s1, s2) =>
      {
        if (s1 > s2) return 1;
        if (s1 < s2) return -1;
        return 0;
      });

      IComparer<CardSign> createCardSuitComparer(CardSign trump) => Comparer<CardSign>.Create((s1, s2) =>
      {
        if (s1 == trump && s2 != trump) return 1;
        if (s1 != trump && s2 == trump) return -1;
        return 0;
      });

      IComparer<Card> createCardComparer(CardSign trump) => createCardSuitComparer(trump).contramap((Card c) => c.sign).AndThen(cardStrengthComparer().contramap((Card c) => c.strength));

      var game = GenerateCards(allCardStrengths, allCardSigns).Match(
        Left => Console.WriteLine(Left),
        Right => Console.WriteLine(Right.OrderBy(a => rng.Next())
                .ToArr()
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 2)
                .Select(x => createCardComparer(CardSign.Diamonds).Compare(x.ElementAt(0).Value, x.ElementAt(1).Value))
                .Sum()
                .ToString())
        );
    }

    static Either<Arr<Card>, string> GenerateCards(Arr<CardStrength> allCardStrengths, Arr<CardSign> allCardSigns)
    {
      Arr<Card> deck = allCardStrengths.SelectMany(strength => allCardSigns.Select(sign => new Card(strength, sign)));
      if (deck.Length() < 1) return "No cards to play with";

      return deck;
    }
  }
}
