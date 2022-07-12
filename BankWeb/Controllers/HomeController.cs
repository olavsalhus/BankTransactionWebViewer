using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using BankWeb.Services;

namespace BankWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly NordigenService _nordigenService;
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, HttpClient httpClient, NordigenService nordigenService)
        {
            _nordigenService = nordigenService;
            _logger = logger;
            _httpClient = httpClient;
        }

        public ViewResult Index() => View();
        public ViewResult Transactions(string @ref) => View(model: @ref);

        public async Task<ActionResult> GetAccessToken() => Content(await _nordigenService.GetAccessToken(_httpClient), "application/json");

        public async Task BankLogin(string bank) {
            var token = JsonNode.Parse(await _nordigenService.GetAccessToken(_httpClient));
            var redirectUrl = $"{Request.Scheme}://{Request.Host}/Home/Transactions";
            
            var requisition = await _nordigenService.GetRequisition(_httpClient, token["access"].ToString(), redirectUrl, bank);
            Response.Redirect(JsonNode.Parse(requisition)["link"].ToString());
        }

        public async Task<ActionResult> GetBanks(string access_token, string country) =>
            Content(await Get($"institutions/?country={country}", access_token), "application/json");

        public async Task<ActionResult> ListBankAccounts(string reference, string access_token)
        {
            var accounts = JsonNode.Parse(await Get($"requisitions/{reference}/", access_token))["accounts"].AsArray();

            var accountObjects = await Task.WhenAll(
                accounts.Select(async accountId =>
                {
                    string metadata = await Get($"accounts/{accountId}/", access_token);
                    //string details = await Get($"accounts/{accountId}/details/", access_token); @"""details"": {details},"
                    string balances = (await Get($"accounts/{accountId}/balances/", access_token)).Trim()[1..^1];
                    string transactions = (await Get($"accounts/{accountId}/transactions/", access_token)).Trim()[1..^1];
                    return @$"
                    {{
                        ""id"": ""{accountId}"", 
                        ""metadata"": {metadata},
                        {balances},
                        {transactions}
                    }}";
                })
            );
            
            return Content(@$"[{string.Join(", ", accountObjects)}]", "application/json");
        }

        public async Task<string> Get(string url, string access_token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"https://ob.nordigen.com/api/v2/{url}");
            request.Headers.Add("Authorization", $"Bearer {access_token}");

            return await (await _httpClient.SendAsync(request)).Content.ReadAsStringAsync();
        }
    }
}