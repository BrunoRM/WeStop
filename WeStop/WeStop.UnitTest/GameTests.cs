using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;

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
        private Game _game;

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

            _player1 = new Player(new User("Bruno"), true);
            _player2 = new Player(new User("Gustavo"), false);
            _player3 = new Player(new User("Davi"), false);
        }

        [SetUp]
        public void SetUp()
        {
            _game = new Game("teste", "", new GameOptions(_themes, _availableLetters, 3, 5));
            _game.AddPlayer(_player1);
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
        }

        /// <summary>
        /// Testa se nas respostas válidas duplicadas são gerados 5 pontos para cada jogador
        /// </summary>
        [Test]
        public void TestIfDuplicatesAnswersGenerates5PointsForEachPlayer()
        {
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.ProccessPontuationForTheme("FDS");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player1.User.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player2.User.Id);
            var player3Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player3.User.Id);

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
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            _game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bianca", true),
                new AnswerValidation("Bruna", true)
            }));

            _game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bianca") });
            _game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));

            _game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            _game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bianca", true),
                new AnswerValidation("Bruno", true)
            }));

            _game.ProccessPontuationForTheme("Nome");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player1.User.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player2.User.Id);
            var player3Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player3.User.Id);

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
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", false)
            }));

            _game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", false)
            }));

            _game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.ProccessPontuationForTheme("FDS");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player1.User.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player2.User.Id);
            var player3Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player3.User.Id);

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
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true),
                new AnswerValidation("Ben", false)
            }));

            _game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            _game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true),
                new AnswerValidation("Ben", false)
            }));

            _game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben") });
            _game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.ProccessPontuationForTheme("FDS");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player1.User.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player2.User.Id);
            var player3Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player3.User.Id);

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
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("FDS", "Ben 10")
            });
            _game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true),
                new AnswerValidation("Ben", false)
            }));

            _game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));

            _game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("FDS", "Ben 10")
            });
            _game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true),
                new AnswerValidation("Ben", false)
            }));

            _game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));

            _game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Breno"),
                new ThemeAnswer("FDS", "Ben")
            });
            _game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            _game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
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
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            _game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));

            _game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            _game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));

            _game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Breno") });
            _game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));

            Assert.IsTrue(_game.AllPlayersSendValidationsOfTheme("Nome"));
        }

        /// <summary>
        /// Testa se com as respostas de todos os jogadores sendo diferentes, ao fim de uma rodada as pontuações foram geradas corretamente
        /// </summary>
        [Test]
        public void TestPontuationWithAllAnswersDistinctIsCorrectAfterEndOfRound()
        {
            string[] themes = new string[]
            {
                "Nome",
                "CEP",
                "Carro",
                "FDS"
            };

            GameOptions gameOptions = new GameOptions(themes, _availableLetters, 2, 3);
            var game = new Game("teste", "", gameOptions);
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);
            game.AddPlayer(_player3);

            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Brasil") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Brasilia") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Band of Brothers") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Bahamas", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Belina", true),
                new AnswerValidation("Bugatti", true)
            }));
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Bublebee", true),
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bahamas") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Belina") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Bumblebee") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Bugatti", true)
            }));
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Breno") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bélgica") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Bugatti") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Belina", true)
            }));
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Bumblebee", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");
            game.ProccessPontuationForTheme("Carro");
            game.ProccessPontuationForTheme("FDS");

            var player1Round = game.CurrentRound.Players.First(x => x.Player.User.Id == _player1.User.Id);
            var player2Round = game.CurrentRound.Players.First(x => x.Player.User.Id == _player2.User.Id);
            var player3Round = game.CurrentRound.Players.First(x => x.Player.User.Id == _player3.User.Id);

            Assert.AreEqual(10, player1Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player1Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player1Round.ThemesPontuations["Carro"]);
            Assert.AreEqual(10, player1Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(40, player1Round.EarnedPoints);

            Assert.AreEqual(10, player2Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player2Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player2Round.ThemesPontuations["Carro"]);
            Assert.AreEqual(10, player2Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(40, player2Round.EarnedPoints);

            Assert.AreEqual(10, player3Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player3Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player3Round.ThemesPontuations["Carro"]);
            Assert.AreEqual(10, player3Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(40, player3Round.EarnedPoints);
        }

        /// <summary>
        /// Testa se com as respostas de todos os jogadores, com algumas iguais, ao fim de uma rodada as pontuações foram geradas corretamente
        /// </summary>
        [Test]
        public void TestPontuationWithSomeAnswersEqualsIsCorrectAfterEndOfRound()
        {
            string[] themes = new string[]
            {
                "Nome",
                "CEP",
                "Carro",
                "FDS"
            };

            GameOptions gameOptions = new GameOptions(themes, _availableLetters, 2, 3);
            var game = new Game("teste", "", gameOptions);
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);
            game.AddPlayer(_player3);

            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Brasil") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Brasilia") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Belina", true),
                new AnswerValidation("Brasilia", true)
            }));
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Brasil") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Belina") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true)
            }));
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Breno") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bélgica") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Brasilia") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true)
            }));
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Belina", true),
                new AnswerValidation("Belina", true)
            }));
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");
            game.ProccessPontuationForTheme("Carro");
            game.ProccessPontuationForTheme("FDS");

            var player1Round = game.CurrentRound.Players.First(x => x.Player.User.Id == _player1.User.Id);
            var player2Round = game.CurrentRound.Players.First(x => x.Player.User.Id == _player2.User.Id);
            var player3Round = game.CurrentRound.Players.First(x => x.Player.User.Id == _player3.User.Id);

            Assert.AreEqual(10, player1Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(5, player1Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(5, player1Round.ThemesPontuations["Carro"]);
            Assert.AreEqual(5, player1Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(25, player1Round.EarnedPoints);

            Assert.AreEqual(10, player2Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(5, player2Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player2Round.ThemesPontuations["Carro"]);
            Assert.AreEqual(5, player2Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(30, player2Round.EarnedPoints);

            Assert.AreEqual(10, player3Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player3Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(5, player3Round.ThemesPontuations["Carro"]);
            Assert.AreEqual(5, player3Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(30, player3Round.EarnedPoints);
        }

        /// <summary>
        /// Testa se com algumas respostas sendo inválidadas pelos jogadores, ao fim de uma rodada as pontuações foram geradas corretamente
        /// </summary>
        [Test]
        public void TestPontuationWithSomeAnswersInvalidatedByPlayersIsCorrectAfterEndOfRound()
        {
            string[] themes = new string[]
            {
                "Nome",
                "CEP",
                "Carro",
                "FDS"
            };

            GameOptions gameOptions = new GameOptions(themes, _availableLetters, 2, 3);
            var game = new Game("teste", "", gameOptions);
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);
            game.AddPlayer(_player3);

            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Br") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Brasilia") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Band of Brothers") });
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Bahamas", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Be", false),
                new AnswerValidation("Bugatti", true)
            }));
            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Bu", false),
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bahamas") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Be") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Bu") });
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Br", false),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Bugatti", true)
            }));
            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Breno") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bélgica") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Bugatti") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Br", false),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Be", false)
            }));
            game.GetPlayerCurrentRound(_player3.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Bu", false)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");
            game.ProccessPontuationForTheme("Carro");
            game.ProccessPontuationForTheme("FDS");

            var player1Round = game.CurrentRound.Players.First(x => x.Player.User.Id == _player1.User.Id);
            var player2Round = game.CurrentRound.Players.First(x => x.Player.User.Id == _player2.User.Id);
            var player3Round = game.CurrentRound.Players.First(x => x.Player.User.Id == _player3.User.Id);

            Assert.AreEqual(10, player1Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(0, player1Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player1Round.ThemesPontuations["Carro"]);
            Assert.AreEqual(10, player1Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(30, player1Round.EarnedPoints);

            Assert.AreEqual(10, player2Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player2Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(0, player2Round.ThemesPontuations["Carro"]);
            Assert.AreEqual(0, player2Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(20, player2Round.EarnedPoints);

            Assert.AreEqual(10, player3Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player3Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player3Round.ThemesPontuations["Carro"]);
            Assert.AreEqual(10, player3Round.ThemesPontuations["FDS"]);
            Assert.AreEqual(40, player3Round.EarnedPoints);
        }

        [Test]
        public void TestReturnTrueWhenPlayerDidNotReportATheme()
        {
            GameOptions gameOptions = new GameOptions(_themes, _availableLetters, 2, 3);
            var game = new Game("teste", "", gameOptions);
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            });

            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true)
            }));

            game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("FDS", "Ben 10")
            });

            game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            Assert.IsTrue(game.AllPlayersSendValidationsOfAllThemes());
        }

        [Test]
        public void TestPontuationIsCorrectWhenPlayerDidNotReportATheme()
        {
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Brasil") });
            _game.GetPlayerCurrentRound(_player1.User.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bianca", true)
            }));

            _game.GetPlayerCurrentRound(_player2.User.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bianca") });
            _game.GetPlayerCurrentRound(_player2.User.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true)
            }));

            _game.ProccessPontuationForTheme("Nome");
            _game.ProccessPontuationForTheme("CEP");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player1.User.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.User.Id == _player2.User.Id);

            Assert.AreEqual(0, player1Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player1Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player1Round.EarnedPoints);

            Assert.AreEqual(10, player2Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(0, player2Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player2Round.EarnedPoints);
        }
    }
}
