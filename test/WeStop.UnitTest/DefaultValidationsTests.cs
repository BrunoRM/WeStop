using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WeStop.Api.Core;
using WeStop.Api.Extensions;
using WeStop.UnitTest.Helpers;

namespace WeStop.UnitTest
{
    [TestFixture]
    public class DefaultValidationsTests : Test
    {
        [SetUp]
        public void Initialize()
        {
            CreateDefaultConfig();
        }

        [Test]
        public void GivenBlankAnswersShouldDiscard()
        {
            Game.StartNextRound();
            var dustinRoundAnswers = new RoundAnswers(Game.Id, Game.CurrentRoundNumber, TestUsers.Dustin.Id,
                new List<Answer>
                {
                    new Answer("Nome", ""),
                    new Answer("CEP", "Brasil")
                });

            var willRoundAnswers = new RoundAnswers(Game.Id, Game.CurrentRoundNumber, TestUsers.Will.Id,
                new List<Answer>
                {
                    new Answer("Nome", "Bruno"),
                    new Answer("CEP", "")
                });

            var roundAnswers = new List<RoundAnswers>
            {
                dustinRoundAnswers,
                willRoundAnswers
            };

            var dustinDefaultValidationsForThemeNome = roundAnswers.BuildValidationsForPlayer(TestUsers.Dustin.Id, "Nome").ToList();
            var dustinDefaultValidationsForThemeCEP = roundAnswers.BuildValidationsForPlayer(TestUsers.Dustin.Id, "CEP").ToList();
            var willDefaultValidationsForThemeNome = roundAnswers.BuildValidationsForPlayer(TestUsers.Will.Id, "Nome").ToList();
            var willDefaultValidationsForThemeCEP = roundAnswers.BuildValidationsForPlayer(TestUsers.Will.Id, "CEP").ToList();

            Assert.False(dustinDefaultValidationsForThemeCEP.Any(v => v.Answer == new Answer("CEP", "")));
            Assert.True(dustinDefaultValidationsForThemeNome.Any(v => v.Answer == new Answer("Nome", "Bruno")));

            Assert.False(willDefaultValidationsForThemeNome.Any(v => v.Answer == new Answer("Nome", "")));
            Assert.True(willDefaultValidationsForThemeCEP.Any(v => v.Answer == new Answer("CEP", "Brasil")));
        }

        [Test]
        public void GivenRepeatedAnswersShouldConsiderOnlyOneForEachPlayer()
        {
            Game.StartNextRound();
            var dustinRoundAnswers = new RoundAnswers(Game.Id, Game.CurrentRoundNumber, TestUsers.Dustin.Id,
                new List<Answer>
                {
                    new Answer("Nome", "Bruno"),
                    new Answer("CEP", "Brasil")
                });

            var willRoundAnswers = new RoundAnswers(Game.Id, Game.CurrentRoundNumber, TestUsers.Will.Id,
                new List<Answer>
                {
                    new Answer("Nome", "Bruno"),
                    new Answer("CEP", "Brasil")
                });

            var roundAnswers = new List<RoundAnswers>
            {
                dustinRoundAnswers,
                willRoundAnswers
            };

            var dustinDefaultValidationsForThemeNome = roundAnswers.BuildValidationsForPlayer(TestUsers.Dustin.Id, "Nome").ToList();
            var dustinDefaultValidationsForThemeCEP = roundAnswers.BuildValidationsForPlayer(TestUsers.Dustin.Id, "CEP").ToList();
            var willDefaultValidationsForThemeNome = roundAnswers.BuildValidationsForPlayer(TestUsers.Will.Id, "Nome").ToList();
            var willDefaultValidationsForThemeCEP = roundAnswers.BuildValidationsForPlayer(TestUsers.Will.Id, "CEP").ToList();

            Assert.AreEqual(1, dustinDefaultValidationsForThemeNome.Count());
            Assert.AreEqual(1, dustinDefaultValidationsForThemeCEP.Count());
            Assert.True(dustinDefaultValidationsForThemeNome.Any(v => v.Answer == new Answer("Nome", "Bruno")));
            Assert.True(dustinDefaultValidationsForThemeCEP.Any(v => v.Answer == new Answer("CEP", "Brasil")));

            Assert.AreEqual(1, willDefaultValidationsForThemeNome.Count());
            Assert.AreEqual(1, willDefaultValidationsForThemeCEP.Count());
            Assert.True(willDefaultValidationsForThemeNome.Any(v => v.Answer == new Answer("Nome", "Bruno")));
            Assert.True(willDefaultValidationsForThemeCEP.Any(v => v.Answer == new Answer("CEP", "Brasil")));
        }

        [Test]
        public void AnswerOfPlayerShouldNotBeIncludedInHisValidations()
        {
            Game.StartNextRound();
            var dustinRoundAnswers = new RoundAnswers(Game.Id, Game.CurrentRoundNumber, TestUsers.Dustin.Id,
                new List<Answer>
                {
                    new Answer("Nome", "Bruno"),
                    new Answer("CEP", "Brasil")
                });

            var willRoundAnswers = new RoundAnswers(Game.Id, Game.CurrentRoundNumber, TestUsers.Will.Id,
                new List<Answer>
                {
                    new Answer("Nome", "Bruna"),
                    new Answer("CEP", "Brasilia")
                });

            var roundAnswers = new List<RoundAnswers>
            {
                dustinRoundAnswers,
                willRoundAnswers
            };

            var dustinDefaultValidationsForThemeNome = roundAnswers.BuildValidationsForPlayer(TestUsers.Dustin.Id, "Nome").ToList();
            var dustinDefaultValidationsForThemeCEP = roundAnswers.BuildValidationsForPlayer(TestUsers.Dustin.Id, "CEP").ToList();
            var willDefaultValidationsForThemeNome = roundAnswers.BuildValidationsForPlayer(TestUsers.Will.Id, "Nome").ToList();
            var willDefaultValidationsForThemeCEP = roundAnswers.BuildValidationsForPlayer(TestUsers.Will.Id, "CEP").ToList();

            Assert.AreEqual(1, dustinDefaultValidationsForThemeNome.Count());
            Assert.AreEqual(1, dustinDefaultValidationsForThemeCEP.Count());
            Assert.True(dustinDefaultValidationsForThemeNome.Any(v => v.Answer == new Answer("Nome", "Bruna")));
            Assert.True(dustinDefaultValidationsForThemeCEP.Any(v => v.Answer == new Answer("CEP", "Brasilia")));
            Assert.False(dustinDefaultValidationsForThemeNome.Any(v => v.Answer == new Answer("Nome", "Bruno")));
            Assert.False(dustinDefaultValidationsForThemeCEP.Any(v => v.Answer == new Answer("CEP", "Brasil")));

            Assert.AreEqual(1, willDefaultValidationsForThemeNome.Count());
            Assert.AreEqual(1, willDefaultValidationsForThemeCEP.Count());
            Assert.True(willDefaultValidationsForThemeNome.Any(v => v.Answer == new Answer("Nome", "Bruno")));
            Assert.True(willDefaultValidationsForThemeCEP.Any(v => v.Answer == new Answer("CEP", "Brasil")));
            Assert.False(willDefaultValidationsForThemeNome.Any(v => v.Answer == new Answer("Nome", "Bruna")));
            Assert.False(willDefaultValidationsForThemeCEP.Any(v => v.Answer == new Answer("CEP", "Brasilia")));
        }
    }
}