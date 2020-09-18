using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using CFB.Infrastructure.Identity.Stores;

namespace CFB.Infrastructure.Identity.Extensions
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder UseCosmosStore(this IdentityBuilder identityBuilder)
        {
            return identityBuilder
                .AddCosmosUserStore()
                .AddCosmosRoleStore();
        }

        private static IdentityBuilder AddCosmosUserStore(this IdentityBuilder builder)
        {
            builder.Services.AddScoped(typeof(IUserStore<>).MakeGenericType(builder.UserType),
                                       typeof(CosmosUserStore<,>).MakeGenericType(builder.UserType, builder.RoleType));

            return builder;
        }

        private static IdentityBuilder AddCosmosRoleStore(this IdentityBuilder builder)
        {
            builder.Services.AddScoped(typeof(IRoleStore<>).MakeGenericType(builder.RoleType),
                                       typeof(CosmosRoleStore<>).MakeGenericType(builder.RoleType));

            return builder;
        }
    }
}
