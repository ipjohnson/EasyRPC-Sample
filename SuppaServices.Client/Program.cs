using System;
using System.IO;
using EasyRpc.DynamicClient;
using EasyRpc.DynamicClient.Grace;
using Grace.DependencyInjection;
using SuppaServices.Interfaces;
using SuppaServices.Services;

namespace SuppaServices.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new DependencyInjectionContainer();

            // this is here because the services are exported by name but we're using the interface 
            // so we need to remove the I from the front of the interface name
            container.Configure(c => c.Export<InterfaceNamingConvention>().As<INamingConventionService>());

            container.ProxyNamespace("http://localhost:55840", namespaces: typeof(Anchor).Namespace);

            var mathService = container.Locate<IBitmapService>();

            var bytes = File.ReadAllBytes("sample.png");

            var response = mathService.Create(bytes, 90);

        }
    }
}
