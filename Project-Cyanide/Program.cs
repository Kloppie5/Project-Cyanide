using System;
using System.Windows.Forms;

namespace Project_Cyanide {
	class Program {
		static void Main( string[] args ) {
			Console.WriteLine("Program started on {0:d} at {0:t}", DateTime.Now);
			Overlay overlay = new Overlay();
			Application.Run(overlay);
		}
	}
}
