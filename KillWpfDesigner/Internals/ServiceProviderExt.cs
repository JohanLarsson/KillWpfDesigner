namespace KillWpfDesigner
{
    using System;

    internal static class ServiceProviderExt
    {
        internal static T GetService<T>(this IServiceProvider provider) where T : class
        {
            return provider.GetService(typeof(T)) as T;
        }
    }
}