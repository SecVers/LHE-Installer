using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;
namespace LHEInstaller.Core
{
    internal class SecVersLHEChecker
    {
        public static bool IsSecVersLHERunning()
        {
            var processes = Process.GetProcessesByName("SecVersLHE");

            return processes.Any(); 
        }
    }
}
