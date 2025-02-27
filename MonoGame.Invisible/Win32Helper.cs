using System.Runtime.InteropServices;

namespace MonoGame.Invisible
{
    /// <summary>
    /// Helper class for basic Win32 calls.
    /// </summary>
    internal static class Win32Helper
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x00080000;
        public const int WS_EX_TOOLWINDOW = 0x80;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const int WS_EX_APPWINDOW = 0x40000;

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int HWND_TOPMOST = -1;
        private const int HWND_NOTOPMOST = -2;
        private const int HWND_BOTTOM = 1;

        private const uint GW_HWNDPREV = 3;
        private const uint GW_HWNDNEXT = 2;

        public const uint LWA_COLORKEY = 0x1;
        public const uint LWA_ALPHA = 0x2;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(nint hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(nint hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        private static extern nint GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern nint GetWindow(nint hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern nint GetTopWindow(nint hWnd);
        
        /// <summary>
        /// Brings the window to the front of the Z-order, making it the topmost window.
        /// </summary>
        public static void BringToFront(nint hWnd)
        {
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        /// <summary>
        /// Sends the window to the back of the Z-order, behind other windows.
        /// </summary>
        public static void SendToBack(nint hWnd)
        {
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        /// <summary>
        /// Keeps the window in the background, preventing it from being moved to the front.
        /// </summary>
        public static void KeepInBackground(nint hWnd)
        {
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        /// <summary>
        /// Keeps the window in the foreground, preventing it from being moved to the back.
        /// </summary>
        public static void KeepInForeground(nint hWnd)
        {
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        /// <summary>
        /// Checks if the window is the currently active (focused) window.
        /// </summary>
        /// <returns>True if the window is the foreground window; otherwise, false.</returns>
        public static bool IsForegroundWindow(nint hWnd)
        {
            return GetForegroundWindow() == hWnd;
        }

        /// <summary>
        /// Checks if the window is in the background (behind other windows).
        /// </summary>
        /// <returns>True if the window is behind another window; otherwise, false.</returns>
        public static bool IsInBackground(nint hWnd)
        {
            return GetWindow(hWnd, GW_HWNDPREV) != nint.Zero;
        }

        /// <summary>
        /// Checks if the window is the topmost window on the screen.
        /// </summary>
        /// <returns>True if the window is the topmost; otherwise, false.</returns>
        public static bool IsTopWindow(nint hWnd)
        {
            return GetTopWindow(nint.Zero) == hWnd;
        }
    }
}
