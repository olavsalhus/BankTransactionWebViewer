using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using BankWeb.Services;

namespace BankWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public IActionResult Index() => View();

        public async Task BankLogin(string bank) {
            var token = JsonNode.Parse(await NordigenService.GetAccessToken(_httpClient));
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/Home/Transactions";

            var requisition = await NordigenService.GetRequisition(_httpClient, token["access"].ToString(), redirectUrl, bank);
            var link = JsonNode.Parse(requisition)["link"].ToString();
            Response.Redirect(link);
            await Response.StartAsync();
        }

        public async Task<ActionResult> GetAccessToken() => Content(await NordigenService.GetAccessToken(_httpClient), "application/json");

        public async Task<ActionResult> GetBanks(string access_token, string country) =>
            Content(await Get(
                $"institutions/{(string.IsNullOrWhiteSpace(country) ? "" : $"?country={country}")}"
                , access_token), "application/json");

        public IActionResult Transactions(string @ref) => View("Transactions", model: @ref);

        public async Task<ActionResult> ListBankAccounts(string reference, string access_token)
        {
            var accounts = JsonNode.Parse(await Get($"requisitions/{reference}/", access_token))["accounts"].AsArray();

            var accountObjects = await Task.WhenAll(
                accounts.Select(async accountId => @$"
                    {{
                        ""id"": ""{accountId}"", 
                        ""metadata"": {await Get($"accounts/{accountId}/", access_token)},
                        ""details"": {await Get($"accounts/{accountId}/details/", access_token)},
                        {(await Get($"accounts/{accountId}/balances/", access_token))[1..^1]},
                        {(await Get($"accounts/{accountId}/transactions/", access_token))[1..^1]}
                    }}"
                )
            );
            
            return Content(@$"[{string.Join(", ", accountObjects)}]", "application/json");
        }

        public async Task<string> Get(string url, string access_token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"https://ob.nordigen.com/api/v2/{url}");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {access_token}");

            var response = await _httpClient.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}