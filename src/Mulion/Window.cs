using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static Mulion.Win32;

namespace Mulion{
	public class Window{
		IWindowBackend backend;
		
		public event Action Move;
		public event Action Resize;
		public event Action Close;
		public event Action<int> MouseDown;
		public event Action<int> MouseUp;
		public event Action<Key> KeyDown;
		public event Action<Key> KeyUp;
		public event Action<Point, Point> MouseMove;

		IntPtr NativeHandle => backend.NativeHandle;
		
		public string Title{
			get => backend.Title;

			set => backend.Title = value;
		}

		public bool Enabled{
			get => backend.Enabled;

			set => backend.Enabled = value;
		}
		public bool Visible{
			get => backend.Visible;

			set => backend.Visible = value;
		}
		public bool Closed{
			get => backend.Closed;

			set => backend.Closed = value;
		}
		public bool Fullscreen{
			get => backend.Fullscreen;

			set => backend.Fullscreen = value;
		}
		public bool Resizable{
			get => backend.Resizable;

			set => backend.Resizable = value;
		}

		public Rectangle Bounds{
			get => backend.Bounds;

			set => backend.Bounds = value;
		}
		public Point Location{
			get => Bounds.Location;

			set => Bounds = new Rectangle(value, Bounds.Size);
		}
		public Size Size{
			get => Bounds.Size;

			set => Bounds = new Rectangle(Bounds.Location, value);
		}

		public Window(){
			if(/* Window backend checks */){
				
			}else{
				throw new MulionException("No suitable window backend found");
			}
		}

		void OnMove(){
			Move?.Invoke();
		}

		void OnResize(){
			Resize?.Invoke();
		}

		void OnClose(){
			Close?.Invoke();
		}

		void OnMouseDown(int button){
			MouseDown?.Invoke(button);
		}

		void OnMouseUp(int button){
			MouseUp?.Invoke(button);
		}

		void OnKeyDown(Key key){
			KeyDown?.Invoke(key);
		}

		void OnKeyUp(Key key){
			KeyUp?.Invoke(key);
		}

		void OnMouseMove(Point old, Point @new){
			MouseMove?.Invoke(old, @new);
		}
	}
}