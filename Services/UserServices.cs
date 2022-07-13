using Prode2022Server.Models;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

namespace Prode2022Server.Services;

public class UserServices
{
    private readonly JWTSettings _jwtsettings;

    public UserServices(
        JWTSettings jwtsettings
    )
    {
        _jwtsettings = jwtsettings;
    }

    public async Task<UserLogin> GetUserByAccessTokenAsync(string accessToken)
    {
        //Serialize token
        string serializedRefreshRequest= JsonSerializer.Serialize(accessToken);
        //string serializedRefreshRequest = JsonConvert.SerializeObject(accessToken);
        //Call the database to know if the user is knowed

        /*var requestMessage = new HttpRequestMessage(HttpMethod.Post, "Users/GetUserByAccessToken");
        requestMessage.Content = new StringContent(serializedRefreshRequest);

        requestMessage.Content.Headers.ContentType
            = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        var response = await _httpClient.SendAsync(requestMessage);*/
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
        SecurityToken securityToken;
        var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
        JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

        if (jwtSecurityToken != null 
            && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            var userId = principle.FindFirst(ClaimTypes.Name)?.Value;

            //aca deberia buscar y retornar el usuario
            return 
        }

        var responseStatusCode = response.StatusCode;
        var responseBody = await response.Content.ReadAsStringAsync();

        var returnedUser = JsonConvert.DeserializeObject<User>(responseBody);

        return await Task.FromResult(returnedUser);
    }
}