using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Project_Cyanide {
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT {
		public int X;
		public int Y;

		public static implicit operator Point( POINT point ) {
			return new Point(point.X, point.Y);
		}
	}

	class Program {

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void mouse_event( uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo );
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow( IntPtr hWnd );
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos( out POINT lpPoint );
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect( IntPtr hWnd, ref RECT lpRect );

		private static Dictionary<String, ProcessHandler> _processHandlers;

		static void Main( string[] args ) {
			Console.WriteLine("Program started on {0:d} at {0:t}", DateTime.Now);

			_processHandlers = new Dictionary<String, ProcessHandler>();

			new Thread(() => {
				Overlay overlay = new Overlay();
				Console.WriteLine($"Started overlay thread {Thread.CurrentThread.ManagedThreadId}");
				Application.Run(overlay);
			}).Start();

			CreateProcessHandler("Crusaders of the Lost Idols");

			new Thread(() => {
				Process[] processes = Process.GetProcessesByName("Crusaders of the Lost Idols");
				while ( true ) {
					RECT rect = new RECT();
					GetWindowRect(processes[0].MainWindowHandle, ref rect);
					POINT lpPoint;
					GetCursorPos(out lpPoint);
					Console.WriteLine($"{rect.Left}|{rect.Top} ||| {lpPoint.X}|{lpPoint.Y} ||| {lpPoint.X - rect.Left}|{lpPoint.Y - rect.Top}");
					Thread.Sleep(2000);
				}
			}).Start();

			AddAction("Crusaders of the Lost Idols", "start", LocalClickActionWithReturn(510, 570));
			AddAction("Crusaders of the Lost Idols", "maximize_crusader_levels", LocalClickActionWithReturn(990, 660));
			AddAction("Crusaders of the Lost Idols", "unlock_upgrades", LocalClickActionWithReturn(990, 570));
			AddAction("Crusaders of the Lost Idols", "magnify", LocalClickActionWithReturn(410, 500));
			AddAction("Crusaders of the Lost Idols", "storm_rider", LocalClickActionWithReturn(610, 500));

			Thread.Sleep(1000);

			InvokeAction("Crusaders of the Lost Idols", "maximize_crusader_levels");
			Thread.Sleep(2000);
			InvokeAction("Crusaders of the Lost Idols", "unlock_upgrades");
			Thread.Sleep(2000);
			InvokeAction("Crusaders of the Lost Idols", "magnify");
			Thread.Sleep(2000);
			InvokeAction("Crusaders of the Lost Idols", "storm_rider");

		}
		public static void CreateProcessHandler( String name ) {
			Process[] processes = Process.GetProcessesByName(name);
			if ( processes != null && processes.Length == 1 )
				_processHandlers.Add(name, new ProcessHandler(processes[0]));
		}
		public static Boolean InvokeAction( String process, String action ) {
			if ( !_processHandlers.TryGetValue(process, out ProcessHandler processHandler) )
				return false;
			return processHandler.InvokeAction(action);
		}
		public static void AddAction( String process, String name, Action<Process> action ) {
			if ( !_processHandlers.TryGetValue(process, out ProcessHandler processHandler) )
				return;
			processHandler.AddAction(name, action);
		}

		public static Action<Process> LocalClickActionWithReturn( int x, int y) {
			return ( process ) => {
				RECT rect = new RECT();
				SetForegroundWindow(process.MainWindowHandle);
				GetWindowRect(process.MainWindowHandle, ref rect);
				POINT lpPoint;
				GetCursorPos(out lpPoint);
				mouse_event(0x8001, (uint) (65536.0 / Screen.PrimaryScreen.Bounds.Width * (rect.Left + x)), (uint) (65536.0 / Screen.PrimaryScreen.Bounds.Height * (rect.Top + y)), 0, 0);
				mouse_event(0x0006, 0, 0, 0, 0);
				mouse_event(0x8001, (uint) (65536.0 / Screen.PrimaryScreen.Bounds.Width * lpPoint.X), (uint) (65536.0 / Screen.PrimaryScreen.Bounds.Height * lpPoint.Y), 0, 0);
			};
		}
	}
}
