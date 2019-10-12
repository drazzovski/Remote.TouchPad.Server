using SuperWebSocket;
using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace RemoteTouchPad
{
    public partial class Form1 : Form
    {

        private static WebSocketServer wsServer;
        private static bool highlightingStarted = false;

        [Flags]
        public enum MouseEventFlags : int
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_HWHEEL = 0x1000
        }        

        public Form1()
        {
            InitializeComponent();

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    var str = ip.ToString();
                    int index = str.LastIndexOf(".");
                    txtHost.Text = str.Substring(index + 1, str.Length - index - 1);
                }
            }

            notifyIcon.Visible = true;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int port = int.Parse(string.IsNullOrEmpty(portInput.Text) ? "0" : portInput.Text);
            if (port < 1 || port > 65535)
            {
                MessageBox.Show("Port must be between 1 and 65535");
                return;
            }

            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    MessageBox.Show("Port is in use, please choose another one.");
                    return;
                }
            }

            wsServer = new WebSocketServer();
            wsServer.Setup(port);
            wsServer.NewSessionConnected += WsServer_NewSessionConnected;
            wsServer.NewMessageReceived += WsServer_NewMessageReceived;
            wsServer.NewDataReceived += WsServer_NewDataReceived;
            wsServer.SessionClosed += WsServer_SessionClosed;
            wsServer.Start();
            label2.Text = "Server is running.";
            notifyIcon.BalloonTipText = "Server is started.";
            notifyIcon.ShowBalloonTip(1000);
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void WsServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            notifyIcon.BalloonTipText = "Device is disconnected.";
            notifyIcon.ShowBalloonTip(1000);
        }

        private void WsServer_NewSessionConnected(WebSocketSession session)
        {
            notifyIcon.BalloonTipText = "Device is connected.";
            notifyIcon.ShowBalloonTip(1000);
        }

        private void WsServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            var g = "NewDataReceived";
        }

        private void WsServer_NewMessageReceived(WebSocketSession session, string value)
        {

            var t = "NewMessageReceived " + value;
            var mousePosition = Win32.GetMousePosition();

            System.Diagnostics.Debug.WriteLine(value);

            if (value == "left")
            {
                Win32.DoMouseClick((int)MouseEventFlags.LeftDown, (int)MouseEventFlags.LeftUp, mousePosition.X, mousePosition.Y);
            }
            else if (value == "right")
            {
                Win32.DoMouseClick((int)MouseEventFlags.RightDown, (int)MouseEventFlags.RightUp, mousePosition.X, mousePosition.Y);
            }
            else if (value == "leftup")
            {
                System.Diagnostics.Debug.WriteLine(highlightingStarted);
                if (highlightingStarted)
                {
                    Win32.MouseEvent((int)MouseEventFlags.LeftUp, mousePosition.X, mousePosition.Y);
                    highlightingStarted = false;
                }
            }
            else if (value.Contains("scrollup"))
            {
                var arr = value.Split(',');
                Win32.MouseEvent((int)MouseEventFlags.MOUSEEVENTF_WHEEL, 0, 0, 100 * int.Parse(arr[1]), 0);
            }
            else if (value.Contains("scrolldown"))
            {
                var arr = value.Split(',');      
                Win32.MouseEvent((int)MouseEventFlags.MOUSEEVENTF_WHEEL, 0, 0, -100 * int.Parse(arr[1]), 0);
            }
            else if (value == "scrollleft")
            {
                Win32.MouseEvent((int)MouseEventFlags.MOUSEEVENTF_HWHEEL, 0, 0, 100, 0);
            }
            else if (value == "scrollright")
            {
                Win32.MouseEvent((int)MouseEventFlags.MOUSEEVENTF_HWHEEL, 0, 0, -100, 0);
            } 
            else if (value == "zoomin")
            {
                Win32.SendKeyDown((byte)KeyboardApi.KeyboardEventFlags.VK_LCONTROL/*(byte)KeyboardEventFlags.VK_LCONTROL*/);
                Win32.MouseEvent((int)MouseEventFlags.MOUSEEVENTF_WHEEL, 0, 0, 20, 0);
                Win32.SendKeyUp((byte)KeyboardApi.KeyboardEventFlags.VK_LCONTROL);
            }
            else if (value == "zoomout")
            {
                Win32.SendKeyDown((byte)KeyboardApi.KeyboardEventFlags.VK_LCONTROL);
                Win32.MouseEvent((int)MouseEventFlags.MOUSEEVENTF_WHEEL, 0, 0, -20, 0);
                Win32.SendKeyUp((byte)KeyboardApi.KeyboardEventFlags.VK_LCONTROL);
            }
            else if (value.Contains("mark"))
            {
                if (!highlightingStarted)
                {
                    Win32.MouseEvent((int)MouseEventFlags.LeftDown, mousePosition.X, mousePosition.Y);
                    highlightingStarted = true;
                } else
                {
                    var arr = value.Split(',');
                    MoveIt(short.Parse(arr[1]), short.Parse(arr[2]), mousePosition);
                }
                
            }
            else if (value.Contains("buttons"))
            {
                var arr = value.Split(',');
                //key down
                KeyboardApi.KeyboardPressKeys(arr.Skip(1).AsEnumerable());
            }
            else if (value.Contains("release"))
            {
                var arr = value.Split(',');
                // key up
                KeyboardApi.KeyboardReleaseKeys(arr.Skip(1));
            }
            else if (value.Contains("key"))
            {
                var arr = value.Split(',');
                KeyboardApi.KeyboardType(arr[1]);
            }
            else
            {
                var numbers = Array.ConvertAll(value.Split(','), int.Parse);

                MoveIt((short)numbers[0], (short)numbers[1], mousePosition);
            }
        }

        void MoveIt(short x, short y, Point pos)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate {

                    Win32.POINT p = new Win32.POINT();
                    Win32.ClientToScreen(this.Handle, ref p);
                    Win32.SetCursorPos(pos.X + x, pos.Y + y);
                }));
            }
        }

        private void portInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !(e.KeyChar == (char)8))
            {
                e.Handled = true;
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Hide();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            wsServer.Stop();
            label2.Text = "Server is stopped.";
            notifyIcon.BalloonTipText = "Server is stopped.";
            notifyIcon.ShowBalloonTip(1000);
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            wsServer.Dispose();
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon.Dispose();
        }
        
    }
    
}
