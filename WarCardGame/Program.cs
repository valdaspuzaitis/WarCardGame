using System;
using LanguageExt;
using System.Linq;

namespace WarCardGame
{
  class Program
  {
    static void Main(string[] args)
    {
      Random rng = new Random();
      Arr<CardStrength> allCardStrengths = (Arr<CardStrength>)Enum.GetValues(typeof(CardStrength));
      Arr<CardSign> allCardSigns = (Arr<CardSign>)Enum.GetValues(typeof(CardSign));
      CardSign trump = CardSign.Spades;

      var game = GenerateCards(allCardStrengths, allCardSigns).Match(
        Left => Console.WriteLine(Left),
        Right => WinCondition(Right.OrderBy(a => rng.Next())
                .ToArr()
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / 2)
                .Map(x => BattleScore(x.ElementAt(0).Value, x.ElementAt(1).Value, trump))
                .Sum()
                , Right.Length() / 2)
        );
    }

    static void WinCondition(int alphaPlayerScoreAfterGame, int playerHandSize)
    {
      if (alphaPlayerScoreAfterGame > playerHandSize) Console.WriteLine($"Alpha player won with {alphaPlayerScoreAfterGame} points");
      else if (alphaPlayerScoreAfterGame < playerHandSize) Console.WriteLine($"Beta player won with {playerHandSize * 2 - alphaPlayerScoreAfterGame} points");
      else Console.WriteLine("Game was a tie");
    }

    static int BattleScore(Card x, Card y, CardSign trump)
    {
      ShowCardPair(x, y);
      if (x.sign == trump && y.sign != trump || x.strength > y.strength && y.sign != trump) return 2;
      if (x.strength == y.strength && y.sign != trump) return 1;
      return 0;
    }

    static Either<Arr<Card>, string> GenerateCards(Arr<CardStrength> allCardStrengths, Arr<CardSign> allCardSigns)
    {
      Arr<Card> deck = allCardStrengths.SelectMany(strength => allCardSigns.Select(sign => new Card(strength, sign)));
      if (deck.Length() < 1) return "No cards to play with";

      return deck;
    }

    record Card(CardStrength strength, CardSign sign);

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

    enum CardSign
    {
      Diamonds,
      Spades,
      Clubs,
      Hearts
    }

    static void ShowCardPair(Card x, Card y)
    {
      Console.WriteLine($"{x.strength} of {x.sign} vs {y.strength} of {y.sign}");
    }
  }
}
