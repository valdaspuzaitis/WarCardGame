using System;
using System.Collections.Generic;
using LanguageExt;
using System.Linq;

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

  public record Card(CardStrength Strength, CardSign Sign);

  public static class Extension
  {
    public static void SowResult(this int x) => Console.WriteLine(x);
    
    public static IComparer<TWider> Contramap<TWider, TNarrower>(this IComparer<TNarrower> cmp,
      Func<TWider, TNarrower> narrow) =>
      Comparer<TWider>.Create((wider1, wider2) => cmp.Compare(narrow(wider1), narrow(wider2)));

    public static IComparer<A> AndThen<A>(this IComparer<A> cmp1, IComparer<A> cmp2)
    {
      return Comparer<A>.Create((a1, a2) => cmp1.Compare(a1, a2) == 0 ? cmp2.Compare(a1, a2) : cmp1.Compare(a1, a2));
    }
  }

  class Program
  {
    static void Main(string[] args)
    {
      var rng = new Random();
      var allCardStrengths = ArrayOfEnumValues<CardStrength>();
      var allCardSigns = ArrayOfEnumValues<CardSign>();
      const CardSign trumpSign = CardSign.Spades;

      var game = GenerateCards(allCardStrengths, allCardSigns).Match(
        right => right.OrderBy((a) => rng.Next())
          .Select((x, i) => new {Index = i, Value = x})
          .GroupBy(x => x.Index / 2)
          .Select(x => (x.ElementAt(0).Value, x.ElementAt(1).Value))
          .Select(x =>
          {
            var compareResult = CreateCardComparer(trumpSign).Compare(x.Item1, x.Item2);
            SowPair(x);
            Console.WriteLine($" --- {compareResult}");
            return compareResult;
          })
          .Sum()
          .SowResult(),
        left => Console.WriteLine(left)
      );
    }

    private static Either<string, Arr<Card>> GenerateCards(Arr<CardStrength> allCardStrengths,
      Arr<CardSign> allCardSigns)
    {
      var deck = allCardStrengths.SelectMany(strength => allCardSigns.Select(sign => new Card(strength, sign)));
      if (deck.Count < 1) return "No cards to play with";
      return deck;
    }

    private static Arr<T> ArrayOfEnumValues<T>() where T : System.Enum => (Arr<T>) Enum.GetValues(typeof(T));
    
    IComparer<CardStrength> CardStrengthComparer() => Comparer<CardStrength>.Create((s1, s2) =>
    {
      if (s1 > s2) return 1;
      if (s1 < s2) return -1;
      return 0;
    });

    IComparer<CardSign> CreateCardSuitComparer(CardSign trump) => Comparer<CardSign>.Create((s1, s2) =>
    {
      if (s1 == trump && s2 != trump) return 1;
      if (s1 != trump && s2 == trump) return -1;
      return 0;
    });

    IComparer<Card> CreateCardComparer(CardSign trump) => CreateCardSuitComparer(trump).Contramap((Card c) => c.Sign)
      .AndThen(CardStrengthComparer().Contramap((Card c) => c.Strength));
    
    private static void SowPair((Card, Card) cmp)
    {
      Console.Write($"{cmp.Item1.Strength} of {cmp.Item1.Sign} vs {cmp.Item2.Strength} of {cmp.Item2.Sign}" );
    }
  }
}