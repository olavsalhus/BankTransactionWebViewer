public class NordigenService
{
    private readonly IConfiguration Configuration;
    public NordigenService(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public async Task<string> GetAccessToken(IHttpClientFactory httpClientFactory)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://ob.nordigen.com/api/v2/token/new/");

        request.Content = JsonContent.Create(new {
            secret_id = Configuration["Nordigen:secret_id"],
            secret_key = Configuration["Nordigen:secret_key"]
        });

        return await (await httpClientFactory.CreateClient().SendAsync(request)).Content.ReadAsStringAsync();
    }

    public async Task<string> GetRequisition(IHttpClientFactory httpClientFactory, string access_token, string redirect, string institution_id)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://ob.nordigen.com/api/v2/requisitions/");

        request.Content = JsonContent.Create(new { redirect, institution_id, user_language = "EN" });
        request.Headers.Add("Authorization", $"Bearer {access_token}");

        return await (await httpClientFactory.CreateClient().SendAsync(request)).Content.ReadAsStringAsync();
    }
}
