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
		
		Action Move{get; set;}
		Action Resize{get; set;}
		Action Close{get; set;}
		Action<int> MouseDown{get; set;}
		Action<int> MouseUp{get; set;}
		Action<Key> KeyDown{get; set;}
		Action<Key> KeyUp{get; set;}
		Action<Point, Point> MouseMove{get; set;}
	}
}