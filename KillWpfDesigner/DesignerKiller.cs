namespace KillWpfDesigner
{
    using System.Diagnostics;

    public static class DesignerKiller
    {
        public static void KillAll()
        {
            var designers = Process.GetProcessesByName("XDesProc");
            foreach (var designer in designers)
            {
                designer.Kill();
            }
        }
    }
}