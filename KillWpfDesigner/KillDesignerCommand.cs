namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;
    using Microsoft.VisualStudio.Shell;

    internal sealed class KillDesignerCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KillDesignerCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        internal KillDesignerCommand(Package package)
        {
            package.GetService<IMenuCommandService>()?.AddCommand(new MenuCommand(this.Execute, GuidsAndIds.KillDesignerCommandId));
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
    }
}
