using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wallet.Models;

namespace Wallet.Helpers
{
    public class Tokens
    {
        public static async Task<string> GenerateJwt(ClaimsIdentity identity, IJwtFactory jwtFactory, string userName, JWTSettings jwtOptions, JsonSerializerSettings serializerSettings)
        {
            string accessToken = string.Empty;
            if (identity.HasClaim(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.AdminRol))
            {
                accessToken = await jwtFactory.GenerateEncodedToken(userName, identity, true);
            }
            else
            {
                accessToken = await jwtFactory.GenerateEncodedToken(userName, identity, false);
            }

            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                access_token = accessToken,
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}