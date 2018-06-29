using Microsoft.AspNetCore.Http;

namespace Wallet.Extensions
{
    public static class ResponseExtensions
    {
        public static void AddError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
        }
    }
}