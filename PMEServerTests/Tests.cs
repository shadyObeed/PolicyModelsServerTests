using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class Tests
    {
        private ServerApi _serverApi;

        [SetUp]
        public void Setup()
        {
            _serverApi = new ServerApi();
        }

        [Test]
        public async Task TestServerStatus()
        {
            await FluentActions.Invoking(() => _serverApi.GetModels()).Should().NotThrowAsync<Exception>();
        }

        [Test]
        public async Task GetModelsHappy()
        {
            var models = await _serverApi.GetModels();
            Assert.IsNotEmpty(models);
        }

        [Test]
        public async Task GetModelLanguagesHappy()
        {
            var models = await _serverApi.GetModelLanguages("1");
            Assert.IsNotEmpty(models);
        }

        [Test]
        public async Task GetModelLanguagesSad()
        {
            await FluentActions.Invoking(() => _serverApi.GetModelLanguages("notExistModelId")).Should()
                .ThrowAsync<Exception>();
        }

        [Test]
        public async Task StartInterviewHappy()
        {
            var res = await _serverApi.StartInterview("1", 1, "en-US");
            var userId = res["ssid"].ToObject<string>();
            var questionId = res["questionId"].ToObject<string>();
            var questionText = res["questionText"].ToObject<string>();
            Assert.IsNotEmpty(userId);
            Assert.IsNotEmpty(questionId);
            Assert.IsNotEmpty(questionText);
        }

        [Test]
        public async Task StartInterviewSad()
        {
            await FluentActions.Invoking(() => _serverApi.StartInterview("NotExistModelId", 1, "en-US")).Should()
                .ThrowAsync<Exception>();
            await FluentActions.Invoking(() => _serverApi.StartInterview("1", -5, "en-US")).Should()
                .ThrowAsync<Exception>();
        }

        [Test]
        public async Task AnswerHappy()
        {
            var InterviewData = await _serverApi.StartInterview("1", 1, "en-US");
            var userId = InterviewData["ssid"].ToObject<string>();
            var questionId = InterviewData["questionId"].ToObject<string>();
            var questionText = InterviewData["questionText"].ToObject<string>();
            var res = await _serverApi.AnswerQuestion(userId, "1", 1, "en-US", questionId, "yes");
            var resultuserId = res["ssid"].ToObject<string>();
            var nextQuestionId = res["questionId"].ToObject<string>();
            var nextQuestionText = res["questionText"].ToObject<string>();
            Assert.AreEqual(userId, resultuserId);
            Assert.AreNotEqual(questionId, nextQuestionId);
            Assert.AreNotEqual(questionText, nextQuestionText);
        }

        [Test]
        public async Task AnswerBad()
        {
            var InterviewData = await _serverApi.StartInterview("1", 1, "en-US");
            var userId = InterviewData["ssid"].ToObject<string>();
            var questionId = InterviewData["questionId"].ToObject<string>();
            var questionText = InterviewData["questionText"].ToObject<string>();
            await FluentActions
                .Invoking(() => _serverApi.AnswerQuestion("otherUserId", "1", 1, "en-US", questionId, "yes")).Should()
                .ThrowAsync<Exception>();
        }

        [Test]
        public async Task ChnageTextLanguage()
        {
            var EngInterview = await _serverApi.StartInterview("1", 1, "en-US");
            var userId = EngInterview["ssid"].ToObject<string>();
            var questionId = EngInterview["questionId"].ToObject<string>();

            //ans and request english language text
            var engAnswer = await _serverApi.AnswerQuestion(userId, "1", 1, "en-US", questionId, "yes");
            var questionTextEng = engAnswer["questionText"].ToObject<string>();

            //ans and request hebrew language text
            var heAnswer = await _serverApi.AnswerQuestion(userId, "1", 1, "he-IL", questionId, "yes");
            var questionTextHe = heAnswer["questionText"].ToObject<string>();

            Assert.AreNotEqual(questionTextEng, questionTextHe);
        }

        [Test]
        public async Task AnswerPreviousQuestion()
        {
            var EngInterview = await _serverApi.StartInterview("1", 1, "en-US");
            var userId = EngInterview["ssid"].ToObject<string>();
            var questionId = EngInterview["questionId"].ToObject<string>();

            //ans and request english language text
            var firstTimeAnswer = await _serverApi.AnswerQuestion(userId, "1", 1, "en-US", questionId, "yes");
            var questionText1 = firstTimeAnswer["questionText"].ToObject<string>();
            var nextQuestionId = firstTimeAnswer["questionId"].ToObject<string>();

            //ans and request hebrew language text
            var secendTimeAnswer = await _serverApi.AnswerQuestion(userId, "1", 1, "he-IL", questionId, "no");
            var questionText2 = secendTimeAnswer["questionText"].ToObject<string>();
            var nextQuestionId2 = secendTimeAnswer["questionId"].ToObject<string>();

            Assert.AreNotEqual(nextQuestionId, nextQuestionId2);
            Assert.AreNotEqual(questionText1, questionText2);
        }

        [Test]
        public async Task DifferentUsersDifferentId()
        {
            var user1 = await _serverApi.StartInterview("1", 1, "en-US");
            var user1Id = user1["ssid"].ToObject<string>();
            var user2 = await _serverApi.StartInterview("1", 1, "en-US");
            var user2Id = user2["ssid"].ToObject<string>();
            Assert.AreNotEqual(user1Id, user2Id);
        }
    }
}