using CFB.Infrastructure.Persistence.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace CFB
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args)
                .Build();

            await webHost.SeedIdentityDateAsync();
            await webHost.SeedCacheDateAsync();
            await webHost.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
