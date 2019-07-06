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

        [Test]
        public void DuplicatesAnswersGeneratesFivePointsForEachPlayer()
        {
            _game.SetAllPlayersReadyForTheNextRound();

            _game.StartNextRound();

            List<ThemeAnswer> player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            };

            List<ThemeAnswer> player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            };

            List<ThemeAnswer> player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            };

            _game.AddPlayerAnswers(_player1.Id, player1Answers);
            _game.AddPlayerAnswers(_player2.Id, player2Answers);
            _game.AddPlayerAnswers(_player3.Id, player3Answers);

            List<ThemeValidation> player1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("FDS", new List<AnswerValidation>()
                {
                    new AnswerValidation("Ben 10", true)
                })
            };

            List<ThemeValidation> player2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("FDS", new List<AnswerValidation>()
                {
                    new AnswerValidation("Ben 10", true)
                })
            };

            List<ThemeValidation> player3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("FDS", new List<AnswerValidation>()
                {
                    new AnswerValidation("Ben 10", true)
                })
            };

            _game.AddPlayerAnswersValidations(_player1.Id, player1Validations);
            _game.AddPlayerAnswersValidations(_player2.Id, player2Validations);
            _game.AddPlayerAnswersValidations(_player3.Id, player3Validations);

            _game.GeneratePontuationForTheme("FDS");

            Assert.AreEqual(5, _game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "FDS"));
            Assert.AreEqual(5, _game.GetPlayerCurrentRoundEarnedPoints(_player1.Id));

            Assert.AreEqual(5, _game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "FDS"));
            Assert.AreEqual(5, _game.GetPlayerCurrentRoundEarnedPoints(_player2.Id));

            Assert.AreEqual(5, _game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "FDS"));
            Assert.AreEqual(5, _game.GetPlayerCurrentRoundEarnedPoints(_player3.Id));
        }

        [Test]
        public void DifferentsAnswersGeneratesTenPointsForEachPlayer()
        {
            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno")
            };

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bianca")
            };

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna")
            };

            _game.AddPlayerAnswers(_player1.Id, player1Answers);
            _game.AddPlayerAnswers(_player2.Id, player2Answers);
            _game.AddPlayerAnswers(_player3.Id, player3Answers);

            var player1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bianca", true),
                    new AnswerValidation("Bruna", true)
                })
            };

            var player2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruno", true),
                    new AnswerValidation("Bruna", true)
                })
            };

            var player3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bianca", true),
                    new AnswerValidation("Bruno", true)
                })
            };

            _game.AddPlayerAnswersValidations(_player1.Id, player1Validations);
            _game.AddPlayerAnswersValidations(_player2.Id, player2Validations);
            _game.AddPlayerAnswersValidations(_player3.Id, player3Validations);

            _game.GeneratePontuationForTheme("Nome");

            Assert.AreEqual(10, _game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "Nome"));
            Assert.AreEqual(10, _game.GetPlayerCurrentRoundEarnedPoints(_player1.Id));

            Assert.AreEqual(10, _game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "Nome"));
            Assert.AreEqual(10, _game.GetPlayerCurrentRoundEarnedPoints(_player2.Id));

            Assert.AreEqual(10, _game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "Nome"));
            Assert.AreEqual(10, _game.GetPlayerCurrentRoundEarnedPoints(_player3.Id));
        }

        [Test]
        public void InvalidAnswersGeneratesZeroPointsForEachPlayer()
        {
            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            };

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            };

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            };

            _game.AddPlayerAnswers(_player1.Id, player1Answers);
            _game.AddPlayerAnswers(_player2.Id, player2Answers);
            _game.AddPlayerAnswers(_player3.Id, player3Answers);

            var player1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("FDS", new List<AnswerValidation>
                {
                    new AnswerValidation("Ben 10", false)
                })
            };

            var player2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("FDS", new List<AnswerValidation>
                {
                    new AnswerValidation("Ben 10", false)
                })
            };

            var player3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("FDS", new List<AnswerValidation>
                {
                    new AnswerValidation("Ben 10", true)
                })
            };

            _game.AddPlayerAnswersValidations(_player1.Id, player1Validations);
            _game.AddPlayerAnswersValidations(_player2.Id, player2Validations);
            _game.AddPlayerAnswersValidations(_player3.Id, player3Validations);

            _game.GeneratePontuationForTheme("FDS");

            Assert.AreEqual(0, _game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "FDS"));
            Assert.AreEqual(0, _game.GetPlayerCurrentRoundEarnedPoints(_player1.Id));

            Assert.AreEqual(0, _game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "FDS"));
            Assert.AreEqual(0, _game.GetPlayerCurrentRoundEarnedPoints(_player2.Id));

            Assert.AreEqual(0, _game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "FDS"));
            Assert.AreEqual(0, _game.GetPlayerCurrentRoundEarnedPoints(_player3.Id));
        }

        [Test]
        public void PlayerWithAnswerInvalidatedForOthersIsGenerated0Points()
        {
            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            };

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben 10")
            };

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("FDS", "Ben")
            };

            _game.AddPlayerAnswers(_player1.Id, player1Answers);
            _game.AddPlayerAnswers(_player2.Id, player2Answers);
            _game.AddPlayerAnswers(_player3.Id, player3Answers);

            var player1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("FDS", new List<AnswerValidation>
                {
                    new AnswerValidation("Ben 10", true),
                    new AnswerValidation("Ben", false)
                })
            };

            var player2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("FDS", new List<AnswerValidation>
                {
                    new AnswerValidation("Ben 10", true),
                    new AnswerValidation("Ben", false)
                })
            };

            var player3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("FDS", new List<AnswerValidation>
                {
                    new AnswerValidation("Ben 10", true)
                })
            };

            _game.AddPlayerAnswersValidations(_player1.Id, player1Validations);
            _game.AddPlayerAnswersValidations(_player2.Id, player2Validations);
            _game.AddPlayerAnswersValidations(_player3.Id, player3Validations);

            _game.GeneratePontuationForTheme("FDS");

            Assert.AreEqual(5, _game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "FDS"));
            Assert.AreEqual(5, _game.GetPlayerCurrentRoundEarnedPoints(_player1.Id));

            Assert.AreEqual(5, _game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "FDS"));
            Assert.AreEqual(5, _game.GetPlayerCurrentRoundEarnedPoints(_player2.Id));

            Assert.AreEqual(0, _game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "FDS"));
            Assert.AreEqual(0, _game.GetPlayerCurrentRoundEarnedPoints(_player3.Id));
        }

        [Test]
        public void ReturnTrueWhenAllPlayersSendValidationsOfATheme()
        {
            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno")
            };

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna")
            };

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Breno")
            };

            _game.AddPlayerAnswers(_player1.Id, player1Answers);
            _game.AddPlayerAnswers(_player2.Id, player2Answers);
            _game.AddPlayerAnswers(_player3.Id, player3Answers);

            var player1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruna", true),
                    new AnswerValidation("Breno", true)
                })
            };

            var player2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruno", true),
                    new AnswerValidation("Breno", true)
                })
            };

            var player3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruno", true),
                    new AnswerValidation("Bruna", true)
                })
            };

            _game.AddPlayerAnswersValidations(_player1.Id, player1Validations);
            _game.AddPlayerAnswersValidations(_player2.Id, player2Validations);
            _game.AddPlayerAnswersValidations(_player3.Id, player3Validations);

            Assert.IsTrue(_game.AllPlayersSendValidationsOfTheme("Nome"));
        }

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

            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("CEP", "Brasil"),
                new ThemeAnswer("Carro", "Brasilia"),
                new ThemeAnswer("FDS", "Band of Brothers"),
            };

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("CEP", "Bahamas"),
                new ThemeAnswer("Carro", "Belina"),
                new ThemeAnswer("FDS", "Bumblebee"),
            };

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Breno"),
                new ThemeAnswer("CEP", "Bélgica"),
                new ThemeAnswer("Carro", "Bugatti"),
                new ThemeAnswer("FDS", "Ben 10"),
            };

            game.AddPlayerAnswers(_player1.Id, player1Answers);
            game.AddPlayerAnswers(_player2.Id, player2Answers);
            game.AddPlayerAnswers(_player3.Id, player3Answers);

            var player1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruna", true),
                    new AnswerValidation("Breno", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>()
                {
                    new AnswerValidation("Bahamas", true),
                    new AnswerValidation("Bélgica", true)
                }),
                new ThemeValidation("Carro", new List<AnswerValidation>()
                {
                    new AnswerValidation("Belina", true),
                    new AnswerValidation("Bugatti", true)
                }),
                new ThemeValidation("FDS", new List<AnswerValidation>()
                {
                    new AnswerValidation("Bublebee", true),
                    new AnswerValidation("Ben 10", true)
                })
            };

            var player2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>()
                {
                    new AnswerValidation("Bruno", true),
                    new AnswerValidation("Breno", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>()
                {
                    new AnswerValidation("Brasil", true),
                    new AnswerValidation("Bélgica", true)
                }),
                new ThemeValidation("Carro", new List<AnswerValidation>()
                {
                    new AnswerValidation("Brasilia", true),
                    new AnswerValidation("Bugatti", true)
                }),
                new ThemeValidation("FDS", new List<AnswerValidation>()
                {
                    new AnswerValidation("Band of Brothers", true),
                    new AnswerValidation("Ben 10", true)
                })
            };

            var player3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>()
                {
                    new AnswerValidation("Bruno", true),
                    new AnswerValidation("Bruna", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>()
                {
                    new AnswerValidation("Brasil", true),
                    new AnswerValidation("Bélgica", true)
                }),
                new ThemeValidation("Carro", new List<AnswerValidation>()
                {
                    new AnswerValidation("Brasilia", true),
                    new AnswerValidation("Belina", true)
                }),
                new ThemeValidation("FDS", new List<AnswerValidation>()
                {
                    new AnswerValidation("Band of Brothers", true),
                    new AnswerValidation("Bumblebee", true)
                })
            };

            game.AddPlayerAnswersValidations(_player1.Id, player1Validations);
            game.AddPlayerAnswersValidations(_player2.Id, player2Validations);
            game.AddPlayerAnswersValidations(_player3.Id, player3Validations);

            game.GeneratePontuationForTheme("Nome");
            game.GeneratePontuationForTheme("CEP");
            game.GeneratePontuationForTheme("Carro");
            game.GeneratePontuationForTheme("FDS");

            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "Nome"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "CEP"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "Carro"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "FDS"));
            Assert.AreEqual(40, game.GetPlayerCurrentRoundEarnedPoints(_player1.Id));

            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "Nome"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "CEP"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "Carro"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "FDS"));
            Assert.AreEqual(40, game.GetPlayerCurrentRoundEarnedPoints(_player2.Id));

            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "Nome"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "CEP"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "Carro"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "FDS"));
            Assert.AreEqual(40, game.GetPlayerCurrentRoundEarnedPoints(_player3.Id));
        }

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

            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("CEP", "Brasil"),
                new ThemeAnswer("Carro", "Brasilia"),
                new ThemeAnswer("FDS", "Ben 10"),
            };

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("CEP", "Brasil"),
                new ThemeAnswer("Carro", "Belina"),
                new ThemeAnswer("FDS", "Ben 10"),
            };

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Breno"),
                new ThemeAnswer("CEP", "Bélgica"),
                new ThemeAnswer("Carro", "Brasilia"),
                new ThemeAnswer("FDS", "Ben 10"),
            };

            game.AddPlayerAnswers(_player1.Id, player1Answers);
            game.AddPlayerAnswers(_player2.Id, player2Answers);
            game.AddPlayerAnswers(_player3.Id, player3Answers);

            var player1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruna", true),
                    new AnswerValidation("Breno", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>()
                {
                    new AnswerValidation("Brasil", true),
                    new AnswerValidation("Bélgica", true)
                }),
                new ThemeValidation("Carro", new List<AnswerValidation>()
                {
                    new AnswerValidation("Belina", true),
                    new AnswerValidation("Brasilia", true)
                }),
                new ThemeValidation("FDS", new List<AnswerValidation>()
                {
                    new AnswerValidation("Ben 10", true)
                })
            };

            var player2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>()
                {
                    new AnswerValidation("Bruno", true),
                    new AnswerValidation("Breno", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>()
                {
                    new AnswerValidation("Brasil", true),
                    new AnswerValidation("Bélgica", true)
                }),
                new ThemeValidation("Carro", new List<AnswerValidation>()
                {
                    new AnswerValidation("Brasilia", true)
                }),
                new ThemeValidation("FDS", new List<AnswerValidation>()
                {
                    new AnswerValidation("Ben 10", true)
                })
            };

            var player3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>()
                {
                    new AnswerValidation("Bruno", true),
                    new AnswerValidation("Bruna", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>()
                {
                    new AnswerValidation("Brasil", true)
                }),
                new ThemeValidation("Carro", new List<AnswerValidation>()
                {
                    new AnswerValidation("Belina", true)
                }),
                new ThemeValidation("FDS", new List<AnswerValidation>()
                {
                    new AnswerValidation("Ben 10", true)
                })
            };

            game.AddPlayerAnswersValidations(_player1.Id, player1Validations);
            game.AddPlayerAnswersValidations(_player2.Id, player2Validations);
            game.AddPlayerAnswersValidations(_player3.Id, player3Validations);

            game.GeneratePontuationForTheme("Nome");
            game.GeneratePontuationForTheme("CEP");
            game.GeneratePontuationForTheme("Carro");
            game.GeneratePontuationForTheme("FDS");

            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "Nome"));
            Assert.AreEqual(5, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "CEP"));
            Assert.AreEqual(5, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "Carro"));
            Assert.AreEqual(5, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "FDS"));
            Assert.AreEqual(25, game.GetPlayerCurrentRoundEarnedPoints(_player1.Id));

            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "Nome"));
            Assert.AreEqual(5, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "CEP"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "Carro"));
            Assert.AreEqual(5, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "FDS"));
            Assert.AreEqual(30, game.GetPlayerCurrentRoundEarnedPoints(_player2.Id));

            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "Nome"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "CEP"));
            Assert.AreEqual(5, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "Carro"));
            Assert.AreEqual(5, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "FDS"));
            Assert.AreEqual(30, game.GetPlayerCurrentRoundEarnedPoints(_player3.Id));
        }

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

            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("CEP", "Br"),
                new ThemeAnswer("Carro", "Brasilia"),
                new ThemeAnswer("FDS", "Band of Brothers")
            };

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("CEP", "Bahamas"),
                new ThemeAnswer("Carro", "Be"),
                new ThemeAnswer("FDS", "Bu")
            };

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Breno"),
                new ThemeAnswer("CEP", "Bélgica"),
                new ThemeAnswer("Carro", "Bugatti"),
                new ThemeAnswer("FDS", "Ben 10")
            };

            game.AddPlayerAnswers(_player1.Id, player1Answers);
            game.AddPlayerAnswers(_player2.Id, player2Answers);
            game.AddPlayerAnswers(_player3.Id, player3Answers);

            game.AddPlayerAnswersValidations(_player1.Id, new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Bruna", true),
                new AnswerValidation("Breno", true)
            }));

            game.AddPlayerAnswersValidations(_player1.Id, new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Bahamas", true),
                new AnswerValidation("Bélgica", true)
            }));

            game.AddPlayerAnswersValidations(_player1.Id, new ThemeValidation("Carro", new List<AnswerValidation>
            {
                new AnswerValidation("Be", false),
                new AnswerValidation("Bugatti", true)
            }));

            game.AddPlayerAnswersValidations(_player1.Id, new ThemeValidation("FDS", new List<AnswerValidation>
            {
                new AnswerValidation("Bu", false),
                new AnswerValidation("Ben 10", true)
            }));

            game.AddPlayerAnswersValidations(_player2.Id, new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Breno", true)
            }));

            game.AddPlayerAnswersValidations(_player2.Id, new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Br", false),
                new AnswerValidation("Bélgica", true)
            }));

            game.AddPlayerAnswersValidations(_player2.Id, new ThemeValidation("Carro", new List<AnswerValidation>
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Bugatti", true)
            }));

            game.AddPlayerAnswersValidations(_player2.Id, new ThemeValidation("FDS", new List<AnswerValidation>
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Ben 10", true)
            }));

            game.AddPlayerAnswersValidations(_player3.Id, new ThemeValidation("Nome", new List<AnswerValidation>
            {
                new AnswerValidation("Bruno", true),
                new AnswerValidation("Bruna", true)
            }));

            game.AddPlayerAnswersValidations(_player3.Id, new ThemeValidation("CEP", new List<AnswerValidation>
            {
                new AnswerValidation("Br", false),
                new AnswerValidation("Bélgica", true)
            }));

            game.AddPlayerAnswersValidations(_player3.Id, new ThemeValidation("Carro", new List<AnswerValidation>
            {
                new AnswerValidation("Brasilia", true),
                new AnswerValidation("Be", false)
            }));

            game.AddPlayerAnswersValidations(_player3.Id, new ThemeValidation("FDS", new List<AnswerValidation>
            {
                new AnswerValidation("Band of Brothers", true),
                new AnswerValidation("Bu", false)
            }));

            game.GeneratePontuationForTheme("Nome");
            game.GeneratePontuationForTheme("CEP");
            game.GeneratePontuationForTheme("Carro");
            game.GeneratePontuationForTheme("FDS");

            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "Nome"));
            Assert.AreEqual(0, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "CEP"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "Carro"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "FDS"));
            Assert.AreEqual(30, game.GetPlayerCurrentRoundEarnedPoints(_player1.Id));
            ;

            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "Nome"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "CEP"));
            Assert.AreEqual(0, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "Carro"));
            Assert.AreEqual(0, game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "FDS"));
            Assert.AreEqual(20, game.GetPlayerCurrentRoundEarnedPoints(_player2.Id));

            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "Nome"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "CEP"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "Carro"));
            Assert.AreEqual(10, game.GetPlayerCurrentRoundPontuationForTheme(_player3.Id, "FDS"));
            Assert.AreEqual(40, game.GetPlayerCurrentRoundEarnedPoints(_player3.Id));
        }

        [Test]
        public void PontuationIsCorrectWhenPlayerDidNotReplyForATheme()
        {
            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("CEP", "Brasil")
            };
            
            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bianca")
            };

            _game.AddPlayerAnswers(_player1.Id, player1Answers);
            _game.AddPlayerAnswers(_player2.Id, player2Answers);

            var player1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bianca", true)
                })
            };

            var player2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Brasil", true)
                })
            };

            _game.GeneratePontuationForTheme("Nome");
            _game.GeneratePontuationForTheme("CEP");

            Assert.AreEqual(0, _game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "Nome"));
            Assert.AreEqual(10, _game.GetPlayerCurrentRoundPontuationForTheme(_player1.Id, "CEP"));
            Assert.AreEqual(10, _game.GetPlayerCurrentRoundEarnedPoints(_player1.Id));

            Assert.AreEqual(10, _game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "Nome"));
            Assert.AreEqual(0, _game.GetPlayerCurrentRoundPontuationForTheme(_player2.Id, "CEP"));
            Assert.AreEqual(10, _game.GetPlayerCurrentRoundEarnedPoints(_player2.Id));
        }

        [Test]
        public void NotRepeatSortedLetters()
        {
            List<string> sortedLetters = new List<string>();

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            sortedLetters.Add(_game.GetCurrentRoundSortedLetter());

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.GetCurrentRoundSortedLetter()));
            sortedLetters.Add(_game.GetCurrentRoundSortedLetter());

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.GetCurrentRoundSortedLetter()));
            sortedLetters.Add(_game.GetCurrentRoundSortedLetter());

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.GetCurrentRoundSortedLetter()));
            sortedLetters.Add(_game.GetCurrentRoundSortedLetter());

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.GetCurrentRoundSortedLetter()));
            sortedLetters.Add(_game.GetCurrentRoundSortedLetter());

            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();
            Assert.IsFalse(sortedLetters.Contains(_game.GetCurrentRoundSortedLetter()));
            sortedLetters.Add(_game.GetCurrentRoundSortedLetter());
        }

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

            var player1Round1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("CEP", "Brasil")
            };
            
            var player2Round1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("CEP", "Brasilia")
            };

            game.AddPlayerAnswers(player1.Id, player1Round1Answers);
            game.AddPlayerAnswers(player2.Id, player2Round1Answers);

            var player1Round1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruna", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Brasilia", true)
                })
            };

            var player2Round1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruno", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Brasil", true)
                })
            };

            game.AddPlayerAnswersValidations(player1.Id, player1Round1Validations);
            game.AddPlayerAnswersValidations(player2.Id, player2Round1Validations);

            game.GeneratePontuationForTheme("Nome");
            game.GeneratePontuationForTheme("CEP");

            #endregion

            #region Second Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            var player1Round2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Amanda"),
                new ThemeAnswer("CEP", "Amapá")
            };

            var player2Round2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Alberto"),
                new ThemeAnswer("CEP", "Alemanha")
            };

            game.AddPlayerAnswers(player1.Id, player1Round2Answers);
            game.AddPlayerAnswers(player2.Id, player2Round2Answers);

            var player1Round2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Alberto", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Alemanha", true)
                })
            };

            var player2Round2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Amanda", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Amapá", true)
                })
            };

            game.AddPlayerAnswersValidations(player1.Id, player1Round2Validations);
            game.AddPlayerAnswersValidations(player2.Id, player2Round2Validations);

            game.GeneratePontuationForTheme("Nome");
            game.GeneratePontuationForTheme("CEP");

            #endregion

            #region Third Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            var player1Round3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Gabriel"),
                new ThemeAnswer("CEP", "Goiás")
            };

            var player2Round3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Gabriela"),
                new ThemeAnswer("CEP", "Goiânia")
            };

            game.AddPlayerAnswers(player1.Id, player1Round3Answers);
            game.AddPlayerAnswers(player2.Id, player2Round3Answers);

            var player1Round3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Gabriela", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Goiânia", true)
                })
            };

            var player2Round3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Gabriel", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Goiás", true)
                })
            };

            game.AddPlayerAnswersValidations(player1.Id, player1Round3Validations);
            game.AddPlayerAnswersValidations(player2.Id, player2Round3Validations);

            game.GeneratePontuationForTheme("Nome");
            game.GeneratePontuationForTheme("CEP");

            #endregion

            var winners = game.GetWinners();

            Assert.Greater(winners.Count(), 1);
        }

        [Test]
        public void ReturnOneWinnerWhenHasVictory()
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

            var player1Round1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("CEP", "Brasil")
            };

            var player2Round1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("CEP", "Brasilia")
            };

            game.AddPlayerAnswers(player1.Id, player1Round1Answers);
            game.AddPlayerAnswers(player2.Id, player2Round1Answers);

            var player1Round1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruna", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Brasilia", true)
                })
            };

            var player2Round1Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Bruno", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Brasil", true)
                })
            };

            game.AddPlayerAnswersValidations(player1.Id, player1Round1Validations);
            game.AddPlayerAnswersValidations(player2.Id, player2Round1Validations);

            game.GeneratePontuationForTheme("Nome");
            game.GeneratePontuationForTheme("CEP");

            #endregion

            #region Second Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            var player1Round2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Amanda"),
                new ThemeAnswer("CEP", "Amapá")
            };

            var player2Round2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Alberto"),
                new ThemeAnswer("CEP", "Alemanha")
            };

            game.AddPlayerAnswers(player1.Id, player1Round2Answers);
            game.AddPlayerAnswers(player2.Id, player2Round2Answers);

            var player1Round2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Alberto", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Alemanha", true)
                })
            };

            var player2Round2Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Amanda", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Amapá", true)
                })
            };

            game.AddPlayerAnswersValidations(player1.Id, player1Round2Validations);
            game.AddPlayerAnswersValidations(player2.Id, player2Round2Validations);

            game.GeneratePontuationForTheme("Nome");
            game.GeneratePontuationForTheme("CEP");

            #endregion

            #region Third Round

            game.SetAllPlayersReadyForTheNextRound();
            game.StartNextRound();

            var player1Round3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Gabriel"),
                new ThemeAnswer("CEP", "Goiás")
            };

            var player2Round3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Gabriela"),
                new ThemeAnswer("CEP", "G")
            };

            game.AddPlayerAnswers(player1.Id, player1Round3Answers);
            game.AddPlayerAnswers(player2.Id, player2Round3Answers);

            var player1Round3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Gabriela", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("G", false)
                })
            };

            var player2Round3Validations = new List<ThemeValidation>
            {
                new ThemeValidation("Nome", new List<AnswerValidation>
                {
                    new AnswerValidation("Gabriel", true)
                }),
                new ThemeValidation("CEP", new List<AnswerValidation>
                {
                    new AnswerValidation("Goiás", true)
                })
            };

            game.AddPlayerAnswersValidations(player1.Id, player1Round3Validations);
            game.AddPlayerAnswersValidations(player2.Id, player2Round3Validations);

            game.GeneratePontuationForTheme("Nome");
            game.GeneratePontuationForTheme("CEP");

            #endregion

            var winners = game.GetWinners();

            Assert.AreEqual(1, winners.Count());
            Assert.AreEqual("Bruno", winners.ElementAt(0));
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

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("FDS", "Ben 10")
            };

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("FDS", "Ben 10")
            };

            _game.AddPlayerAnswers(_player1.Id, player1Answers);
            _game.AddPlayerAnswers(_player2.Id, player2Answers);
            _game.AddPlayerAnswers(_player3.Id, player3Answers);

            _game.GeneratePontuationForTheme("Nome");
            _game.GeneratePontuationForTheme("FDS");

            var answersForPlayer1 = _game.GetCurrentRoundPlayersAnswersExceptFromPlayer(_player1.Id);
            Assert.AreEqual(2, answersForPlayer1.First(x => x.Theme == "Nome").Answers.Count());
            Assert.AreEqual("Bruna", answersForPlayer1.First(x => x.Theme == "Nome").Answers.ToArray()[0]);
            Assert.AreEqual(1, answersForPlayer1.First(x => x.Theme == "FDS").Answers.Count());
            Assert.AreEqual("Ben 10", answersForPlayer1.First(x => x.Theme == "FDS").Answers.ToArray()[0]);

            var answersForPlayer2 = _game.GetCurrentRoundPlayersAnswersExceptFromPlayer(_player2.Id);
            Assert.AreEqual(1, answersForPlayer2.First(x => x.Theme == "Nome").Answers.Count());
            Assert.AreEqual("Bruno", answersForPlayer2.First(x => x.Theme == "Nome").Answers.ToArray()[0]);
            Assert.AreEqual(2, answersForPlayer2.First(x => x.Theme == "FDS").Answers.Count());
            Assert.AreEqual("Breaking bad", answersForPlayer2.First(x => x.Theme == "FDS").Answers.ToArray()[0]);

            var answersForPlayer3 = _game.GetCurrentRoundPlayersAnswersExceptFromPlayer(_player3.Id);
            Assert.AreEqual(2, answersForPlayer3.First(x => x.Theme == "Nome").Answers.Count());
            Assert.AreEqual("Bruno", answersForPlayer3.First(x => x.Theme == "Nome").Answers.ToArray()[0]);
            Assert.AreEqual("Bruna", answersForPlayer3.First(x => x.Theme == "Nome").Answers.ToArray()[1]);
            Assert.AreEqual(2, answersForPlayer3.First(x => x.Theme == "FDS").Answers.Count());
            Assert.AreEqual("Breaking bad", answersForPlayer3.First(x => x.Theme == "FDS").Answers.ToArray()[0]);
            Assert.AreEqual("Ben 10", answersForPlayer3.First(x => x.Theme == "FDS").Answers.ToArray()[1]);
        }

        [Test]
        public void ValidationObjectIsBuiltCorrectly()
        {
            _game.SetAllPlayersReadyForTheNextRound();
            _game.StartNextRound();

            var player1Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("FDS", "Breaking bad")
            };

            var player2Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruna"),
                new ThemeAnswer("FDS", "Band of Brothers")
            };

            var player3Answers = new List<ThemeAnswer>
            {
                new ThemeAnswer("Nome", "Bruno"),
                new ThemeAnswer("FDS", "Band of Brothers")
            };

            _game.AddPlayerAnswers(_player1.Id, player1Answers);
            _game.AddPlayerAnswers(_player2.Id, player2Answers);
            _game.AddPlayerAnswers(_player3.Id, player3Answers);

            var player1Validations = _game.BuildValidationForPlayer(_player1.Id);
            var player2Validations = _game.BuildValidationForPlayer(_player2.Id);
            var player3Validations = _game.BuildValidationForPlayer(_player3.Id);

            Assert.True(player1Validations.First(x => x.Theme == "Nome").AnswersValidations.Select(x => x.Answer).Contains("Bruna"));
            Assert.True(player1Validations.First(x => x.Theme == "Nome").AnswersValidations.Select(x => x.Answer).Contains("Bruno"));
            Assert.True(player1Validations.First(x => x.Theme == "FDS").AnswersValidations.Select(x => x.Answer).Contains("Band of Brothers"));
            Assert.False(player1Validations.First(x => x.Theme == "FDS").AnswersValidations.Select(x => x.Answer).Contains("Breaking bad"));

            Assert.False(player2Validations.First(x => x.Theme == "Nome").AnswersValidations.Select(x => x.Answer).Contains("Bruna"));
            Assert.True(player2Validations.First(x => x.Theme == "Nome").AnswersValidations.Select(x => x.Answer).Contains("Bruno"));
            Assert.True(player2Validations.First(x => x.Theme == "FDS").AnswersValidations.Select(x => x.Answer).Contains("Band of Brothers"));
            Assert.True(player2Validations.First(x => x.Theme == "FDS").AnswersValidations.Select(x => x.Answer).Contains("Breaking bad"));

            Assert.True(player3Validations.First(x => x.Theme == "Nome").AnswersValidations.Select(x => x.Answer).Contains("Bruna"));
            Assert.True(player3Validations.First(x => x.Theme == "Nome").AnswersValidations.Select(x => x.Answer).Contains("Bruno"));
            Assert.True(player3Validations.First(x => x.Theme == "FDS").AnswersValidations.Select(x => x.Answer).Contains("Band of Brothers"));
            Assert.True(player3Validations.First(x => x.Theme == "FDS").AnswersValidations.Select(x => x.Answer).Contains("Breaking bad"));
        }
    }
}