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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetLayeredWindowAttributes(nint hwnd, uint crKey, byte bAlpha, uint dwFlags);

        public const uint LWA_COLORKEY = 0x1;
        public const uint LWA_ALPHA = 0x2;
    }

    /// <summary>
    /// Class containing P/Invoke definitions for UpdateLayeredWindow (used in Per-Pixel mode).
    /// </summary>
    internal static class Win32Native
    {
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
        public static extern bool UpdateLayeredWindow(nint hwnd, nint hdcDst,
            ref POINT pptDst, ref SIZE psize, nint hdcSrc, ref POINT pptSrc,
            int crKey, ref BLENDFUNCTION pblend, uint dwFlags);
    }

    // Native Methods for Per-Pixel Alpha
    internal static class NativeMethods
    {
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
    }
}
