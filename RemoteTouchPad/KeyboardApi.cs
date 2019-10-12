using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteTouchPad
{
    public class KeyboardApi
    {

        [Flags]
        public enum KeyboardEventFlags
        {
            VK_LCONTROL = 0xA2,
            VK_SHIFT = 0x10,
            VK_TAB = 0x09,
            VK_PAUSE = 0x13,
            VK_SCROLL = 0x91,
            VK_SNAPSHOT = 0x2C,
            VK_PRINT = 0x2A,
            VK_CONTROL = 0x11, // ctrl
            VK_MENU = 0x12, // alt
            VK_LEFT = 0x25,
            VK_UP = 0x26,
            VK_RIGHT = 0x27,
            VK_DOWN = 0x28,
            VK_DELETE = 0x2E,
            VK_INSERT = 0x2D,
            VK_HOME = 0x24,
            VK_END = 0x23,
            VK_PRIOR = 0x21, // pg up
            VK_NEXT = 0x22, // pg dn
            VK_ESCAPE = 0x1B,
            VK_F1 = 0x70,
            VK_F2 = 0x71,
            VK_F3 = 0x72,
            VK_F4 = 0x73,
            VK_F5 = 0x74,
            VK_F6 = 0x75,
            VK_F7 = 0x76,
            VK_F8 = 0x77,
            VK_F9 = 0x78,
            VK_F10 = 0x79,
            VK_F11 = 0x7A,
            VK_F12 = 0x7B,
            VK_BACK = 0x08,
            VK_RETURN = 0x0D,
            VK_RMENU = 0xA5,         
        }

        public static void KeyboardType(string value)
        {
            if (value == "backspace")
            {
                Win32.SendKeyDown((byte)KeyboardEventFlags.VK_BACK);
                Win32.SendKeyUp((byte)KeyboardEventFlags.VK_BACK);
            }
            else if (value == "enter")
            {
                Win32.SendKeyDown((byte)KeyboardEventFlags.VK_RETURN);
                Win32.SendKeyUp((byte)KeyboardEventFlags.VK_RETURN);
            }
            else
            {
                char key = value[0];
                var keyscan = Win32.VkKeyScanEx(key, Win32.GetKeyboardLayout(0));

                byte upper = (byte)(keyscan >> 8);
                byte lower = (byte)(keyscan & 0xff);

                if (upper == 1)
                {
                    Win32.SendKeyDown((byte)KeyboardEventFlags.VK_SHIFT);
                }
                else if (upper == 2)
                {
                    Win32.SendKeyDown((byte)KeyboardEventFlags.VK_LCONTROL);
                }
                else if (upper == 4)
                {
                    Win32.SendKeyDown((byte)KeyboardEventFlags.VK_MENU);
                }
                else if (upper == 6)
                {
                    Win32.SendKeyDown((byte)KeyboardEventFlags.VK_RMENU);
                }

                Win32.SendKeyDown(lower);
                Win32.SendKeyUp(lower);

                if (upper == 1)
                {
                    Win32.SendKeyUp((byte)KeyboardEventFlags.VK_SHIFT);
                }
                else if (upper == 2)
                {
                    Win32.SendKeyUp((byte)KeyboardEventFlags.VK_LCONTROL);
                }
                else if (upper == 4)
                {
                    Win32.SendKeyUp((byte)KeyboardEventFlags.VK_MENU);
                }
                else if (upper == 6)
                {
                    Win32.SendKeyUp((byte)KeyboardEventFlags.VK_RMENU);
                }
            }
        }

        public static void KeyboardPressKeys(IEnumerable<string> list)
        {
            foreach (var key in list)
            {
                // key down
                if (key == "arrowDown") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_DOWN);
                else if (key == "arrowUp") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_UP);
                else if (key == "arrowRight") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_RIGHT);
                else if (key == "arrowLeft") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_LEFT);
                else if (key == "f12") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F12);
                else if (key == "f11") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F11);
                else if (key == "f10") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F10);
                else if (key == "f9") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F9);
                else if (key == "f8") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F8);
                else if (key == "f7") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F7);
                else if (key == "f6") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F6);
                else if (key == "f5") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F5);
                else if (key == "f4") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F4);
                else if (key == "f3") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F3);
                else if (key == "f2") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F2);
                else if (key == "f1") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_F1);
                else if (key == "tab") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_TAB);
                else if (key == "shift") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_SHIFT);
                else if (key == "ctrl") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_LCONTROL);
                else if (key == "alt") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_MENU);
                else if (key == "del") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_DELETE);
                else if (key == "insert") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_INSERT);
                else if (key == "home") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_HOME);
                else if (key == "end") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_END);
                else if (key == "pgup") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_PRIOR);
                else if (key == "pgdn") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_NEXT);
                else if (key == "scroll") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_SCROLL);
                else if (key == "pause") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_PAUSE);
                else if (key == "print") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_PRINT);
                else if (key == "esc") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_ESCAPE);
                else if (key == "altgr") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_RMENU);
                else if (key == "enter") Win32.SendKeyDown((byte)KeyboardEventFlags.VK_RETURN);
            }
        }

        public static void KeyboardReleaseKeys(IEnumerable<string> list)
        {
            foreach (var key in list)
            {
                // key up
                if (key == "arrowDown") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_DOWN);
                else if (key == "arrowUp") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_UP);
                else if (key == "arrowRight") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_RIGHT);
                else if (key == "arrowLeft") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_LEFT);
                else if (key == "f12") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F12);
                else if (key == "f11") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F11);
                else if (key == "f10") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F10);
                else if (key == "f9") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F9);
                else if (key == "f8") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F8);
                else if (key == "f7") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F7);
                else if (key == "f6") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F6);
                else if (key == "f5") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F5);
                else if (key == "f4") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F4);
                else if (key == "f3") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F3);
                else if (key == "f2") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F2);
                else if (key == "f1") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_F1);
                else if (key == "tab") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_TAB);
                else if (key == "shift") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_SHIFT);
                else if (key == "ctrl") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_LCONTROL);
                else if (key == "alt") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_MENU);
                else if (key == "del") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_DELETE);
                else if (key == "insert") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_INSERT);
                else if (key == "home") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_HOME);
                else if (key == "end") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_END);
                else if (key == "pgup") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_PRIOR);
                else if (key == "pgdn") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_NEXT);
                else if (key == "scroll") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_SCROLL);
                else if (key == "pause") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_PAUSE);
                else if (key == "print") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_PRINT);
                else if (key == "esc") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_ESCAPE);
                else if (key == "altgr") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_RMENU);
                else if (key == "enter") Win32.SendKeyUp((byte)KeyboardEventFlags.VK_RETURN);

            }
        }

    }
}
