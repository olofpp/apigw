using Microsoft.IdentityModel.Tokens;

namespace ITest;

public interface IHttpConfiguration
{
    public Task<JsonWebKey> ReturnMessage();
}