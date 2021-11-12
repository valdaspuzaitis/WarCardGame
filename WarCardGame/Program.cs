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
      Arr<Card> schuffledDeck = GenerateCards().OrderBy(a => rng.Next()).ToArr();

      int playerHandSize = schuffledDeck.Length / 2;

      Arr<Card> alphaCards = schuffledDeck.RemoveRange(0, playerHandSize);
      Arr<Card> betaCards = schuffledDeck.RemoveRange(playerHandSize, playerHandSize);
      CardSign trump = CardSign.Spades;

      var alphaPlayerScoreAfterGame = alphaCards.Zip(betaCards, (x, y) => BattleScore(x, y, trump)).Sum();

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

    record Card(CardStrength strength, CardSign sign);

    enum CardSign
    {
      Diamonds,
      Spades,
      Clubs,
      Hearts
    }

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

    static Arr<Card> GenerateCards()
    {
      Arr<CardStrength> allCardStrengths = (Arr<CardStrength>)Enum.GetValues(typeof(CardStrength));
      Arr<CardSign> allCardSigns = (Arr<CardSign>)Enum.GetValues(typeof(CardSign));

      //method syntax
      Arr<Card> fullDeckMethod = allCardStrengths.SelectMany(strength => allCardSigns.Select(sign => new Card(strength, sign)));

      //query syntax
      Arr<Card> fullDeckQuery = (from strength in allCardStrengths
                             from sign in allCardSigns
                             select new Card(strength, sign));

      return fullDeckMethod;
    }

    static void ShowCardPair(Card x, Card y)
    {
      Console.WriteLine($"{x.strength} of {x.sign} vs {y.strength} of {y.sign}");
    }
  }
}
