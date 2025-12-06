using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace infex1rn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        private bool _consoleAllocated = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Allocate a console window for tool output
            _consoleAllocated = AllocConsole();
            if (_consoleAllocated)
            {
                Console.Title = "infex1rn - Tool Output Console";
                Console.WriteLine("===========================================");
                Console.WriteLine("       infex1rn Tool Console");
                Console.WriteLine("===========================================");
                Console.WriteLine("This console will display output from tools.");
                Console.WriteLine("Keep this window open while using the app.");
                Console.WriteLine();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_consoleAllocated)
            {
                FreeConsole();
            }
            base.OnExit(e);
        }
    }
}
