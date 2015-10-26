namespace KillWpfDesigner
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Text;

    using EnvDTE;

    using Microsoft.VisualStudio.CommandBars;

    public static class DumpCommandBars
    {
        public static string Dump(IServiceProvider provider)
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
                    WriteInfo(bar, writer);
                    writer.Indent++;
                    Dump(bar.Controls, writer);
                    writer.Indent--;
                    continue;
                }
                WriteInfo((CommandBarControl)item, writer);
            }
            var info = sb.ToString();
            return info;
        }

        private static void Dump(CommandBarControls controls, IndentedTextWriter writer)
        {
            foreach (var item in controls)
            {
                var bar = item as CommandBar;
                if (bar != null)
                {
                    WriteInfo(bar, writer);
                    writer.Indent++;
                    Dump(bar.Controls, writer);
                    writer.Indent--;
                    continue;
                }
                var popup = item as CommandBarPopup;
                if (popup != null)
                {
                    WriteInfo(popup, writer);
                    writer.Indent++;
                    Dump(popup.Controls, writer);
                    writer.Indent--;
                    continue;
                }
                WriteInfo((CommandBarControl)item, writer);
            }
        }

        private static void WriteInfo(CommandBarControl control, IndentedTextWriter writer)
        {
            var propertyInfo = control.GetType()
                                               .GetProperty("Controls");
            if (propertyInfo != null)
            {
                System.Diagnostics.Debugger.Break();
            }
            writer.WriteLine($"{control.Type}: {control.Caption} Id:{control.Id} Index: {control.Index} BuiltIn: {control.BuiltIn} Prio: {control.Priority}");
        }

        private static void WriteInfo(CommandBar bar, IndentedTextWriter writer)
        {
            writer.WriteLine($"{bar.Type}: {bar.Name} Id:{bar.Id} Index: {bar.Index} BuiltIn: {bar.BuiltIn}");
        }

        private static void WriteInfo(CommandBarPopup popup, IndentedTextWriter writer)
        {
            writer.WriteLine($"{popup.Type}: {popup.Caption} Id:{popup.Id} Index: {popup.Index} BuiltIn: {popup.BuiltIn} Prio: {popup.Priority}");
        }
    }
}
