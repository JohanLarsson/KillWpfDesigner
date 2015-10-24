namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell;

    internal sealed class KillDesignerAndRebuildSolutionCommand : IDisposable
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;
        private readonly MenuCommand _menuItem;

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
                _menuItem = new MenuCommand(Execute, GuidsAndIds.KillDesignerAndRebuildSolutionCommandId)
                {
                    Visible = false
                };
                commandService.AddCommand(_menuItem);
                var service = GetService<DTE>();
                service.Events.SolutionEvents.AfterClosing += UpdateVisibility;
                service.Events.SolutionEvents.Opened += UpdateVisibility;
            }
        }

        private Command RebuildSolutionCommand
        {
            get
            {
                var dte = GetService<DTE>();
                var commands = dte.Commands;
                var command = commands.Item("Build.RebuildSolution");
                return command;
            }
        }

        public void Dispose()
        {
            var dte = GetService<DTE>();
            dte.Events.SolutionEvents.AfterClosing -= UpdateVisibility;
            dte.Events.SolutionEvents.Opened -= UpdateVisibility;
        }

        public T GetService<T>() where T : class
        {
            return _package.GetService<T>();
        }

        private void Execute(object sender, EventArgs e)
        {
            _menuItem.Enabled = false;
            try
            {
                WpfDesigner.KillAll();
                var command = RebuildSolutionCommand;
                object customIn = null;
                object customOut = null;
                var service = GetService<DTE>();
                var commands = service.Commands;
                commands.Raise(command.Guid, command.ID, ref customIn, ref customOut);
            }
            finally
            {
                _menuItem.Enabled = true;
            }
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
    }
}
