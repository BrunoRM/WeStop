using NUnit.Framework;
using WeStop.Api.Infra.Storages.InMemory;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.UnitTest.Storages
{
    [TestFixture]
    public class InMemoryAnswerStorageTests
    {
        private readonly IAnswerStorage _answerStorage;

        public InMemoryAnswerStorageTests()
        {
            _answerStorage = new AnswerStorage();
        }

        [Test]
        public void ReturnAnswersOfSpecificPlayer()
        {

        }
    }
}