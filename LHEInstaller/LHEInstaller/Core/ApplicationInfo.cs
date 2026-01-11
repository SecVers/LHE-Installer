using System;
using System.Diagnostics;
using System.IO;

namespace LHEInstaller.Core
{
    internal class ApplicationInfo
    {
        internal static string GetLHEVersion()
        {
            string lhePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "LHE", "LHE.exe");

            if (File.Exists(lhePath))
            {
                try
                {
                    FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(lhePath);
                    return versionInfo.FileVersion;
                }
                catch (Exception)
                {
                    return "0.0.0.0";
                }
            }
            else
            {
                return "0.0.0.0";
            }
        }
    }
}