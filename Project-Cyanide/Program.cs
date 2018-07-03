using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Cyanide {
	class Program {
		static void Main( string[] args ) {
			Overlay overlay = new Overlay();
			Application.Run(overlay);
		}
	}

	class Overlay : Form {

		public delegate void WinEventDelegate( IntPtr hWinEventHook, uint eventType, IntPtr hwnd, uint idObject, long idChild, uint dwEventThread, uint dwmsEventTime );
		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowThreadProcessId( IntPtr hWnd, IntPtr processId );
		[DllImport("user32.dll", SetLastError = true)]
		static extern int GetWindowLong( IntPtr hWnd, int nIndex );
		[DllImport("user32.dll")]
		static extern int SetWindowLong( IntPtr hWnd, int nIndex, int dwNewLong );
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow( IntPtr hWnd );
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect( IntPtr hWnd, ref Rectangle lpRect );
		[DllImport("user32.dll")]
		private static extern IntPtr SetWinEventHook( uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags );

		protected WinEventDelegate _WinEventDelegate;

		public Overlay() {
			// _WinEventDelegate = new WinEventDelegate(WinEventCallback);

			// SetWinEventHook(0x800B, 0x800B, IntPtr.Zero, _WinEventDelegate, (uint) Id, GetWindowThreadProcessId(MainWindowHandle, IntPtr.Zero), 0x3);

			Rectangle rect = Screen.PrimaryScreen.Bounds;
			TransparencyKey = Color.Turquoise;
			BackColor = Color.Turquoise;
			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.Manual;
			DoubleBuffered = true;
			Location = new Point(rect.Left, rect.Top);
			Size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
			TopMost = true;

			SetForegroundWindow(Handle);

			Console.WriteLine("");
		}

		// protected void WinEventCallback( IntPtr hWinEventHook, uint eventType, IntPtr hWnd, uint idObject, long idChild, uint dwEventThread, uint dwmsEventTime ) {
		//	
		// }

		protected override void OnLoad( EventArgs e ) {
			base.OnLoad(e);
			var style = GetWindowLong(Handle, -20);
			SetWindowLong(Handle, -20, style | 0x80020);
		}

		protected override void OnPaint( PaintEventArgs e ) {
			base.OnPaint(e);

			Graphics display = e.Graphics;
			Image img = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
			Graphics g = Graphics.FromImage(img);

			DrawSquareFrame(g, Color.LightBlue, 3f, 0, 0, Size.Width, Size.Height);

			display.DrawImage(img, ClientRectangle);
			img.Dispose();
		}

		public void DrawSquareFrame(Graphics g, Color color, float width, int x1, int y1, int x2, int y2) {
			Pen pen = new Pen(color, width);
				g.DrawLine(pen, x1, y1, x2, y1);
				g.DrawLine(pen, x1, y1, x1, y2);
				g.DrawLine(pen, x1, y1, x2, y2);
				g.DrawLine(pen, x1, y2, x2, y1);
				g.DrawLine(pen, x2, y1, x2, y2);
				g.DrawLine(pen, x1, y2, x2, y2);
			pen.Dispose();
		}
	}
}
