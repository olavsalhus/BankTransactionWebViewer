using BankWebApi.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace BankWebApi.Service
{
    public static class NordigenService
    {
        public const string SECRET_ID = "527a5fda-d3d5-4363-b52f-0d62b81ce623";
        public const string SECRET_KEY = "5e2f1b44c8c333bffb4adcf853c41bf921236f73c146b8f35bf5623f2d01e54df23b4ed1f845339e0f445bbeaa30eaf7548eff1238761daa8c0ae1b123e9a185";
        public static async Task<Token> GetAccessToken(HttpClient httpClient)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://ob.nordigen.com/api/v2/token/new/");
            request.Headers.Add("accept", "application/json");

            request.Content = new StringContent($@"{{ ""secret_id"": ""{SECRET_ID}"", ""secret_key"": ""{SECRET_KEY}"" }}");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await httpClient.SendAsync(request);
            var x = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Token>(x);
        }
    }
}
