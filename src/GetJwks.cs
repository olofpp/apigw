namespace  GetJwks;

using ITest;
using Microsoft.IdentityModel.Tokens;

public class HttpConfiguration : IHttpConfiguration
{
    private readonly IConfiguration _configuration;
    public HttpConfiguration(IConfiguration configuration)
    {
        this._configuration = configuration;
    }    

    public async Task<JsonWebKey> ReturnMessage()
    {
        JsonWebKey jsonResponse;
        try
        {
            HttpClient client = new HttpClient();  
            using HttpResponseMessage response = await client.GetAsync(_configuration["JwtSettings:JwksUrl"]);
            
            jsonResponse = new JsonWebKey(await response.Content.ReadAsStringAsync());    
        }
        catch (System.Exception)
        {
            
            throw;
        }

        return jsonResponse;
    }
}
