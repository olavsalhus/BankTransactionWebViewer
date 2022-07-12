using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        // GET: api/<ValuesController1>
        [HttpGet]
        public async Task<string> Get(string @ref)
        {
            var requisition_id = @ref;

            var httpClient = new HttpClient();
            var token = await Service.NordigenService.GetAccessToken(httpClient);
            using var request = new HttpRequestMessage(new HttpMethod("GET"), $"https://ob.nordigen.com/api/v2/requisitions/{requisition_id}/");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token.access}");


            var response = await httpClient.SendAsync(request);
            var test = await response.Content.ReadAsStringAsync();

            return test;
        }

    }
}
