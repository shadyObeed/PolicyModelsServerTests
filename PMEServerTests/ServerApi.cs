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
        public ServerApi() : base($"http://localhost:9000/apiInterviewCtrl/")
        {
        }

        public async Task<string> GetModels()
        {
            var endpoint = $"models/";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var res = await response.Content.ReadAsStringAsync();
            return res;
        }


        public async Task<string> GetModelLanguages(string modelId)
        {
            var endpoint = $"{modelId}/start";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<JObject> StartInterview(string modelId, int versionId, string languageId)
        {
            var endpoint = $"{modelId}/{versionId}/{languageId}/start";
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
        
        public async Task<JObject> AnswerQuestion(string uuid, string modelId, int versionId, string languageId,
            string questionId, string answer)
        {
            var content = getAnswerContent(uuid, modelId, versionId, languageId, questionId, answer);
            var endpoint = $"answer/";
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

        public async Task<string> GetLanguages(string modelId)
        {
            var endpoint = $"{modelId}/start";
            HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
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