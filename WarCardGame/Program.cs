using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;

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

      ShowCardPairs(alphaCards.Zip(betaCards, (x, y) => (x, y)).ToList());

      if (alphaPlayerScoreAfterGame > playerHandSize) Console.WriteLine($"Alpha player won with {alphaPlayerScoreAfterGame} points");
      else if (alphaPlayerScoreAfterGame < playerHandSize) Console.WriteLine($"Beta player won with {playerHandSize * 2 - alphaPlayerScoreAfterGame} points");
      else Console.WriteLine("Game was a tie");
    }

    static int BattleScore(Card x, Card y, CardSign trump)
    {
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
      Arr<Card> fullDeck = new Arr<Card>(new Card[0]);

      foreach (CardSign sign in Enum.GetValues(typeof(CardSign)))
      {
        foreach (CardStrength strength in Enum.GetValues(typeof(CardStrength)))
        {
          fullDeck = fullDeck.Add(new Card(strength, sign));
        }
      }

      return fullDeck;
    }

    static void ShowCardPairs(List<(Card x, Card y)> pairs)
    {
      foreach (var pair in pairs)
      {
        Console.WriteLine(pair.x.strength + " of " + pair.x.sign + " vs " + pair.y.strength + " of " + pair.y.sign);
      }
    }
  }
}
