using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;

namespace CFB.Infrastructure.Identity.Models
{
    public class CosmosIdentityResult : IdentityResult
    {
        public static IdentityResult Failed(ResponseMessage responseMessage)
        {
            var identityError = new IdentityError
            {
                Code = responseMessage.StatusCode.ToString(),
                Description = responseMessage.ErrorMessage
            };

            return Failed(identityError);
        }
    }
}
