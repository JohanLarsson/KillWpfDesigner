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
        private readonly Package _package;
        private readonly MenuCommand _menuItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="KillDesignerCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private KillDesignerCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            _package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(GuidsAndIds.CommandSet, GuidsAndIds.KillDesignerCommandId);
                _menuItem = new MenuCommand(Execute, menuCommandID)
                {
                    Visible = false
                };
                commandService.AddCommand(_menuItem);
                UpdateVisibility();
                var service = ServiceProvider.GetService<DTE>();
                service.Events.SolutionEvents.AfterClosing += UpdateVisibility;
                service.Events.SolutionEvents.Opened += UpdateVisibility;
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static KillDesignerCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new KillDesignerCommand(package);
        }

        public void Dispose()
        {
            var service = ServiceProvider.GetService<DTE>();
            service.Events.SolutionEvents.AfterClosing -= UpdateVisibility;
            service.Events.SolutionEvents.Opened -= UpdateVisibility;
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
            var service = ServiceProvider.GetService<DTE>();
            var solution = service.Solution;
            if (solution == null || solution.Projects.Count == 0)
            {
                _menuItem.Visible = false;
                return;
            }

            _menuItem.Visible = true; // Check if it is a WPF s
        }
    }
}
