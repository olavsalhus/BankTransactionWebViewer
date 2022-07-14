using System.Text.Json.Nodes;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient().AddSingleton<NordigenService>().AddRazorPages();

var app = builder.Build();

app.MapGet("GetAccessToken", async (HttpContext ctx, IHttpClientFactory httpClientFactory, NordigenService _nordigenService) =>
    await ctx.Response.WriteAsync(await _nordigenService.GetAccessToken(httpClientFactory.CreateClient()))
);

app.MapGet("BankLogin", async (HttpContext ctx, IHttpClientFactory httpClientFactory, NordigenService _nordigenService, string? bank) =>
{
    var httpClient = httpClientFactory.CreateClient();
    var token = JsonNode.Parse(await _nordigenService.GetAccessToken(httpClient));
    var redirectUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}/Transactions";
    
    var requisition = await _nordigenService.GetRequisition(httpClient, token?["access"]?.ToString()!, redirectUrl, bank!);
    ctx.Response.Redirect(JsonNode.Parse(requisition)?["link"]?.ToString()!);
});

app.MapGet("GetBanks", async (HttpContext ctx, IHttpClientFactory httpClientFactory, NordigenService _nordigenService, string? country, string? access_token) =>
    await ctx.Response.WriteAsync(await Get($"institutions/?country={country}", access_token!, httpClientFactory))
);

app.MapGet("ListBankAccounts", async (HttpContext ctx, IHttpClientFactory httpClientFactory, string? reference, string? access_token) =>
{
    var accounts = JsonNode.Parse(await Get($"requisitions/{reference}/", access_token!, httpClientFactory))?["accounts"]?.AsArray();

    var accountObjects = await Task.WhenAll(
        accounts!.Select(async accountId =>
        {
            string metadata = await Get($"accounts/{accountId}/", access_token!, httpClientFactory);
            //string details = await Get($"accounts/{accountId}/details/", access_token, httpClientFactory); @"""details"": {details},"
            string balances = (await Get($"accounts/{accountId}/balances/", access_token!, httpClientFactory)).Trim()[1..^1];
            string transactions = (await Get($"accounts/{accountId}/transactions/", access_token!, httpClientFactory)).Trim()[1..^1];
            return @$"
                {{
                    ""id"": ""{accountId}"", 
                    ""metadata"": {metadata},
                    {balances},
                    {transactions}
                }}";
        })
    );

    await ctx.Response.WriteAsync(@$"[{string.Join(", ", accountObjects)}]");
});

app.MapRazorPages();
app.Run();

static async Task<string> Get(string url, string access_token, IHttpClientFactory httpClientFactory)
{
    using var request = new HttpRequestMessage(HttpMethod.Get, $"https://ob.nordigen.com/api/v2/{url}");
    request.Headers.Add("Authorization", $"Bearer {access_token}");

    return await (await httpClientFactory.CreateClient().SendAsync(request)).Content.ReadAsStringAsync();
}