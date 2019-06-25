using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using WeStop.Api.Classes;
using WeStop.Api.Exceptions;
using WeStop.UnitTest.Helpers;

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
            _game = new Game("teste", "", new GameOptions(_themes, _availableLetters, 6, 5, 30));
            _game.AddPlayer(_player1);
            _game.AddPlayer(_player2);
            _game.AddPlayer(_player3);
        }

        /// <summary>
        /// Testa se nas respostas válidas duplicadas são gerados 5 pontos para cada jogador
        /// </summary>
        [Test]
        public void DuplicatesAnswersGeneratesFivePointsForEachPlayer()
        {
            _game.SetAllPlayersReadyForTheNextRound();

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
        public void DifferentsAnswersGenerates10PointsForEachPlayer()
        {
            _game.SetAllPlayersReadyForTheNextRound();
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
        public void InvalidAnswersGenerates0PointsForEachPlayer()
        {
            _game.SetAllPlayersReadyForTheNextRound();
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
        public void PlayerWithAnswerInvalidatedForOthersIsGenerated0Points()
        {
            _game.SetAllPlayersReadyForTheNextRound();
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
        public void ReturnTrueWhenAllPlayersSendValidationsOfAllThemes()
        {
            _game.SetAllPlayersReadyForTheNextRound();
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
        public void ReturnTrueWhenAllPlayersSendValidationsOfATheme()
        {
            _game.SetAllPlayersReadyForTheNextRound();
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

        /// <summary>
        /// Testa se com as respostas de todos os jogadores sendo diferentes, ao fim de uma rodada as pontuações foram geradas corretamente
        /// </summary>
        [Test]
        public void PontuationWithAllAnswersDistinctIsCorrectAfterEndOfRound()
        {
            string[] themes = new string[]
            {
                "Nome",
                "CEP",
                "Carro",
                "FDS"
            };

            GameOptions gameOptions = new GameOptions(themes, _availableLetters, 2, 3, 30);
            var game = new Game("teste", "", gameOptions);
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);
            game.AddPlayer(_player3);

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Brasil") });
            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Brasilia") });
            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Band of Brothers") });
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Bahamas", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Belina", true),
                new AnswerValidation("Bugatti", true)
            }));
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Bublebee", true),
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bahamas") });
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Belina") });
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Bumblebee") });
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Bugatti", true)
            }));
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Breno") });
            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bélgica") });
            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Bugatti") });
            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Belina", true)
            }));
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Bumblebee", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");
            game.ProccessPontuationForTheme("Carro");
            game.ProccessPontuationForTheme("FDS");

            var player1Round = game.CurrentRound.Players.First(x => x.Player.Id == _player1.Id);
            var player2Round = game.CurrentRound.Players.First(x => x.Player.Id == _player2.Id);
            var player3Round = game.CurrentRound.Players.First(x => x.Player.Id == _player3.Id);

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
        public void PontuationWithSomeAnswersEqualsIsCorrectAfterEndOfRound()
        {
            string[] themes = new string[]
            {
                "Nome",
                "CEP",
                "Carro",
                "FDS"
            };

            GameOptions gameOptions = new GameOptions(themes, _availableLetters, 2, 3, 30);
            var game = new Game("teste", "", gameOptions);
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);
            game.AddPlayer(_player3);

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Brasil") });
            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Brasilia") });
            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Belina", true),
                new AnswerValidation("Brasilia", true)
            }));
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Brasil") });
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Belina") });
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true)
            }));
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Breno") });
            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bélgica") });
            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Brasilia") });
            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true)
            }));
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Belina", true),
                new AnswerValidation("Belina", true)
            }));
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");
            game.ProccessPontuationForTheme("Carro");
            game.ProccessPontuationForTheme("FDS");

            var player1Round = game.CurrentRound.Players.First(x => x.Player.Id == _player1.Id);
            var player2Round = game.CurrentRound.Players.First(x => x.Player.Id == _player2.Id);
            var player3Round = game.CurrentRound.Players.First(x => x.Player.Id == _player3.Id);

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
        public void PontuationWithSomeAnswersInvalidatedByPlayersIsCorrectAfterEndOfRound()
        {
            string[] themes = new string[]
            {
                "Nome",
                "CEP",
                "Carro",
                "FDS"
            };

            GameOptions gameOptions = new GameOptions(themes, _availableLetters, 2, 3, 30);
            var game = new Game("teste", "", gameOptions);
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);
            game.AddPlayer(_player3);

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruno") });
            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Br") });
            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Brasilia") });
            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Band of Brothers") });
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Bahamas", true),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Be", false),
                new AnswerValidation("Bugatti", true)
            }));
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Bu", false),
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bruna") });
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bahamas") });
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Be") });
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Bu") });
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Br", false),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Bugatti", true)
            }));
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Breno") });
            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Bélgica") });
            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Carro", "Bugatti") });
            game.GetPlayerCurrentRound(_player3.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("FDS", "Ben 10") });
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Br", false),
                new AnswerValidation("Bélgica", true)
            }));
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("Carro", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Be", false)
            }));
            game.GetPlayerCurrentRound(_player3.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Bu", false)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");
            game.ProccessPontuationForTheme("Carro");
            game.ProccessPontuationForTheme("FDS");

            var player1Round = game.CurrentRound.Players.First(x => x.Player.Id == _player1.Id);
            var player2Round = game.CurrentRound.Players.First(x => x.Player.Id == _player2.Id);
            var player3Round = game.CurrentRound.Players.First(x => x.Player.Id == _player3.Id);

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
        public void ReturnTrueWhenPlayerDidNotReportATheme()
        {
            GameOptions gameOptions = new GameOptions(_themes, _availableLetters, 2, 3, 30);
            var game = new Game("teste", "", gameOptions);
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            });

            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bruna", true)
            }));

            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("FDS", "Ben 10")
            });

            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("FDS", new List<AnswerValidation>()
            {
                new AnswerValidation("Ben 10", true)
            }));

            Assert.IsTrue(game.AllPlayersSendValidationsOfAllThemes());
        }

        /// <summary>
        /// Verifica se os pontos são gerados corretamente quando um jogador não informou resposa para um tema
        /// </summary>
        [Test]
        public void PontuationIsCorrectWhenPlayerDidNotReportATheme()
        {
            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            _game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("CEP", "Brasil") });
            _game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>()
            {
                new AnswerValidation("Bianca", true)
            }));

            _game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer> { new ThemeAnswer("Nome", "Bianca") });
            _game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>()
            {
                new AnswerValidation("Brasil", true)
            }));

            _game.ProccessPontuationForTheme("Nome");
            _game.ProccessPontuationForTheme("CEP");

            var player1Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player1.Id);
            var player2Round = _game.CurrentRound.Players.First(x => x.Player.Id == _player2.Id);

            Assert.AreEqual(0, player1Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(10, player1Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player1Round.EarnedPoints);

            Assert.AreEqual(10, player2Round.ThemesPontuations["Nome"]);
            Assert.AreEqual(0, player2Round.ThemesPontuations["CEP"]);
            Assert.AreEqual(10, player2Round.EarnedPoints);
        }

        /// <summary>
        /// Testa se as letras sorteadas nas rodadas não são repetidas
        /// </summary>
        [Test]
        public void NotRepeatSortedLetters()
        {
            List<string> sortedLetters = new List<string>();

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            sortedLetters.Add(_game.CurrentRound.SortedLetter);

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.CurrentRound.SortedLetter));
            sortedLetters.Add(_game.CurrentRound.SortedLetter);

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.CurrentRound.SortedLetter));
            sortedLetters.Add(_game.CurrentRound.SortedLetter);

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.CurrentRound.SortedLetter));
            sortedLetters.Add(_game.CurrentRound.SortedLetter);

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.CurrentRound.SortedLetter));
            sortedLetters.Add(_game.CurrentRound.SortedLetter);

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.CurrentRound.SortedLetter));
            sortedLetters.Add(_game.CurrentRound.SortedLetter);
        }

        /// <summary>
        /// Testa se quando o jogo já estiver finalizado, não será possível iniciar uma nova rodada
        /// </summary>
        [Test]
        public void NewRoundIsNotStartedIfGameIsFinished()
        {
            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            Assert.Throws<WeStopException>(() => _game.StartNextRound());
        }

        /// <summary>
        /// Verifica se o array de vencedores possui mais de um registro quando a partida terminou em empate
        /// </summary>
        [Test]
        public void ReturnMoreThanOneWinnerWhenDrew()
        {
            var themes = new string[]
            {
                "Nome",
                "CEP"
            };

            var player1 = new Player(new User("Bruno"), true);
            var player2 = new Player(new User("Gustavo"), false);

            var game = new Game("teste", "", new GameOptions(themes, _availableLetters, 3, 2, 30));
            game.AddPlayer(player1);
            game.AddPlayer(player2);

            #region First Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();
            
            game.GetPlayerCurrentRound(player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("CEP", "Brasil")
            });

            game.GetPlayerCurrentRound(player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Bruna", true)
            }));

            game.GetPlayerCurrentRound(player1.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Brasilia", true)
            }));

            game.GetPlayerCurrentRound(player2.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("CEP", "Brasilia")
            });
            
            game.GetPlayerCurrentRound(player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Bruno", true)
            }));

            game.GetPlayerCurrentRound(player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Brasil", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");

            #endregion

            #region Second Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();
            
            game.GetPlayerCurrentRound(player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Amanda"),
                new ThemeAnswer("CEP", "Amapá")
            });

            game.GetPlayerCurrentRound(player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Alberto", true)
            }));

            game.GetPlayerCurrentRound(player1.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Alemanha", true)
            }));
            
            game.GetPlayerCurrentRound(player2.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Alberto"),
                new ThemeAnswer("CEP", "Alemanha")
            });

            game.GetPlayerCurrentRound(player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Amanda", true)
            }));

            game.GetPlayerCurrentRound(player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Amapá", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");

            #endregion

            #region Third Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Gabriel"),
                new ThemeAnswer("CEP", "Goiás")
            });

            game.GetPlayerCurrentRound(player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Gabriela", true)
            }));

            game.GetPlayerCurrentRound(player1.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Goiânia", true)
            }));
            
            game.GetPlayerCurrentRound(player2.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Gabriela"),
                new ThemeAnswer("CEP", "Goiânia")
            });

            game.GetPlayerCurrentRound(player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Gabriel", true)
            }));

            game.GetPlayerCurrentRound(player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Goiás", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");

            #endregion
        
            var winners = game.GetWinners();
            
            Assert.Greater(winners.Count(), 1);
        }

        /// <summary>
        /// Verifica se o array de vencedores possui apenas um registro quando a partida possui apenas um vencedor
        /// </summary>
        [Test]
        public void ReturnOneWinnerWhenHasVictory()
        {
            var themes = new string[]
            {
                "Nome",
                "CEP"
            };

            var game = new Game("teste", "", new GameOptions(themes, _availableLetters, 3, 2, 30));
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);

            #region First Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("CEP", "Brasil")
            });

            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Bruna", true)
            }));

            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Brasilia", true)
            }));

            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("CEP", "Brasilia")
            });
            
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Bruno", true)
            }));

            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Brasil", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");

            #endregion

            #region Second Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Amanda"),
                new ThemeAnswer("CEP", "Amapá")
            });            
            
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Alberto", true)
            }));

            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Alemanha", true)
            }));
            
            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Alberto"),
                new ThemeAnswer("CEP", "Alemanha")
            });

            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Amanda", true)
            }));

            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Amapá", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");

            #endregion

            #region Third Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Gabriel"),
                new ThemeAnswer("CEP", "Goiás")
            });
            
            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Gabriela", true)
            }));

            game.GetPlayerCurrentRound(_player1.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("G", false)
            }));

            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Gabriela"),
                new ThemeAnswer("CEP", "G")
            });
            
            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Gabriel", true)
            }));

            game.GetPlayerCurrentRound(_player2.Id).AddThemeAnswersValidations(new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Goiás", true)
            }));

            game.ProccessPontuationForTheme("Nome");
            game.ProccessPontuationForTheme("CEP");

            #endregion
        
            var winners = game.GetWinners();

            Assert.AreEqual(1, winners.Count());
            Assert.AreEqual("Bruno", winners.ElementAt(0));
        }
    
        [Test]
        public void ReturnTrueWhenAllPlayersSendAnswers()
        {
            var themes = new string[]
            {
                "Nome",
                "CEP"
            };

            var game = new Game("teste", "", new GameOptions(themes, _availableLetters, 3, 2, 30));
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("CEP", "Brasil")
            });

            game.GetPlayerCurrentRound(_player2.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("CEP", "Brasilia")
            });
            
            Assert.True(game.AllOnlinePlayersSendAnswers());
        }

        [Test]
        public void ReturnFalseWhenOnePlayerNotSendAnswers()
        {
            var themes = new string[]
            {
                "Nome",
                "CEP"
            };

            var game = new Game("teste", "", new GameOptions(themes, _availableLetters, 3, 2, 30));
            game.AddPlayer(_player1);
            game.AddPlayer(_player2);

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            game.GetPlayerCurrentRound(_player1.Id).AddAnswers(new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("CEP", "Brasil")
            });
            
            Assert.False(game.AllOnlinePlayersSendAnswers());
        }
    
        [Test]
        public void AllPlayersAnswersAreReturnedExceptFromEspecifiedPlayer()
        {
            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("FDS", "Breaking bad")
            };

            _game.GetPlayerCurrentRound(_player1.Id).AddAnswers(player1Answers);

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("FDS", "Ben 10")
            };

            _game.GetPlayerCurrentRound(_player2.Id).AddAnswers(player2Answers);

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("FDS", "Ben 10")
            };

            _game.GetPlayerCurrentRound(_player3.Id).AddAnswers(player3Answers);

            _game.ProccessPontuationForTheme("Nome");
            _game.ProccessPontuationForTheme("FDS");

            var answersForPlayer1 = _game.CurrentRound.GetPlayersAnswers(_player1.Id);
            Assert.AreEqual(2, answersForPlayer1.First(x => x.Theme == "Nome").Answers.Count());
            Assert.AreEqual("Bruna", answersForPlayer1.First(x => x.Theme == "Nome").Answers.ToArray()[0]);
            Assert.AreEqual(1, answersForPlayer1.First(x => x.Theme == "FDS").Answers.Count());
            Assert.AreEqual("Ben 10", answersForPlayer1.First(x => x.Theme == "FDS").Answers.ToArray()[0]);

            var answersForPlayer2 = _game.CurrentRound.GetPlayersAnswers(_player2.Id);
            Assert.AreEqual(1, answersForPlayer2.First(x => x.Theme == "Nome").Answers.Count());
            Assert.AreEqual("Bruno", answersForPlayer2.First(x => x.Theme == "Nome").Answers.ToArray()[0]);
            Assert.AreEqual(2, answersForPlayer2.First(x => x.Theme == "FDS").Answers.Count());
            Assert.AreEqual("Breaking bad", answersForPlayer2.First(x => x.Theme == "FDS").Answers.ToArray()[0]);

            var answersForPlayer3 = _game.CurrentRound.GetPlayersAnswers(_player3.Id);
            Assert.AreEqual(2, answersForPlayer3.First(x => x.Theme == "Nome").Answers.Count());
            Assert.AreEqual("Bruno", answersForPlayer3.First(x => x.Theme == "Nome").Answers.ToArray()[0]);
            Assert.AreEqual("Bruna", answersForPlayer3.First(x => x.Theme == "Nome").Answers.ToArray()[1]);
            Assert.AreEqual(2, answersForPlayer3.First(x => x.Theme == "FDS").Answers.Count());
            Assert.AreEqual("Breaking bad", answersForPlayer3.First(x => x.Theme == "FDS").Answers.ToArray()[0]);
            Assert.AreEqual("Ben 10", answersForPlayer3.First(x => x.Theme == "FDS").Answers.ToArray()[1]);
        }
    }
}