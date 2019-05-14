using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Infra.Hubs;

namespace WeStop.UnitTest
{
    [TestFixture]
    public class GameTests
    {
        private readonly string[] _availableLetters;
        private readonly string[] _themes;
        private readonly Player _player1;
        private readonly Player _player2;
        private readonly Player _player3;
        private readonly Game _game;

        public GameTests()
        {
            _availableLetters = new string[]
            {
                "A",
                "B",
                "C",
                "D",
                "E",
                "F"
            };

            _themes = new string[]
            {
                "Nome",
                "FDS"
            };

            _player1 = new Player
            {
                Id = Guid.NewGuid(),
                UserName = "Bruno",
                IsAdmin = true,
                IsReady = true
            };

            _player2 = new Player
            {
                Id = Guid.NewGuid(),
                UserName = "Gustavo",
                IsAdmin = false,
                IsReady = true
            };

            _player3 = new Player
            {
                Id = Guid.NewGuid(),
                UserName = "Davi",
                IsAdmin = false,
                IsReady = true
            };

            _game = new Game("teste", "", new GameOptions(_themes, _availableLetters, 3, 5));
        }

        [SetUp]
        public void SetUp()
        {

        }

        /// <summary>
        /// Testa se nas respostas válidas duplicadas são gerados 5 pontos para cada jogador
        /// </summary>
        [Test]
        public void TestIfDuplicatesAnswersGenerates5PointsForEachPlayer()
        {
            _game.AddPlayer(_player1);
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.ProccessPontuationForTheme("FDS");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player1.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player2.Id);
            var player3Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player3.Id);

            Assert.AreEqual(5, player1Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(5, player1Round.EarnedPoints);

            Assert.AreEqual(5, player2Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(5, player2Round.EarnedPoints);

            Assert.AreEqual(5, player3Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(5, player3Round.EarnedPoints);
        }

        /// <summary>
        /// Testa se nas respostas válidas diferentes são gerados 10 pontos para cada jogador
        /// </summary>
        [Test]
        public void TestIfDifferentsAnswersGenerates10PointsForEachPlayer()
        {
            _game.AddPlayer(_player1);
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            _game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bianca", true),
                new AnswerValidation("Bruna", true)
            }));

            _game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bianca") });
            _game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));

            _game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            _game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bianca", true),
                new AnswerValidation("Bruno", true)
            }));

            _game.ProccessPontuationForTheme("Nome");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player1.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player2.Id);
            var player3Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player3.Id);

            Assert.AreEqual(10, player1Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player1Round.EarnedPoints);

            Assert.AreEqual(10, player2Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player2Round.EarnedPoints);

            Assert.AreEqual(10, player3Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player3Round.EarnedPoints);
        }

        /// <summary>
        /// Testa se nas respostas inválidas são gerados 0 pontos para cada jogador
        /// </summary>
        [Test]
        public void TestIfInvalidAnswersGenerates0PointsForEachPlayer()
        {
            _game.AddPlayer(_player1);
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", false)
            }));

            _game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", false)
            }));

            _game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.ProccessPontuationForTheme("FDS");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player1.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player2.Id);
            var player3Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player3.Id);

            Assert.AreEqual(0, player1Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(0, player1Round.EarnedPoints);

            Assert.AreEqual(0, player2Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(0, player2Round.EarnedPoints);

            Assert.AreEqual(0, player3Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(0, player3Round.EarnedPoints);
        }

        /// <summary>
        /// Testa se para um jogador com resposta invalidada pelos outros é gerado 0 pontos
        /// </summary>
        [Test]
        public void TestPlayerWithAnswerInvalidatedForOthersIsGenerated0Points()
        {
            _game.AddPlayer(_player1);
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true),
                new AnswerValidation("Ben", false)
            }));

            _game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true),
                new AnswerValidation("Ben", false)
            }));

            _game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben") });
            _game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.ProccessPontuationForTheme("FDS");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player1.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player2.Id);
            var player3Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player3.Id);

            Assert.AreEqual(5, player1Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(5, player1Round.EarnedPoints);

            Assert.AreEqual(5, player2Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(5, player2Round.EarnedPoints);

            Assert.AreEqual(0, player3Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(0, player3Round.EarnedPoints);
        }

        /// <summary>
        /// Testa se retorna verdadeiro quando todos os jogadores enviaram suas validações para todos os temas
        /// </summary>
        [Test]
        public void TestReturnTrueWhenAllPlayersSendValidationsOfAllThemes()
        {
            _game.AddPlayer(_player1);
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("FDS", "Ben 10")
            });
            _game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true),
                new AnswerValidation("Ben", false)
            }));

            _game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));

            _game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("FDS", "Ben 10")
            });
            _game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true),
                new AnswerValidation("Ben", false)
            }));

            _game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));

            _game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Breno"),
                new ThemeAnswer("FDS", "Ben")
            });
            _game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));

            Assert.IsTrue(_game.AllPlayersSendValidationsOfAllThemes());
        }

        /// <summary>
        /// Testa se retorna verdadeiro quando todos os jogadores enviaram suas validações para um tema específico
        /// </summary>
        [Test]
        public void TestReturnTrueWhenAllPlayersSendValidationsOfATheme()
        {
            _game.AddPlayer(_player1);
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            _game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));

            _game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            _game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));

            _game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Breno") });
            _game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));

            Assert.IsTrue(_game.AllPlayersSendValidationsOfTheme("Nome"));
        }
    }
}
