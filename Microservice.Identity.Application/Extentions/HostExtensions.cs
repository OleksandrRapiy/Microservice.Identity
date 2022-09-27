using Microservice.Identity.Application.Interfaces.Seeders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Microservice.Identity.Application.Extentions
{
    public static class HostExtensions
    {
        public static async Task RunWithSeeder(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                foreach (var service in scope.ServiceProvider.GetServices<ISeeder>())
                    await service.ExecuteAsync();
            }

            await webHost.RunAsync();
        }
    }
}
