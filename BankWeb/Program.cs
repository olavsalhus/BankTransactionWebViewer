using System.Text.Json.Nodes;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient().AddRazorPages();

var app = builder.Build();

#region Minimal API
app.MapGet("GetAccessToken", async (HttpContext ctx, IHttpClientFactory httpClientFactory, IConfiguration configuration) =>
    await ctx.Response.WriteAsync(await GetAccessToken(httpClientFactory, configuration))
);

app.MapGet("GetBanks", async (HttpContext ctx, IHttpClientFactory httpClientFactory, IConfiguration configuration, string? country, string? access_token) =>
    await ctx.Response.WriteAsync(await Get($"institutions/?country={country}", access_token!, httpClientFactory))
);

app.MapGet("BankLogin", async (HttpContext ctx, IHttpClientFactory httpClientFactory, IConfiguration configuration, string? bank) =>
{
    var token = JsonNode.Parse(await GetAccessToken(httpClientFactory, configuration));
    var redirect = $"{ctx.Request.Scheme}://{ctx.Request.Host}/Transactions";
    using var request = new HttpRequestMessage(HttpMethod.Post, "https://ob.nordigen.com/api/v2/requisitions/");

    request.Content = JsonContent.Create(new { redirect, institution_id = bank, user_language = "EN" });
    request.Headers.Add("Authorization", $"Bearer {token?["access"]!}");

    var requisition = await (await httpClientFactory.CreateClient().SendAsync(request)).Content.ReadAsStringAsync();
    ctx.Response.Redirect(JsonNode.Parse(requisition)?["link"]?.ToString()!);
});

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
#endregion

app.UseStaticFiles().UseHsts();
app.MapRazorPages();
app.Run();

static async Task<string> Get(string url, string access_token, IHttpClientFactory httpClientFactory)
{
    using var request = new HttpRequestMessage(HttpMethod.Get, $"https://ob.nordigen.com/api/v2/{url}");
    request.Headers.Add("Authorization", $"Bearer {access_token}");

    return await (await httpClientFactory.CreateClient().SendAsync(request)).Content.ReadAsStringAsync();
}

static async Task<string> GetAccessToken(IHttpClientFactory httpClientFactory, IConfiguration configuration)
{
    using var request = new HttpRequestMessage(HttpMethod.Post, "https://ob.nordigen.com/api/v2/token/new/");

    request.Content = JsonContent.Create(new {
        secret_id = configuration["Nordigen:secret_id"],
        secret_key = configuration["Nordigen:secret_key"]
    });

    return await (await httpClientFactory.CreateClient().SendAsync(request)).Content.ReadAsStringAsync();
}