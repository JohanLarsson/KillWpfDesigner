using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Debugger = System.Diagnostics.Debugger;

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