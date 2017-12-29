using System;
using System.Drawing;

namespace Mulion{
	interface IWindowBackend{
		IntPtr NativeHandle{get;}

		string Title{get; set;}

		bool Enabled{get; set;}
		bool Visible{get; set;}
		bool Closed{get; set;}
		bool Fullscreen{get; set;}
		bool Resizable{get; set;}

		Rectangle Bounds{get; set;}
		
		Action Move{set;}
		Action Resize{set;}
		Action Close{set;}
		Action<int> MouseDown{set;}
		Action<int> MouseUp{set;}
		Action<Key> KeyDown{set;}
		Action<Key> KeyUp{set;}
		Action<Point, Point> MouseMove{set;}
	}
}