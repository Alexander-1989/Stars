﻿using System.Runtime.InteropServices;

namespace Stars
{
    internal static class NativeMethods
    {
        private static bool _show = true;

        [DllImport("user32.dll", EntryPoint = "ShowCursor")]
        static extern int _showCursor(bool show);

        public static void ShowCursor(bool show)
        {
            if (show != _show)
            {
                _show = _showCursor(show) >= 0;
            }
        }
    }
}