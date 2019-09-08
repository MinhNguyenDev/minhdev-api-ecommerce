using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Minhdev.Api.Ecommerce
{
    public abstract class RestApiClient
    {
        private HttpClient _httpClient;

        public RestApiClient(string baseAddress, int timeOutMilliseconds = 2000)
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(baseAddress),
                Timeout = TimeSpan.FromMilliseconds(timeOutMilliseconds)
            };
        }

        public async Task<T> GetAsync<T>(ICollection<KeyValuePair<string, string>> requestData, string uriPath)
        {
            return await SendWithQueryParamAsync<T>(requestData, "GET", uriPath);
        }

        public async Task<string> PostAsync(
            ICollection<KeyValuePair<string, string>> requestParams,
            object requestData, string uriPath)
        {
            return await SendWithContentAsync(requestParams, requestData, "POST", uriPath);
        }

        private async Task<string> SendWithContentAsync(
            ICollection<KeyValuePair<string, string>> requestParams,
            object requestData,
            string httpMethod,
            string uriPath)
        {
            var queryString = BuildQueryString(requestParams);
            if (!string.IsNullOrEmpty(queryString))
            {
                uriPath = $"{uriPath}?{queryString}";
            }

            var payload = JsonConvert.SerializeObject(requestData);

            var request = new HttpRequestMessage(new HttpMethod(httpMethod), uriPath)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status Code: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return result;
        }

        private async Task<T> SendWithQueryParamAsync<T>(
            ICollection<KeyValuePair<string, string>> requestData,
            string httpMethod,
            string uriPath)
        {
            var queryString = BuildQueryString(requestData);
            if (!string.IsNullOrEmpty(queryString))
            {
                uriPath = $"{uriPath}?{queryString}";
            }

            var request = new HttpRequestMessage(new HttpMethod(httpMethod), uriPath);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Status Code: {response.StatusCode}");
            }

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(result);
        }

        private string BuildQueryString(ICollection<KeyValuePair<string, string>> requestData)
        {
            var result = string.Empty;
            if (!requestData.Any())
            {
                return result;
            }

            foreach (var item in requestData)
            {
                result += $"{item.Key}={item.Value}&";
            }

            return result.Remove(result.Length - 1, 1);
        }
    }
}
