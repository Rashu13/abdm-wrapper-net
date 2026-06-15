using System;
using System.Threading;
using System.Windows.Forms;
using HMS.abdm;

namespace AbdmWinFormsTest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(FormResolve_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmABHAMain());
        }

        static void FormResolve_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Thread Exception:\n" + e.Exception.ToString(), "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled Exception:\n" + e.ExceptionObject.ToString(), "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
