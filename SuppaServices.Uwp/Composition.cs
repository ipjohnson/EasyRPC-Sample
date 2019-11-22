using EasyRpc.DynamicClient;
using EasyRpc.DynamicClient.Grace;
using Grace.DependencyInjection;
using SuppaServices.Interfaces;
using Zafiro.Core;
using Zafiro.Uwp.Controls;

namespace SuppaServices.Uwp
{
    public class Composition
    {
        public Composition()
        {
            var container = new DependencyInjectionContainer();

            container.Configure(c => c.Export<InterfaceNamingConvention>().As<INamingConventionService>());
            container.Configure(c => c.Export<UwpFilePicker>().As<IFilePicker>());
            container.ProxyNamespace("http://localhost:55840", namespaces: typeof(Anchor).Namespace);
            Root = container.Locate<MainViewModel>();
        }

        public MainViewModel Root { get; set; }
    }
}