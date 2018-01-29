using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Mulion{
	public abstract class Window{
		public event Action Move;
		public event Action Resize;
		public event Action Close;
		public event Action<int> MouseDown;
		public event Action<int> MouseUp;
		public event Action<Key> KeyDown;
		public event Action<Key> KeyUp;
		public event Action<Point, Point> MouseMove;
		
		public abstract string Title{get;}

		public abstract bool Enabled{get;}
		public abstract bool Visible{get;}
		public abstract bool Closed{get;}
		public abstract bool Fullscreen{get;}
		public abstract bool Resizable{get;}

		public abstract Rectangle Bounds{get;}
		public abstract Point Location{get;}
		public abstract Size Size{get;}

		public abstract Task SetTitle(string title);

		public abstract Task SetEnabled(bool enabled);
		public abstract Task SetVisible(bool visible);
		public abstract Task SetClosed(bool closed);
		public abstract Task SetFullscreen(bool fullscreen);
		public abstract Task SetResizable(bool resizable);

		public abstract Task SetBounds(Rectangle bounds);
		public abstract Task SetLocation(Point location);
		public abstract Task SetSize(Size size);

		protected void OnMove(){
			Move?.Invoke();
		}

		protected void OnResize(){
			Resize?.Invoke();
		}

		protected void OnClose(){
			Close?.Invoke();
		}

		protected void OnMouseDown(int button){
			MouseDown?.Invoke(button);
		}

		protected void OnMouseUp(int button){
			MouseUp?.Invoke(button);
		}

		protected void OnKeyDown(Key key){
			KeyDown?.Invoke(key);
		}

		protected void OnKeyUp(Key key){
			KeyUp?.Invoke(key);
		}

		protected void OnMouseMove(Point old, Point @new){
			MouseMove?.Invoke(old, @new);
		}
	}
}