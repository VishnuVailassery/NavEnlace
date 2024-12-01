using EnsureThat;
using SEO.Optimize.Core.Interfaces;
using SEO.Optimize.Core.Models;
using SEO.Optimize.Postgres.Repository;
using SEO.Optimize.Web.Helpers;
using System.Security.Claims;

namespace SEO.Optimize.Web.Services
{
    public class AuthenticationHandlerService
    {
        private readonly IContentRepository contentRepository;

        public AuthenticationHandlerService(IContentRepository contentRepository)
        {
            EnsureArg.IsNotNull(contentRepository, nameof(contentRepository));
            this.contentRepository = contentRepository;
        }

        public async Task<int> EnsureUserExistsAsync(ClaimsIdentity claimsIdentity)
        {
            var userClaims = claimsIdentity;
            var email = userClaims.FindFirst(ClaimTypes.Email)?.Value;
            var user = await contentRepository.GetUserByMailId(email);
            if (user != null)
            {
                claimsIdentity.AddClaim(new Claim(type:"seopt_user", value:user.Id.ToString()));
                return user.Id;
            }

            var nameId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = userClaims.FindFirst(GoogleOidcClaims.Name)?.Value;          
            var picture = userClaims.FindFirst(GoogleOidcClaims.Picture)?.Value ?? string.Empty;               
            var uniqueId = userClaims.FindFirst(GoogleOidcClaims.Sub)?.Value;
            var issuer = userClaims.FindFirst(GoogleOidcClaims.Iss)?.Value;

            if(!userClaims.IsAuthenticated || nameId is null || name is null 
                || email is null || uniqueId is null || issuer is null)
            {
                throw new ArgumentException(nameof(claimsIdentity));
            }

            var authInfo = new UserAuthInfo()
            {
                Name = name,
                ExternalIdentity = uniqueId,
                MailId = email,
                ProfileImageUrl = picture,
                Provider = issuer,
            };

            await contentRepository.CreateUserAsync(authInfo);

            claimsIdentity.AddClaim(new Claim(type: "seopt_user", value: authInfo.Id.ToString()));
            return authInfo.Id;
        }
    }
}
