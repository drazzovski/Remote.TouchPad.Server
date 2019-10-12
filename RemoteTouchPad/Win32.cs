using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RemoteTouchPad
{
    public class Win32
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref POINT pt);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(uint idThread);

        public static Point GetMousePosition()
        {
            POINT w32Mouse = new POINT();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.x, w32Mouse.y);
        }

        public static void DoMouseClick(int down, int up, int X, int Y)
        {
            mouse_event(down | up, X, Y, 0, 0);
        }

        public static void MouseEvent(int eventFlag, int X, int Y, int dwData = 0, uint dwExtraInfo = 0)
        {
            mouse_event(eventFlag, X, Y, dwData, dwExtraInfo);
        }

        /// <summary>
        /// Key pressed
        /// </summary>
        /// <param name="key"></param>
        public static void SendKeyDown(byte key)
        {
            keybd_event(key, 0, 0, 0);
        }

        /// <summary>
        /// Key released
        /// </summary>
        /// <param name="key"></param>
        public static void SendKeyUp(byte key)
        {
            keybd_event(key, 0, 0x0002, 0);
        }

        /// <summary>
        /// Key type. (Press and Release)
        /// </summary>
        /// <param name="key"></param>
        public static void KeyType(byte key)
        {
            keybd_event(key, 0, 0 | 0x0002, 0);
        }

    }
}
