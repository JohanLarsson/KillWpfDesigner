namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;
    using System.IO;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;

    internal sealed class CleanBinAndObjCommand
    {
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanBinAndObjCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        internal CleanBinAndObjCommand(Package package)
        {
            this.package = package;
            package.GetService<IMenuCommandService>()?.AddCommand(new MenuCommand(this.Execute, GuidsAndIds.CleanBinAndObjCommandId));
        }

        private static void Delete(string dir)
        {
            if (Directory.Exists(dir))
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(dir))
                    {
                        File.Delete(file);
                    }

                    foreach (var subDir in Directory.EnumerateDirectories(dir))
                    {
                        Delete(subDir);
                    }

                    Directory.Delete(dir, recursive: true);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            if (this.package.GetService<DTE>().ActiveSolutionProjects is Array projects)
            {
                foreach (var o in projects)
                {
                    if (o is Project project)
                    {
                        var projectDir = Path.GetDirectoryName(project.FullName);
                        Delete(Path.Combine(projectDir, "bin"));
                        Delete(Path.Combine(projectDir, "obj"));
                    }
                }
            }
        }
    }
}
