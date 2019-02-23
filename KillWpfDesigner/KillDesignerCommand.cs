namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;

    using EnvDTE;

    using Microsoft.VisualStudio.Shell;

    internal sealed class KillDesignerCommand : IDisposable
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;
        private readonly MenuCommand menuItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="KillDesignerCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        internal KillDesignerCommand(Package package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            var commandService = this.GetService<IMenuCommandService>();
            if (commandService != null)
            {
                this.menuItem = new MenuCommand(this.Execute, GuidsAndIds.KillDesignerCommandId);
                this.UpdateVisibility();
                commandService.AddCommand(this.menuItem);

                var service = this.GetService<DTE>();
                service.Events.SolutionEvents.AfterClosing += this.UpdateVisibility;
                service.Events.SolutionEvents.Opened += this.UpdateVisibility;
            }
        }

        public T GetService<T>()
            where T : class
        {
            return this.package.GetService<T>();
        }

        public void Dispose()
        {
            var dte = this.GetService<DTE>();
            dte.Events.SolutionEvents.AfterClosing -= this.UpdateVisibility;
            dte.Events.SolutionEvents.Opened -= this.UpdateVisibility;
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
            WpfDesigner.KillAll();
        }

        private void UpdateVisibility()
        {
            this.menuItem.Visible = this.package.HasOpenWpfSolution();
        }
    }
}
