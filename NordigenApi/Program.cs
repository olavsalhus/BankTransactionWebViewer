// See https://aka.ms/new-console-template for more information
using NordigenApi;
using System.Net.Http.Headers;
using System.Text.Json;

using var httpClient = new HttpClient();


//https://nordigen.com/en/account_information_documenation/integration/quickstart_guide/

// Step 1: Get access token
Token token;
{
    using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://ob.nordigen.com/api/v2/token/new/");
    request.Headers.Add("accept", "application/json");

    request.Content = new StringContent($@"{{ ""secret_id"": ""527a5fda-d3d5-4363-b52f-0d62b81ce623"", ""secret_key"": ""5e2f1b44c8c333bffb4adcf853c41bf921236f73c146b8f35bf5623f2d01e54df23b4ed1f845339e0f445bbeaa30eaf7548eff1238761daa8c0ae1b123e9a185"" }}");
    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

    var response = await httpClient.SendAsync(request);
    var x = await response.Content.ReadAsStreamAsync();
    token = await JsonSerializer.DeserializeAsync<Token>(x);
}


// Step 2: List banks
using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://ob.nordigen.com/api/v2/institutions/?country=no"))
{
    request.Headers.Add("accept", "application/json");
    request.Headers.Add("Authorization", $"Bearer {token.access}");

    var response = await httpClient.SendAsync(request);
    var x2 = await response.Content.ReadAsStringAsync();
    //var token2 = await JsonSerializer.DeserializeAsync<Token>(x2);
}

var BANK_IDentifier = "SBANKEN_SBAKNOBB";
