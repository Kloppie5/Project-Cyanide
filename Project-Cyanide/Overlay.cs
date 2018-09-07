using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Project_Cyanide {
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT {
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;

		public RECT( int Left, int Top, int Right, int Bottom ) {
			this.Left = Left;
			this.Top = Top;
			this.Right = Right;
			this.Bottom = Bottom;
		}

		public static implicit operator RECT( Rectangle rect ) {
			return new RECT(rect.Left, rect.Top, rect.Left + rect.Width, rect.Top + rect.Height);
		}

		public static implicit operator Rectangle( RECT rect ) {
			return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
		}

		public Size Size {
			get {
				return new Size(Right - Left, Bottom - Top);
			}
			set {
				Left = Right + value.Width;
				Bottom = Top + value.Height;
			}
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
		static extern bool GetWindowRect( IntPtr hWnd, ref RECT lpRect );
		[DllImport("user32.dll")]
		private static extern IntPtr SetWinEventHook( uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags );

		protected WinEventDelegate _WinEventDelegate;

		public Overlay() {
			_WinEventDelegate = new WinEventDelegate(WinEventCallback);

			RECT rect = Screen.PrimaryScreen.Bounds;
			TransparencyKey = Color.Turquoise;
			BackColor = Color.Turquoise;
			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.Manual;
			DoubleBuffered = true;
			Location = new Point(rect.Left, rect.Top);
			Size = rect.Size;
			TopMost = true;

			SetForegroundWindow(Handle);

			Console.WriteLine("Initialized Overlay");
		}

		protected void WinEventCallback( IntPtr hWinEventHook, uint eventType, IntPtr hWnd, uint idObject, long idChild, uint dwEventThread, uint dwmsEventTime ) {
			if ( eventType == 0x800B && idObject == 0 ) {
				Invalidate();
			}
		}

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

			Pen pen = new Pen(Color.LightBlue, 3f);
			DrawSquareFrame(g, pen, 0, 0, Size.Width, Size.Height);
			pen.Dispose();
			display.DrawImage(img, ClientRectangle);
			img.Dispose();
		}

		public void DrawSquareFrame( Graphics g, Pen pen, int x1, int y1, int x2, int y2 ) {
			g.DrawLine(pen, x1, y1, x2, y1);
			g.DrawLine(pen, x1, y1, x1, y2);
			g.DrawLine(pen, x1, y1, x2, y2);
			g.DrawLine(pen, x1, y2, x2, y1);
			g.DrawLine(pen, x2, y1, x2, y2);
			g.DrawLine(pen, x1, y2, x2, y2);
		}
	}
}
