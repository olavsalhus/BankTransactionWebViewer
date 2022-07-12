using BankWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        // GET: api/Values
        [HttpGet]
        public async Task<string> GetAsync()
        {
            var httpClient = new HttpClient();

            var token = await Service.NordigenService.GetAccessToken(httpClient);
            
            var institution_id = "SBANKEN_SBAKNOBB";

            var redirectUrl = "https://localhost:7226/api/redirect";


            using var request = new HttpRequestMessage(new HttpMethod("POST"), "https://ob.nordigen.com/api/v2/requisitions/");
            request.Headers.Add("accept", "application/json");
            request.Headers.Add("Authorization", $"Bearer {token.access}");
            
            request.Content = new StringContent(@$"{{  
      ""redirect"": ""{redirectUrl}"", 
      ""institution_id"": ""{institution_id}"",
      ""user_language"":""EN"" }}");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await httpClient.SendAsync(request);
            var test = await response.Content.ReadAsStringAsync();
            //var test =  await JsonSerializer.DeserializeAsync<Token>(x);


            return test;
        }

    }
}
