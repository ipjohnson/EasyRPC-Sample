using System;
using System.Reflection;
using EasyRpc.DynamicClient;

namespace SuppaServices.Uwp
{
    public class InterfaceNamingConvention : INamingConventionService
    {
        // strip the I off the beginning of interface names so they match what the server is exporting
        public string GetNameForType(Type type) => type.Name.Substring(1);

        public string GetMethodName(MethodInfo method) => method.Name;
    }
}