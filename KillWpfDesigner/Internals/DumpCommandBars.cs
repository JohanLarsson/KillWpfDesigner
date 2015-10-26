namespace KillWpfDesigner.Internals
{
    using System;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using EnvDTE;
    using Microsoft.VisualStudio.CommandBars;
    using Debugger = System.Diagnostics.Debugger;

    public static class DumpCommandBars
    {
        public static void Dump(IServiceProvider provider)
        {
            var dte = provider.GetService<DTE>();
            var commandBars = (CommandBars)dte.CommandBars;
            var sb = new StringBuilder();

            var writer = new IndentedTextWriter(new StringWriter(sb), "    ");
            foreach (var item in commandBars)
            {
                var bar = item as CommandBar;
                if (bar != null)
                {
                    writer.WriteLine($"CommandBar: {bar.Name}");
                    writer.Indent++;
                    Dump(bar.Controls, writer);
                    writer.Indent--;
                    continue;
                }
                else
                {
                    Dump((CommandBarControl) item, writer);
                }
            }
            var s = sb.ToString();
            var commandBar = commandBars["Rebuild Solution"];
        }

        private static void Dump(CommandBarControls controls, IndentedTextWriter writer)
        {
            foreach (var item in controls)
            {
                var bar = item as CommandBar;
                if (bar != null)
                {
                    writer.WriteLine($"CommandBar: {bar.Name}");
                    writer.Indent++;
                    Dump(bar.Controls, writer);
                    writer.Indent--;
                    continue;
                }
                var popup = item as CommandBarPopup;
                if (popup != null)
                {
                    writer.WriteLine($"Popup: {popup.Caption}");
                    writer.Indent++;
                    Dump(popup.Controls, writer);
                    writer.Indent--;
                    continue;
                }
                else
                {
                    Dump((CommandBarControl)item, writer);
                }
            }
        }

        private static void Dump(CommandBarControl control, IndentedTextWriter writer)
        {
            writer.WriteLine($"{control.Type}: {control.Caption}");
        }
    }
}
