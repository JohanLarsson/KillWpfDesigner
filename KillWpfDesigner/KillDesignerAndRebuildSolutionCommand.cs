namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;
    using EnvDTE;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;

    internal sealed class KillDesignerAndRebuildSolutionCommand : IDisposable
    {
        private static readonly string GuidVsStandardCommandSet97 = VSConstants.GUID_VSStandardCommandSet97.ToString("B");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;
        private readonly MenuCommand menuItem;
        private readonly CommandEvents rebuildSlnEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="KillDesignerCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        internal KillDesignerAndRebuildSolutionCommand(Package package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            var commandService = this.GetService<IMenuCommandService>();
            if (commandService != null)
            {
                this.menuItem = new MenuCommand(this.Execute, GuidsAndIds.KillDesignerAndRebuildSolutionCommandId);
                commandService.AddCommand(this.menuItem);

                var dte = this.GetService<DTE>();
                this.rebuildSlnEvents = dte.Events.CommandEvents[GuidVsStandardCommandSet97, (int)VSConstants.VSStd97CmdID.RebuildSln];
                this.rebuildSlnEvents.BeforeExecute += this.OnBeforeExecuteRebuildSln;
                this.rebuildSlnEvents.AfterExecute += this.OnAfterExecuteRebuildSln;
            }
        }

        public void Dispose()
        {
            this.rebuildSlnEvents.BeforeExecute -= this.OnBeforeExecuteRebuildSln;
            this.rebuildSlnEvents.AfterExecute -= this.OnAfterExecuteRebuildSln;
        }

        public T GetService<T>()
            where T : class
        {
            return this.package.GetService<T>();
        }

        private void Execute(object sender, EventArgs e)
        {
            this.menuItem.Enabled = false;
            WpfDesigner.KillAll();
            var dte = this.GetService<DTE>();
            dte.ExecuteCommand("Build.RebuildSolution");
        }

        private void OnBeforeExecuteRebuildSln(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            this.menuItem.Enabled = false;
        }

        private void OnAfterExecuteRebuildSln(string guid, int id, object customIn, object customOut)
        {
            this.menuItem.Enabled = true;
        }
    }
}
