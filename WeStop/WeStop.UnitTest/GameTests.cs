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

            _game.AddPlayerAnswerForTheme(_player1.Id, "FDS", "Ben 10");
            _game.AddPlayerAnswerForTheme(_player2.Id, "FDS", "Ben 10");
            _game.AddPlayerAnswerForTheme(_player3.Id, "FDS", "Ben 10");

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

            _game.AddPlayerAnswerForTheme(_player1.Id, "Nome", "Bruno");
            _game.AddPlayerAnswerForTheme(_player2.Id, "Nome", "Bianca");
            _game.AddPlayerAnswerForTheme(_player3.Id, "Nome", "Bruna");

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

            _game.AddPlayerAnswerForTheme(_player1.Id, "FDS", "Ben 10");
            _game.AddPlayerAnswerForTheme(_player2.Id, "FDS", "Ben 10");
            _game.AddPlayerAnswerForTheme(_player3.Id, "FDS", "Ben 10");

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

            _game.AddPlayerAnswerForTheme(_player1.Id, "FDS", "Ben 10");
            _game.AddPlayerAnswerForTheme(_player2.Id, "FDS", "Ben 10");
            _game.AddPlayerAnswerForTheme(_player3.Id, "FDS", "Ben");

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

            game.AddPlayerAnswerForTheme(_player1.Id, "Nome", "Bruno");
            game.AddPlayerAnswerForTheme(_player1.Id, "CEP", "Brasil");
            game.AddPlayerAnswerForTheme(_player1.Id, "Carro", "Belina");
            game.AddPlayerAnswerForTheme(_player1.Id, "FDS", "Bumblebee");

            game.AddPlayerAnswerForTheme(_player2.Id, "Nome", "Bruna");
            game.AddPlayerAnswerForTheme(_player2.Id, "CEP", "Bahamas");
            game.AddPlayerAnswerForTheme(_player2.Id, "Carro", "Brasilia");
            game.AddPlayerAnswerForTheme(_player2.Id, "FDS", "Band of Brothers");

            game.AddPlayerAnswerForTheme(_player3.Id, "Nome", "Breno");
            game.AddPlayerAnswerForTheme(_player3.Id, "CEP", "Bélgica");
            game.AddPlayerAnswerForTheme(_player3.Id, "Carro", "Bugatti");
            game.AddPlayerAnswerForTheme(_player3.Id, "FDS", "Ben 10");

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

            game.AddPlayerAnswerForTheme(_player1.Id, "Nome", "Bruno");
            game.AddPlayerAnswerForTheme(_player1.Id, "CEP", "Brasil");
            game.AddPlayerAnswerForTheme(_player1.Id, "Carro", "Brasilia");
            game.AddPlayerAnswerForTheme(_player1.Id, "FDS", "Ben 10");

            game.AddPlayerAnswerForTheme(_player2.Id, "Nome", "Bruna");
            game.AddPlayerAnswerForTheme(_player2.Id, "CEP", "Brasil");
            game.AddPlayerAnswerForTheme(_player2.Id, "Carro", "Belina");
            game.AddPlayerAnswerForTheme(_player2.Id, "FDS", "Ben 10");

            game.AddPlayerAnswerForTheme(_player3.Id, "Nome", "Breno");
            game.AddPlayerAnswerForTheme(_player3.Id, "CEP", "Bélgica");
            game.AddPlayerAnswerForTheme(_player3.Id, "Carro", "Brasilia");
            game.AddPlayerAnswerForTheme(_player3.Id, "FDS", "Ben 10");

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

            game.AddPlayerAnswerForTheme(_player1.Id, "Nome", "Bruno");
            game.AddPlayerAnswerForTheme(_player1.Id, "CEP", "Br");
            game.AddPlayerAnswerForTheme(_player1.Id, "Carro", "Brasilia");
            game.AddPlayerAnswerForTheme(_player1.Id, "FDS", "Band of Brothers");

            game.AddPlayerAnswerForTheme(_player2.Id, "Nome", "Bruna");
            game.AddPlayerAnswerForTheme(_player2.Id, "CEP", "Bahamas");
            game.AddPlayerAnswerForTheme(_player2.Id, "Carro", "Be");
            game.AddPlayerAnswerForTheme(_player2.Id, "FDS", "Bu");

            game.AddPlayerAnswerForTheme(_player3.Id, "Nome", "Breno");
            game.AddPlayerAnswerForTheme(_player3.Id, "CEP", "Bélgica");
            game.AddPlayerAnswerForTheme(_player3.Id, "Carro", "Bugatti");
            game.AddPlayerAnswerForTheme(_player3.Id, "FDS", "Ben 10");

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

            _game.AddPlayerAnswerForTheme(_player1.Id, "CEP", "Brasil");
            _game.AddPlayerAnswerForTheme(_player2.Id, "Nome", "Bianca");

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

            game.AddPlayerAnswerForTheme(player1.Id, "Nome", "Bruno");
            game.AddPlayerAnswerForTheme(player1.Id, "CEP", "Brasil");

            game.AddPlayerAnswerForTheme(player2.Id, "Nome", "Bruna");
            game.AddPlayerAnswerForTheme(player2.Id, "CEP", "Brasilia");

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

            game.AddPlayerAnswerForTheme(player1.Id, "Nome", "Amanda");
            game.AddPlayerAnswerForTheme(player1.Id, "CEP", "Amapá");

            game.AddPlayerAnswerForTheme(player2.Id, "Nome", "Alberto");
            game.AddPlayerAnswerForTheme(player2.Id, "CEP", "Alemanha");

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

            game.AddPlayerAnswerForTheme(player1.Id, "Nome", "Gabriel");
            game.AddPlayerAnswerForTheme(player1.Id, "CEP", "Goiás");

            game.AddPlayerAnswerForTheme(player2.Id, "Nome", "Gabriela");
            game.AddPlayerAnswerForTheme(player2.Id, "CEP", "Goiânia");

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

            game.AddPlayerAnswerForTheme(player1.Id, "Nome", "Bruno");
            game.AddPlayerAnswerForTheme(player1.Id, "CEP", "Brasil");

            game.AddPlayerAnswerForTheme(player2.Id, "Nome", "Bruna");
            game.AddPlayerAnswerForTheme(player2.Id, "CEP", "Brasilia");

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

            game.AddPlayerAnswerForTheme(player1.Id, "Nome", "Amanda");
            game.AddPlayerAnswerForTheme(player1.Id, "CEP", "Amapá");

            game.AddPlayerAnswerForTheme(player2.Id, "Nome", "Alberto");
            game.AddPlayerAnswerForTheme(player2.Id, "CEP", "Alemanha");

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

            game.AddPlayerAnswerForTheme(player1.Id, "Nome", "Gabriel");
            game.AddPlayerAnswerForTheme(player1.Id, "CEP", "Goiás");

            game.AddPlayerAnswerForTheme(player2.Id, "Nome", "Gabriela");
            game.AddPlayerAnswerForTheme(player2.Id, "CEP", "G");

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

            _game.AddPlayerAnswerForTheme(_player1.Id, "Nome", "Bruno");
            _game.AddPlayerAnswerForTheme(_player1.Id, "FDS", "Breaking bad");

            _game.AddPlayerAnswerForTheme(_player2.Id, "Nome", "Bruna");
            _game.AddPlayerAnswerForTheme(_player2.Id, "FDS", "Ben 10");

            _game.AddPlayerAnswerForTheme(_player3.Id, "Nome", "Bruno");
            _game.AddPlayerAnswerForTheme(_player3.Id, "FDS", "Ben 10");

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

            _game.AddPlayerAnswerForTheme(_player1.Id, "Nome", "Bruno");
            _game.AddPlayerAnswerForTheme(_player1.Id, "FDS", "Breaking bad");

            _game.AddPlayerAnswerForTheme(_player2.Id, "Nome", "Bruna");
            _game.AddPlayerAnswerForTheme(_player2.Id, "FDS", "Band of Brothers");

            _game.AddPlayerAnswerForTheme(_player3.Id, "Nome", "Bruno");
            _game.AddPlayerAnswerForTheme(_player3.Id, "FDS", "Band of Brothers");

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