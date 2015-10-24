namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;

    internal static class GuidsAndIds
    {
        private const int _KillDesignerCommandId = 1;

        private const int _KillDesignerAndRebuildSolutionCommandId = 2;

        /// <summary>
        /// KillDesignerCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "9d0d660c-aad6-44c2-822b-33f00e79d0eb";

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        private static readonly Guid CommandSet = new Guid("e72de556-6b14-45d5-a92f-86185351617b");

        public static readonly CommandID KillDesignerCommandId = new CommandID(CommandSet, _KillDesignerCommandId);
        public static readonly CommandID KillDesignerAndRebuildSolutionCommandId = new CommandID(CommandSet, _KillDesignerAndRebuildSolutionCommandId);
    }
}
