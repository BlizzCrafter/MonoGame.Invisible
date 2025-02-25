using Microsoft.Win32;
using System.Reflection;

namespace MonoGame.Invisible
{
    public static class StartupManager
    {
        // The auto-start registry key path.
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Enables or disables autostart for the application.
        /// </summary>
        public static void SetAutostart(bool enable)
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true);
            if (key == null) throw new Exception("Could not open registry key.");

            var exePath = GetExecutablePath();
            if (string.IsNullOrEmpty(exePath)) throw new Exception("Executable path is empty or invalid.");

            if (enable)
            {
                key.SetValue(TransparentWindowManager.AppName, $"\"{exePath}\"");
            }
            else
            {
                key.DeleteValue(TransparentWindowManager.AppName, false);
            }
        }

        /// <summary>
        /// Checks if autostart is enabled.
        /// </summary>
        public static bool IsAutostartEnabled()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
            return key?.GetValue(TransparentWindowManager.AppName) != null;
        }

        private static string GetExecutablePath()
        {
            string dllPath = Assembly.GetEntryAssembly()?.Location ?? string.Empty;
            string exePath = dllPath.Replace(".dll", ".exe");

            return File.Exists(exePath) ? exePath : string.Empty;
        }
    }
}
