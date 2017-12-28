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
	}
}