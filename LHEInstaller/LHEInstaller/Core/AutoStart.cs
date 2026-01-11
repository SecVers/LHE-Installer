using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LHEInstaller.Core
{
    internal class AutoStart
    {
        private static readonly string AppName = "LHE";
        private static readonly string AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "LHE", "LHE.exe");

        public static bool IsAutoStartEnabled()
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                var autoStartValue = key?.GetValue(AppName)?.ToString();
                return !string.IsNullOrEmpty(autoStartValue) && autoStartValue.Equals(AppPath, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error check autostart: " + ex.Message);
                return false;
            }
        }


        public static void EnableAutoStart()
        {
            try
            {
                if (!IsAutoStartEnabled())
                {
                    var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    key.SetValue(AppName, AppPath);
                }
             
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error enabling auto-start: " + ex.Message);
            }

        }

    }
}
