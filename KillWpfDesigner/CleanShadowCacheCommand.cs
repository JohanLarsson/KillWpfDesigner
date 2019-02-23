namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;
    using System.IO;
    using Microsoft.VisualStudio.Shell;

    internal sealed class CleanShadowCacheCommand
    {
        private readonly string shadowCacheDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanShadowCacheCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        internal CleanShadowCacheCommand(Package package)
        {
            this.shadowCacheDirectory = Path.Combine(package.UserLocalDataPath, "Designer", "ShadowCache");
            package.GetService<IMenuCommandService>()?.AddCommand(new MenuCommand(this.Execute, GuidsAndIds.CleanShadowCacheCommandId));
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
            if (Directory.Exists(this.shadowCacheDirectory))
            {
                foreach (var dir in Directory.EnumerateDirectories(this.shadowCacheDirectory))
                {
                    try
                    {
                        Directory.Delete(dir, recursive: true);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
