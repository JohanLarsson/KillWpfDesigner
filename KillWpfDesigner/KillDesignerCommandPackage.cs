namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(Microsoft.VisualStudio.Shell.Interop.UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidsAndIds.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class KillDesignerCommandPackage : AsyncPackage
    {
        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            var mcs = (IMenuCommandService)await this.GetServiceAsync(typeof(IMenuCommandService));
            var dte = (DTE)await this.GetServiceAsync(typeof(DTE));
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            mcs.AddCommand(new MenuCommand((_, __) => KillAllDesigners(), GuidsAndIds.KillDesignerCommandId));
            mcs.AddCommand(new MenuCommand((_, __) => KillAndRebuild(dte), GuidsAndIds.KillDesignerAndRebuildSolutionCommandId));
            mcs.AddCommand(new MenuCommand((_, __) => CleanBinAndObj(dte), GuidsAndIds.CleanBinAndObjCommandId));
            var shadowCacheDirectory = Path.Combine(this.UserLocalDataPath, "Designer", "ShadowCache");
            mcs.AddCommand(new MenuCommand((_, __) => CleanShadowCache(shadowCacheDirectory), GuidsAndIds.CleanShadowCacheCommandId));
        }

        private static void KillAllDesigners()
        {
            var designers = System.Diagnostics.Process.GetProcessesByName("XDesProc");
            foreach (var designer in designers)
            {
                designer.Kill();
            }

            designers = System.Diagnostics.Process.GetProcessesByName("WpfSurface");
            foreach (var designer in designers)
            {
                designer.Kill();
            }
        }

        private static void CleanBinAndObj(DTE dte)
        {
            if (dte.ActiveSolutionProjects is Array projects)
            {
                foreach (var o in projects)
                {
                    if (o is Project project &&
                        Path.GetDirectoryName(project.FullName) is string projectDir)
                    {
                        DeleteRecursively(Path.Combine(projectDir, "bin"));
                        DeleteRecursively(Path.Combine(projectDir, "obj"));
                    }
                }
            }
        }

        private static void KillAndRebuild(DTE dte)
        {
            KillAllDesigners();
            dte.ExecuteCommand("Build.RebuildSolution");
        }

        private static void CleanShadowCache(string shadowCacheDirectory)
        {
            if (Directory.Exists(shadowCacheDirectory))
            {
                foreach (var dir in Directory.EnumerateDirectories(shadowCacheDirectory))
                {
                    DeleteRecursively(dir);
                }
            }
        }

        private static void DeleteRecursively(string dir)
        {
            if (Directory.Exists(dir))
            {
                foreach (var file in Directory.EnumerateFiles(dir))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // swallowing here, could be that the file was in use or something else that I don't know how to check.
                    }
                }

                foreach (var subDir in Directory.EnumerateDirectories(dir))
                {
                    DeleteRecursively(subDir);
                }

                try
                {
                    Directory.Delete(dir, recursive: true);
                }
                catch
                {
                    // swallowing here, could be that a file was in use or something else that I don't know how to check.
                }
            }
        }
    }
}
