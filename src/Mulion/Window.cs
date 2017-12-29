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

			backend.Move = () => Move?.Invoke();
			backend.Resize = () => Resize?.Invoke();
			backend.Close = () => Close?.Invoke();

			backend.MouseDown = (button) => MouseDown?.Invoke(button);
			backend.MouseUp = (button) => MouseUp?.Invoke(button);

			backend.KeyDown = (key) => KeyDown?.Invoke(key);
			backend.KeyUp = (key) => KeyUp?.Invoke(key);

			backend.MouseMove = (old, @new) => MouseMove(old, @new);
		}
	}
}