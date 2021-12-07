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

  public static class Extension
  {
    public static IComparer<Wider> contramap<Wider, Narrower>(this IComparer<Narrower> cmp, Func<Wider, Narrower> narrow) => Comparer<Wider>.Create((wider1, wider2) => cmp.Compare(narrow(wider1), narrow(wider2)));
    public static IComparer<A> AndThen<A>(this IComparer<A> cmp1, IComparer<A> cmp2)
    {
      // ¯\_(ツ)_/¯ black magic
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
        if (s1 == trump && s2 != trump) return 2;
        if (s1 != trump && s2 == trump) return -2;
        return 0;
      });

      IComparer<Card> createCardComparer(CardSign trump) => createCardSuitComparer(trump).contramap((Card c) => c.sign).AndThen(cardStrengthComparer().contramap((Card c) => c.strength));

      var game = GenerateCards(allCardStrengths, allCardSigns).Match(
        Left => Console.WriteLine(Left),
        Right => Right.OrderBy(a => rng.Next())
                .ToArr()
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 2)
                .ToList()
                .ForEach(x =>
                {
                  Console.Write($"{x.ElementAt(0).Value.strength} {x.ElementAt(0).Value.sign} --- {x.ElementAt(1).Value.strength} {x.ElementAt(1).Value.sign}");
                  var aftercompare = createCardComparer(CardSign.Diamonds).Compare(x.ElementAt(0).Value, x.ElementAt(1).Value);
                  Console.WriteLine($"    {aftercompare}");
                })
        );
    }

    record Card(CardStrength strength, CardSign sign);

    static CardSign CardToCardSign(Card card) => card.sign;
    static CardStrength CardToCardStrength(Card card) => card.strength;

    static Either<Arr<Card>, string> GenerateCards(Arr<CardStrength> allCardStrengths, Arr<CardSign> allCardSigns)
    {
      Arr<Card> deck = allCardStrengths.SelectMany(strength => allCardSigns.Select(sign => new Card(strength, sign)));
      if (deck.Length() < 1) return "No cards to play with";

      return deck;
    }
  }
}
