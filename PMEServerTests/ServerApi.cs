using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestProject1
{
    public class ServerApi : BaseApi
    {
        public ServerApi() : base($"https://policymodelsserver.azurewebsites.net/apiInterviewCtrl/")
        {
        }

        public async Task<JArray> GetModels()
        {
            var endpoint = $"models/";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsStringAsync();
            return JArray.Parse(res);
        }

        public async Task<JArray> GetModelLanguages(string modelId)
        {
            var endpoint = $"{modelId}/start/";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsStringAsync();
            return JArray.Parse(res);
        }
        

        public async Task<JObject> StartInterview(string modelId, string versionId, string languageId)
        {
            var endpoint = $"{modelId}/{versionId}/{languageId}/start/";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }
        public async Task<JObject> GetTags(string userID,string modelId,string versionId,string languageId)
        {
            var endpoint = $"/apiInterviewCtrl/getTags/{userID}/{modelId}/{versionId}/{languageId}/";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<JObject> GetQuestion(string uuid, string modelId, int versionId, string languageId,
            string questionId)
        {
            var endpoint = $"ask/";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }
        public async Task<JObject> AnswerWithGet(string uuid, string modelId, string versionId, string languageId,
            string questionId, string answer)
        {
            var endpoint = $"answer/{uuid}/{modelId}/{versionId}/{languageId}/{questionId}/{answer}/";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }
        
        public async Task<JObject> GetHistory(string uuid, string modelId, string versionId, string languageId,
            string questionId)
        {
            var endpoint = $"askHistory/{uuid}/{modelId}/{versionId}/{languageId}/{questionId}/";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }
        
        public async Task<JObject> AnswerQuestion(string uuid, string modelId, int versionId, string languageId,
            string questionId, string answer)
        {
            var content = getAnswerContent(uuid, modelId, versionId, languageId, questionId, answer);
            var endpoint = $"answerPost/";
            HttpResponseMessage response = null;
            try
            {
                response = await HttpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                return JObject.Parse(await response.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private HttpContent getAnswerContent(string uuid, string modelId, int versionId, string languageId,
            string questionId, string answer)
        {
            var contentBody = new CreateCommandContent
            {
                uuid = uuid,
                modelId = modelId,
                versionNum = versionId.ToString(),
                languageId = languageId,
                reqNodeId = questionId,
                answer = answer
            };

            string contentStr = JsonConvert.SerializeObject(contentBody);

            HttpContent content = new StringContent(contentStr, Encoding.UTF8, "application/json");
            return content;
        }


    }

    public class CreateCommandContent
    {
        public string uuid { get; set; }
        public string modelId { get; set; }
        public string versionNum { get; set; }
        public string reqNodeId { get; set; }
        public string answer { get; set; }
        public string languageId { get; set; }
    }
}