using System.Net.Http.Headers;

namespace BankWeb.Services
{
    public static class NordigenService
    {
        public const string SECRET_ID = "";
        public const string SECRET_KEY = "";
        public static async Task<string> GetAccessToken(HttpClient httpClient)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://ob.nordigen.com/api/v2/token/new/");
            request.Headers.Add("accept", "application/json");
            if (string.IsNullOrWhiteSpace(SECRET_ID) || string.IsNullOrWhiteSpace(SECRET_KEY))
            {
                throw new ArgumentNullException("Please fill in your SECRET_ID and SECRET_KEY from the Nordigen dashboard and add it to the file BankWeb/Services/NordigenService.cs");
            }
            request.Content = new StringContent($@"{{ ""secret_id"": ""{SECRET_ID}"", ""secret_key"": ""{SECRET_KEY}"" }}");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetRequisition(HttpClient httpClient, string access_token, string redirectUrl, string institution)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://ob.nordigen.com/api/v2/requisitions/");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {access_token}");

            request.Content = new StringContent(@$"{{  
                ""redirect"": ""{redirectUrl}"", 
                ""institution_id"": ""{institution}"",
                ""user_language"":""EN"" }}"
            );
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
