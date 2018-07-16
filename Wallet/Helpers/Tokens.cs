using System;
using System.Collections.Generic;
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
            var roles = new List<string>();
            if (identity.HasClaim(c => c.Type == Helpers.Constants.Strings.JwtClaimIdentifiers.AdminRol))
            {
                accessToken = await jwtFactory.GenerateEncodedToken(userName, identity, true);
                roles.Add("ApiAdmin");
            }
            else
            {
                accessToken = await jwtFactory.GenerateEncodedToken(userName, identity, false);
                roles.Add("ApiUser");
            }

            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                access_token = accessToken,
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds,
                roles = roles,
                userName = userName

            };

            return JsonConvert.SerializeObject(response, serializerSettings);
        }
    }
}