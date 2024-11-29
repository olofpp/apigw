namespace GetJwks;

using ITest;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

public class HttpConfiguration : IHttpConfiguration
{
    private readonly IConfiguration _configuration;

    public HttpConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<JsonWebKey> ReturnMessage()
    {
        try
        {
            using var httpClient = new HttpClient();
            var jwksUrl = _configuration["JwtSettings:JwksUrl"];
            var response = await httpClient.GetAsync(jwksUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Unable to fetch JWKS keys: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var jwks = JsonConvert.DeserializeObject<JsonWebKeySet>(json);
            return jwks.Keys.FirstOrDefault();
        }
        catch (HttpRequestException ex)
        {
            // Log the exception and return a default or null key
            Console.WriteLine($"Error fetching JWKS keys: {ex.Message}");
            return null;
        }
    }
}
