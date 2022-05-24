using System;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
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
            var res = await _serverApi.StartInterview("1", "1", "en-US");
            var userId = res["ssid"].ToObject<string>();
            var questionId = res["questionId"].ToObject<string>();
            var questionText = res["questionText"].ToObject<string>();
            Assert.IsNotEmpty(userId);
            Assert.IsNotEmpty(questionId);
            Assert.IsNotEmpty(questionText);
            var answerResultfirstQuestion = await _serverApi.AnswerWithGet(userId, "1", "1","en-US","0","1");
            var answerResultfirstQuestion2 = await _serverApi.AnswerWithGet(userId, "1", "1","he-IL","1","0");
            var answerResultfirstQuestionasd = await _serverApi.AnswerWithGet(userId, "1", "1","en-US","2","1");
            var askResult = await _serverApi.GetHistory(userId, "1", "1","en-US","2");
            
            var ansquestionID = answerResultfirstQuestion["questionId"].ToObject<string>();
            var tags = await _serverApi.GetTags(userId);
            var ansquestionText = answerResultfirstQuestion["questionText"].ToObject<string>();

        }

        [Test]
        public async Task StartInterviewSad()
        {
            await FluentActions.Invoking(() => _serverApi.StartInterview("NotExistModelId", "1", "en-US")).Should()
                .ThrowAsync<Exception>();
            await FluentActions.Invoking(() => _serverApi.StartInterview("1", "-5", "en-US")).Should()
                .ThrowAsync<Exception>();
        }

        [Test]
        public async Task AnswerHappy()
        {
            var InterviewData = await _serverApi.StartInterview("1", "1", "en-US");
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
            var InterviewData = await _serverApi.StartInterview("1", "1", "en-US");
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
            var EngInterview = await _serverApi.StartInterview("1", "1", "en-US");
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
            var EngInterview = await _serverApi.StartInterview("1", "1", "en-US");
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
            var user1 = await _serverApi.StartInterview("1", "1", "en-US");
            var user1Id = user1["ssid"].ToObject<string>();
            var user2 = await _serverApi.StartInterview("1", "1", "en-US");
            var user2Id = user2["ssid"].ToObject<string>();
            Assert.AreNotEqual(user1Id, user2Id);
        }


        [Test]
        public async Task FullInterview()
        {
            //get the second model example
            var models = await _serverApi.GetModels();
            var interviewId = models[0]["id"].ToString();
            var interviewTitle = models[0]["title"].ToString();
            var interviewVersionId = models[0]["versionId"].ToString();
            
            
            //get model languages
            var languages = await _serverApi.GetModelLanguages(interviewId);
            var engLanguage = languages[2].ToString();
            
            //start interview
            var res = await _serverApi.StartInterview(interviewId, interviewVersionId, engLanguage);
                /*you can update this values on every time you answer any question*/
                var userId = res["ssid"].ToString();
                var questionId = res["questionId"].ToString();
                var questionText = res["questionText"].ToString();
                var answers = res["Answers"].ToString();
                //he index of my answer in answers list
                var answerIndex = "1";
                var answersInYourLanguage = res["AnswersInYourLanguage"].ToString();
                var finished = res["finished"].ToString();
                var tags = res["tags"].ToString();
            
            //answer questionId
            var answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            questionId = answer["questionId"].ToString();
            questionId = answer["questionId"].ToString();
            questionText = answer["questionText"].ToString();
            answers = answer["Answers"].ToString();
            //the index of my answer in the next question you can choose it
            answerIndex = "0";
            answersInYourLanguage = answer["AnswersInYourLanguage"].ToString();
            finished = answer["finished"].ToString();
            tags = answer["tags"].ToString();
            

            //answer some questions example
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            questionId = answer["questionId"].ToString();
            answerIndex = "1";
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            questionId = answer["questionId"].ToString();
            answerIndex = "0";
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            questionId = answer["questionId"].ToString();
            answerIndex = "1";
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            questionId = answer["questionId"].ToString();
            answerIndex = "0";
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            questionId = answer["questionId"].ToString();
            answerIndex = "1";
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            questionId = answer["questionId"].ToString();
            answerIndex = "0";
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            questionId = answer["questionId"].ToString();
            answerIndex = "1";
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            questionId = answer["questionId"].ToString();
            answerIndex = "0";
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage, questionId, answerIndex);
            
            //here finished is true and this is how to extract the elements from it
            finished = answer["finished"].ToString(); //this is true
            //you need this two parsed jsons to the final output (**** one is jArray and one is jObject ****)
            var finalTags = JObject.Parse(answer["tags"].ToString());
            var answerHistory = JArray.Parse(answer["answerHistory"].ToString());
            
            //Get Current tags
            var currentTagsExample = await _serverApi.GetTags(userId);
            
            //GetHistory => returns back the tags also 
            var returnOrGetHistory = await _serverApi.GetHistory(userId, interviewId, interviewVersionId, engLanguage, "2");
            answerHistory = JArray.Parse(returnOrGetHistory["answerHistory"].ToString());
            tags = returnOrGetHistory["tags"].ToString();
            //you can get the question text and and answers as I show above....

            
            //answer qusetion 2 question after we resurned to it
            answer = await _serverApi.AnswerWithGet(userId, interviewId, interviewVersionId, engLanguage,  returnOrGetHistory["questionId"].ToString(), answerIndex);
            var shady = "";
        }

    }
}