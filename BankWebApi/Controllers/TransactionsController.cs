using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        // GET: api/Transactions
        [HttpGet("{account}")]
        public async Task<string> GetAsync(string account)
        {
            var httpClient = new HttpClient();
            var token = await Service.NordigenService.GetAccessToken(httpClient);
            using var request = new HttpRequestMessage(new HttpMethod("GET"), $"https://ob.nordigen.com/api/v2/accounts/{account}/transactions/");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token.access}");


            var response = await httpClient.SendAsync(request);
            var test = await response.Content.ReadAsStringAsync();

            return test;
        }

    }
}
