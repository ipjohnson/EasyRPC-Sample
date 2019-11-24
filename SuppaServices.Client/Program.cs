using System;
using System.IO;
using System.Threading.Tasks;
using EasyRpc.DynamicClient;
using EasyRpc.DynamicClient.Grace;
using Grace.DependencyInjection;
using SuppaServices.Interfaces;

namespace SuppaServices.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<InterfaceNamingConvention>().As<INamingConventionService>());

            container.ProxyNamespace("http://localhost:55840", namespaces: typeof(Anchor).Namespace);

            var service = container.Locate<IBitmapService>();

            var bytes = await File.ReadAllBytesAsync("sample.png");
            var response = await service.Create(bytes, 90);
            await File.WriteAllBytesAsync("rotated.png", response);
        }
    }
}
