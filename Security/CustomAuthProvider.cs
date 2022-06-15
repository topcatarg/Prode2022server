using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace Prode2022Server.Security
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //Obtengo la cookie, a ver si puedo
            
            //var accessToken = await _localStorageService.GetItemAsync<string>("accessToken");           

            ClaimsIdentity identity;
            /*
                        if (accessToken != null && accessToken != string.Empty)
                        {
                            User user = await _userService.GetUserByAccessTokenAsync(accessToken);
                            identity = GetClaimsIdentity(user);
                        }
                        else
                        {*/
            identity = new ClaimsIdentity();
            //}          

            var claimsPrincipal = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));

        }
    }
}