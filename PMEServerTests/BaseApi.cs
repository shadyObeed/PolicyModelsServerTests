using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TestProject1
{
    public class BaseApi
    {
        private protected readonly HttpClient HttpClient;
        //private protected readonly IExceptionFactory ExceptionFactory;
        private readonly Uri _baseUrl;
        protected BaseApi( string baseUrl)
        {
            HttpClient = new HttpClient();
            _baseUrl = new Uri(baseUrl);
            HttpClient.BaseAddress = _baseUrl;
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //ExceptionFactory = exceptionFactory;
        }
        
    }
}