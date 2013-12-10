using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace SortVis
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new FrmMain());
            }
            catch (CompositionException)
            {
                System.Diagnostics.Trace.TraceWarning("MEF couldn't find any sorters.");
            }
        }
    }
}
