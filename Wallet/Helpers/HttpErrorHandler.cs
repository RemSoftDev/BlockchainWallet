using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Wallet.Helpers
{
    public static class HttpErrorHandler
    {
        public static ModelStateDictionary AddErrors(IdentityResult identityResult, ModelStateDictionary model)
        {
            foreach (var e in identityResult.Errors)
            {
                model.TryAddModelError(e.Code, e.Description);
            }

            return model;
        }

        public static ModelStateDictionary AddError(string key, string message, ModelStateDictionary modelState)
        {
            modelState.TryAddModelError(key, message);
            return modelState;
        }
    }
}