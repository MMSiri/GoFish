using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using GoFish;
using System.Linq;

namespace GoFishTests
{
    [TestClass]
    public class GameControllerTests
    {
        [TestInitialize]
        public void Initialise()
        {
            Player.Random = new PlayerTests.MockRandom() { ValueToReturn = 0 };
        }

        [TestMethod]
        public void TestConstructor()
        {
            var gameController = new GameController("Human", new List<string>() { "Player1", "Player2", "Player3", });
            Assert.AreEqual("Starting a new game with players Human, Player1, Player2, Player3", gameController.Status);

        }

        [TestMethod]
        public void TestNextRound()
        {

            //Constructor shuffles the deck, but MockRandom ensures it stays in order so Owen should have Ace to 5 of Diamonds, Brittney should have 6 to 10 of Diamonds.
            var gameController = new GameController("Owen", new List<string>() { "Brittney" });

            gameController.NextRound(gameController.Opponents.First(), Values.Six);
            Assert.AreEqual(@"Owen has asked Brittney for Sixes
                              Brittney has 1 Six card
                              Brittney asked Owen for Sevens
                              Brittney drew a card
                              Owen has 6 cards and 0 books
                              Brittney has 5 cards and 0 books
                              The stock has 41 cards
                              ", gameController.Status);
        }

        [TestMethod]
        public void TestNewGame()
        {
            Player.Random = new PlayerTests.MockRandom() { ValueToReturn = 0 };
            var gameController = new GameController("Owen", new List<string>() { "Brittney" });
            gameController.NextRound(gameController.Opponents.First(), Values.Six);
            gameController.NewGame();
            Assert.AreEqual("Owen", gameController.HumanPlayer.Name);
            Assert.AreEqual("Brittney", gameController.Opponents.First().Name);
            Assert.AreEqual("Starting a new game", gameController.Status);
        }
    }
}
