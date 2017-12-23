using System;
using System.Drawing;
using Mulion;

namespace BasicExample{
	class Program{
		static void Main(){
			var eventLoop = new EventLoop();

			eventLoop.Quit += () => Environment.Exit(0);

			var window = new Window{
				Title = "Derp",
				Size = new Size(640, 480)
			};

			window.Close += eventLoop.OnQuit;

			window.Visible = true;

			eventLoop.RunForever();
		}
	}
}