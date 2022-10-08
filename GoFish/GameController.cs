using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace GoFish
{
    public class GameController
    {
        public static Random Random = new Random();

        private GameState gameState;
        public bool GameOver { get { return gameState.GameOver; } }
        public Player HumanPlayer { get { return gameState.HumanPlayer; } }
        public IEnumerable<Player> Opponents { get { return gameState.Opponents; } }

        public string Status { get; private set; }

        /// <summary>
        /// GameController constructor.
        /// </summary>
        /// <param name="humanPlayerName">Name of human player.</param>
        /// <param name="computerPlayerNames">Name of computer players.</param>
        public GameController(string humanPlayerName, IEnumerable<string> computerPlayerNames)
        {
            gameState = new GameState(humanPlayerName, computerPlayerNames, new Deck().Shuffle());
            Status = $"Starting a new game with players {string.Join(", ", gameState.Players)}";
        }

        /// <summary>
        /// Plays next round, game ends if all run out of cards.
        /// </summary>
        /// <param name="playerToAsk">The player the human is asking for a card.</param>
        /// <param name="valuesToAskFor">Value of the card human is asking for.</param>
        public void NextRound(Player playerToAsk, Values valuesToAskFor)
        {
            Status = gameState.PlayRound(gameState.HumanPlayer, playerToAsk, valuesToAskFor, gameState.Stock) + Environment.NewLine;

            ComputerPlayersPlayNextRound();

            Status += string.Join(Environment.NewLine, gameState.Players.Select(player => player.Status));
            Status += $"{Environment.NewLine} The stock has {gameState.Stock.Count()} cards";
            Status += Environment.NewLine + gameState.CheckForWinner();
        }

        /// <summary>
        /// All COM players with cards remaining play next round. If human player out of cards, the deck is depleted and rest of the game is played out.
        /// </summary>
        private void ComputerPlayersPlayNextRound()
        {
            IEnumerable<Player> computerPlayersWithCards;
            do
            {
                computerPlayersWithCards = gameState.Opponents.Where(player => player.Hand.Count() > 0);
                foreach (Player player in computerPlayersWithCards)
                {
                    var randomPlayer = gameState.RandomPlayer(player);
                    var randomValue = player.RandomValueFromhand();
                    Status += gameState.PlayRound(player, randomPlayer, randomValue, gameState.Stock) + Environment.NewLine;
                }
            } while ((gameState.HumanPlayer.Hand.Count() == 0) && (computerPlayersWithCards.Count() > 0));
        }

        /// <summary>
        /// New game initiated with the same player names.
        /// </summary>
        public void NewGame()
        {
            Status = "Starting a new game";
            gameState = new GameState(gameState.HumanPlayer.Name, gameState.Opponents.Select(player => player.Name), new Deck().Shuffle());
        }

    }
}
