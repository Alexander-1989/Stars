using System.Runtime.InteropServices;

namespace Stars
{
    static class NativeMethods
    {
        static private int count = 0;

        [DllImport("user32.dll", EntryPoint = "ShowCursor")]
        static extern int _showCursor(bool show);

        public static void ShowCursor(bool show)
        {
            if ((!show && count >= 0) || (show && count < 0))
            {
                count = _showCursor(show);
            }
        }
    }
}