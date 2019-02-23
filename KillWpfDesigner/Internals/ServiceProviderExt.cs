namespace KillWpfDesigner
{
    using System;

    using EnvDTE;

    internal static class ServiceProviderExt
    {
        internal static T GetService<T>(this IServiceProvider provider)
            where T : class
        {
            return provider.GetService(typeof(T)) as T;
        }

        internal static bool HasOpenWpfSolution(this IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService<DTE>();
            var solution = service.Solution;
            if (solution == null || !solution.IsOpen)
            {
                return false;
            }

            // TODO check if the solution is a WPF solution?
            return true;
        }
    }
}