using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Prode2022Server.Services;
using Prode2022Server.Models;
using Prode2022Server.Helpers;

namespace Prode2022Server.Security
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {

        public ILocalStorageService _localStorageService { get; }
        private readonly SecurityServices securityServices;
        public CustomAuthProvider(ILocalStorageService localStorageService, 
            SecurityServices securityServices)
        {
            _localStorageService = localStorageService;
            this.securityServices = securityServices;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {   
            try 
            {
                string accessToken = await _localStorageService.GetItemAsync<string>("accessToken"); 
                ClaimsIdentity identity = new ClaimsIdentity();

                if (accessToken != null && accessToken != string.Empty)
                {
                    UserLogin user = await securityServices.GetUserByAccessTokenAsync(accessToken);
                    if (user.IsExpired)
                    {
                        //get user from refresh token
                        accessToken = await _localStorageService.GetItemAsync<string>("refreshToken");
                        user = await securityServices.GetUserByRefreshTokenAsync(accessToken);
                        //If user is ok, has to make a new token and refresh.
                        if (user.LoggedIn)
                        {
                            user.AccessToken = securityServices.GenerateAccessToken(user);
                            user.RefreshToken = accessToken;
                        }
                    }
                    if (user.LoggedIn)
                    {
                        identity = new(securityServices.GenerateClaimsForUser(user), "apiauth_type");
                    }
                }        
                var claimsPrincipal = new ClaimsPrincipal(identity);            
                return await Task.FromResult(new AuthenticationState(claimsPrincipal));
            }
            catch 
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            }
                      
            
            
        }

        public async Task MarkUserAsAuthenticated(UserLogin user)
        {
            await _localStorageService.SetItemAsync("accessToken", user.AccessToken);
            await _localStorageService.SetItemAsync("refreshToken", user.RefreshToken);

            ClaimsIdentity identity = new(securityServices.GenerateClaimsForUser(user), "apiauth_type");

            var claimsPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorageService.RemoveItemAsync("refreshToken");
            await _localStorageService.RemoveItemAsync("accessToken");

            var identity = new ClaimsIdentity();

            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private ClaimsIdentity GetClaimsIdentity(UserLogin user)
        {
            var claimsIdentity = new ClaimsIdentity();

            if (user.Email != null)
            { 
                claimsIdentity = new ClaimsIdentity(new[]
                                {
                                    new Claim(ClaimType.Mail, user.Email),                                   
                                    new Claim(ClaimType.Id, user.Id.ToString())
                                }, "apiauth_type");
            }

            return claimsIdentity;
        }

    }
}