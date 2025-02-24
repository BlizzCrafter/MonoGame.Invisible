using System.Runtime.InteropServices;

namespace MonoGame.Invisible
{
    /// <summary>
    /// Helper class for basic Win32 calls needed for both modes.
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

        private const uint GW_HWNDPREV = 3; // Fenster direkt davor (höher im Z-Order)
        private const uint GW_HWNDNEXT = 2; // Fenster direkt danach (tiefer im Z-Order)

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

        #region P/Invoke definitions for UpdateLayeredWindow (used in Per-Pixel mode).

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;
        public const uint ULW_ALPHA = 0x02;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern nint GetDC(nint hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(nint hWnd, nint hDC);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern nint CreateCompatibleDC(nint hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteDC(nint hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern nint SelectObject(nint hdc, nint hgdiobj);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(nint hObject);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UpdateLayeredWindow(nint hwnd, nint hdcDst,
            ref POINT pptDst, ref SIZE psize, nint hdcSrc, ref POINT pptSrc,
            int crKey, ref BLENDFUNCTION pblend, uint dwFlags);

        #endregion

        public static void BringToFront(nint hWnd)
        {
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        public static void SendToBack(nint hWnd)
        {
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        public static void KeepInBackground(nint hWnd)
        {
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        public static bool IsForegroundWindow(nint hWnd)
        {
            return GetForegroundWindow() == hWnd;
        }

        public static bool IsInBackground(nint hWnd)
        {
            return GetWindow(hWnd, GW_HWNDPREV) != nint.Zero;
        }

        public static bool IsTopWindow(nint hWnd)
        {
            return GetTopWindow(nint.Zero) == hWnd;
        }
    }
}
