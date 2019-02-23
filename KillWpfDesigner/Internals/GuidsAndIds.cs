namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;

    internal static class GuidsAndIds
    {
        /// <summary>
        /// KillDesignerCommandPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "9d0d660c-aad6-44c2-822b-33f00e79d0eb";

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("e72de556-6b14-45d5-a92f-86185351617b");

        public static readonly CommandID KillDesignerCommandId = new CommandID(CommandSet, 1);
        public static readonly CommandID KillDesignerAndRebuildSolutionCommandId = new CommandID(CommandSet, 2);
        public static readonly CommandID CleanShadowCacheCommandId = new CommandID(CommandSet, 3);
    }
}
