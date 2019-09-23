using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using WeStop.Api.Domain;
using WeStop.Api.Extensions;

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

            var dustinDefaultValidations = roundAnswers.BuildValidationsForPlayer(TestUsers.Dustin.Id).ToList();
            var willDefaultValidations = roundAnswers.BuildValidationsForPlayer(TestUsers.Will.Id).ToList();

            Assert.False(dustinDefaultValidations.Any(v => v.Answer == new Answer("CEP", "")));
            Assert.True(dustinDefaultValidations.Any(v => v.Answer == new Answer("Nome", "Bruno")));

            Assert.False(willDefaultValidations.Any(v => v.Answer == new Answer("Nome", "")));
            Assert.True(willDefaultValidations.Any(v => v.Answer == new Answer("CEP", "Brasil")));
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

            var dustinDefaultValidations = roundAnswers.BuildValidationsForPlayer(TestUsers.Dustin.Id).ToList();
            var willDefaultValidations = roundAnswers.BuildValidationsForPlayer(TestUsers.Will.Id).ToList();

            Assert.AreEqual(2, dustinDefaultValidations.Count());
            Assert.AreEqual(2, willDefaultValidations.Count());

            Assert.True(dustinDefaultValidations.Any(v => v.Answer == new Answer("Nome", "Bruno")));
            Assert.True(dustinDefaultValidations.Any(v => v.Answer == new Answer("CEP", "Brasil")));

            Assert.True(willDefaultValidations.Any(v => v.Answer == new Answer("Nome", "Bruno")));
            Assert.True(willDefaultValidations.Any(v => v.Answer == new Answer("CEP", "Brasil")));
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

            var dustinDefaultValidations = roundAnswers.BuildValidationsForPlayer(TestUsers.Dustin.Id).ToList();
            var willDefaultValidations = roundAnswers.BuildValidationsForPlayer(TestUsers.Will.Id).ToList();

            Assert.AreEqual(2, dustinDefaultValidations.Count());
            Assert.AreEqual(2, willDefaultValidations.Count());

            Assert.True(dustinDefaultValidations.Any(v => v.Answer == new Answer("Nome", "Bruna")));
            Assert.True(dustinDefaultValidations.Any(v => v.Answer == new Answer("CEP", "Brasilia")));
            Assert.False(dustinDefaultValidations.Any(v => v.Answer == new Answer("Nome", "Bruno")));
            Assert.False(dustinDefaultValidations.Any(v => v.Answer == new Answer("CEP", "Brasil")));

            Assert.True(willDefaultValidations.Any(v => v.Answer == new Answer("Nome", "Bruno")));
            Assert.True(willDefaultValidations.Any(v => v.Answer == new Answer("CEP", "Brasil")));
            Assert.False(willDefaultValidations.Any(v => v.Answer == new Answer("Nome", "Bruna")));
            Assert.False(willDefaultValidations.Any(v => v.Answer == new Answer("CEP", "Brasilia")));
        }
    }
}