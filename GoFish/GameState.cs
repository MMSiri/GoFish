using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoFish
{
    public class GameState
    {
        public readonly IEnumerable<Player> Players;
        public readonly IEnumerable<Player> Opponents;
        public readonly Player HumanPlayer;
        public bool GameOver { get; private set; } = false;

        public readonly Deck Stock;

        /// <summary>
        /// Constructor creates players and deals their first hands.
        /// </summary>
        /// <param name="humanPlayerName">Name of human player.</param>
        /// <param name="opponentNames">Name of COM players.</param>
        /// <param name="stock">Shuffled stock of cards to deal from.</param>
        public GameState(string humanPlayerName, IEnumerable<string> opponentNames, Deck stock)
        {
            this.Stock = stock;

            HumanPlayer = new Player(humanPlayerName);
            HumanPlayer.GetNextHand(Stock);

            var opponents = new List<Player>();
            foreach (string name in opponentNames)
            {
                var player = new Player(name);
                player.GetNextHand(stock);
                opponents.Add(player);
            }

            Opponents = opponents;
            Players = new List<Player>() { HumanPlayer }.Concat(Opponents);
        }

        /// <summary>
        /// Get a random player not matching current player.
        /// </summary>
        /// <param name="currentPlayer">Current player.</param>
        /// <returns>Random player that current player can request card from.</returns>
        public Player RandomPlayer(Player currentPlayer) => Players.Where(player => player != currentPlayer).Skip(Player.Random.Next(Players.Count() - 1)).First();

        /// <summary>
        /// Makes one player play a round.
        /// </summary>
        /// <param name="player">Player asking for a card.</param>
        /// <param name="playerToAsk">Player being asked for a card.</param>
        /// <param name="valueToAskFor">Value to ask the player for.</param>
        /// <param name="stock">Stock to draw cards from.</param>
        /// <returns>Message describing events that happened.</returns>
        public string PlayRound(Player player, Player playerToAsk, Values valueToAskFor, Deck stock)
        {
            var valuePlural = (valueToAskFor == Values.Six) ? "Sixes" : $"{ valueToAskFor}s";
            var message = $"{player.Name} has asked {playerToAsk.Name} for {valuePlural}{Environment.NewLine}";
            var cards = playerToAsk.DoYouHaveAny(valueToAskFor, stock);
            if (cards.Count() > 0)
            {
                player.AddCardsAndPullOutBooks(cards);
                message += $"{playerToAsk.Name} has {cards.Count()} {valueToAskFor} card{Player.S(cards.Count())}";
            }
            else if (stock.Count == 0)
            {
                message += $"The stock is out of cards";
            }

            else
            {
                player.DrawCard(stock);
                message += $"{player.Name} drew a card";
            }

            if (player.Hand.Count() == 0)
            {
                player.GetNextHand(stock);
                message += $"{Environment.NewLine}{player.Name} ran out of cards, drew {player.Hand.Count()} from the stock";
            }

            return message;
        }

        /// <summary>
        /// Checks for winner by checking if any players have any cards remaining, GameOver set to true if game is over and winner found.
        /// </summary>
        /// <returns>Message showing the winners, nothing if no winners.</returns>
        public string CheckForWinner()
        {
            var playerCards = Players.Select(player => player.Hand.Count()).Sum();
            if (playerCards > 0) return "";
            GameOver = true;
            var winningBookCount = Players.Select(player => player.Books.Count()).Max();
            var winners = Players.Where(player => player.Books.Count() == winningBookCount);
            if (winners.Count() == 1) return $"The winner is {winners.First().Name}";
            return $"The winners are {string.Join(" and ", winners)}";
        }
    }
}
