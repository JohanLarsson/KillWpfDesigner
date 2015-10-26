namespace KillWpfDesigner
{
    using System;
    using System.ComponentModel.Design;
    using System.Text;

    using EnvDTE;

    public static class DumpCommands
    {
        public static string Dump(IServiceProvider provider)
        {
            var dte = provider.GetService<DTE>();
            var stringBuilder = new StringBuilder();
            foreach (var item in dte.Commands)
            {
                var menuCommand = item as MenuCommand;
                if (menuCommand != null)
                {
                    stringBuilder.AppendLine($"{menuCommand.CommandID}");
                    continue;
                }
                var command = item as Command;
                if (command != null)
                {
                    stringBuilder.AppendLine($"{command.Name} GUID:{command.Guid} ID:{command.ID}");
                    continue;
                }
            }
            var infos = stringBuilder.ToString();
            return infos;
        }
    }
}