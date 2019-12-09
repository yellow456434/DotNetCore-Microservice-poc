using System.Threading.Tasks;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Custom
{
    public class WebcommAuthorizeResponseGenerator : AuthorizeResponseGenerator
    {
        public WebcommAuthorizeResponseGenerator(
            ISystemClock clock,
            ITokenService tokenService,
            IKeyMaterialService keyMaterialService,
            IAuthorizationCodeStore authorizationCodeStore,
            ILogger<AuthorizeResponseGenerator> logger,
            IEventService events):base(clock, tokenService, keyMaterialService, authorizationCodeStore,
                logger, events){ }

        protected override async Task<AuthorizeResponse> CreateCodeFlowResponseAsync(ValidatedAuthorizeRequest request)
        {
            Logger.LogDebug("Creating Authorization Code Flow response123.");

            var code = await CreateCodeAsync(request);
            var id = await AuthorizationCodeStore.StoreAuthorizationCodeAsync(code);

            request.ValidatedScopes.GrantedResources.ApiResources.Clear();
            request.ValidatedScopes.GrantedResources.IdentityResources.Clear();
            request.ValidatedScopes.GrantedResources.OfflineAccess = false;

            var response = new AuthorizeResponse
            {
                Request = request,
                Code = id,
                SessionState = request.GenerateSessionStateValue()
            };

            return response;
        }
    }
}
