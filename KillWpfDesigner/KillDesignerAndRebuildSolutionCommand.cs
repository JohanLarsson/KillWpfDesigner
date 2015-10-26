namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;
    using EnvDTE;

    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;

    internal sealed class KillDesignerAndRebuildSolutionCommand : IDisposable
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;
        private readonly MenuCommand _menuItem;
        private static readonly string GuidVsStandardCommandSet97 = VSConstants.GUID_VSStandardCommandSet97.ToString("B");

        /// <summary>
        /// Initializes a new instance of the <see cref="KillDesignerCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        internal KillDesignerAndRebuildSolutionCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            var commandService = GetService<IMenuCommandService>();
            if (commandService != null)
            {
                _menuItem = new MenuCommand(Execute, GuidsAndIds.KillDesignerAndRebuildSolutionCommandId);
                commandService.AddCommand(_menuItem);
                var dte = GetService<DTE>();
                var commandEvents = dte.Events.CommandEvents[GuidVsStandardCommandSet97, (int)VSConstants.VSStd97CmdID.RebuildSln];
                commandEvents.BeforeExecute += OnBeforeExecuteRebuildSln;
                commandEvents.AfterExecute += OnAfterExecuteRebuildSln;
                dte.Events.SolutionEvents.AfterClosing += UpdateVisibility;
                dte.Events.SolutionEvents.Opened += UpdateVisibility;
                UpdateVisibility();
            }
        }

        public void Dispose()
        {
            var dte = GetService<DTE>();
            dte.Events.SolutionEvents.AfterClosing -= UpdateVisibility;
            dte.Events.SolutionEvents.Opened -= UpdateVisibility;
            var commandEvents = dte.Events.CommandEvents[GuidVsStandardCommandSet97, (int)VSConstants.VSStd97CmdID.RebuildSln];
            commandEvents.BeforeExecute -= OnBeforeExecuteRebuildSln;
            commandEvents.AfterExecute -= OnAfterExecuteRebuildSln;
        }

        public T GetService<T>() where T : class
        {
            return _package.GetService<T>();
        }

        private void Execute(object sender, EventArgs e)
        {
            _menuItem.Enabled = false;
            WpfDesigner.KillAll();
            var dte = GetService<DTE>();
            dte.ExecuteCommand("Build.RebuildSolution");
        }

        private void UpdateVisibility()
        {
            var service = GetService<DTE>();
            var solution = service.Solution;
            if (solution == null || !solution.IsOpen)
            {
                _menuItem.Visible = false;
                return;
            }

            _menuItem.Visible = true; // Check if it is a WPF s
        }

        private void OnBeforeExecuteRebuildSln(string guid, int id, object customIn, object customOut, ref bool cancelDefault)
        {
            _menuItem.Enabled = false;
        }

        private void OnAfterExecuteRebuildSln(string guid, int id, object customIn, object customOut)
        {
            _menuItem.Enabled = true;
        }
    }
}
