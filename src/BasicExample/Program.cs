using System;
using System.Drawing;
using Mulion;

namespace BasicExample{
	class Program{
		static void Main(){
			var eventLoop = EventLoop.Create().Result;

			eventLoop.Quit += () => Environment.Exit(0);

			var window = eventLoop.CreateWindow().Result;
			window.SetTitle("Test");
			window.SetSize(new Size(640, 480));

			window.Close += eventLoop.PostQuit;

			window.SetVisible(true).Wait();

			eventLoop.RunForever();
		}
	}
}