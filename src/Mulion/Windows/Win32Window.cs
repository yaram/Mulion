using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static Mulion.Windows.Win32;

namespace Mulion.Windows{
	public class Win32Window : Window{
		private static int windowCount;
		
		Point lastKnownMouseLocation;

		bool visible;
		public override bool Visible => visible;

		IntPtr handle;
		IntPtr instance;
		public IntPtr NativeHandle => handle;

		bool closed;
		public override bool Closed => closed;

		WindowStyle windowStyle;
		Rectangle bounds;
		public override Rectangle Bounds => bounds;

		bool enabled;
		public override bool Enabled => enabled;

		string title;
		public override string Title => title;

		bool fullscreen;
		Rectangle boundsBeforeFullscreen;
		public override bool Fullscreen => fullscreen;

		bool resizable;
		public override bool Resizable => resizable;

		public override Point Location => Bounds.Location;

		public override Size Size => Bounds.Size;

		public Win32Window(){
			instance = GetModuleHandle(null);
			enabled = true;

			var classStyle = WindowClassStyle.HorizontalRedraw | WindowClassStyle.VerticalRedraw | WindowClassStyle.PrivateDeviceContext;

			windowCount++;
			var className = "Window" + windowCount;

			MainWindowProcedureDelegate = MainWindowProcedure;

			var info = new WindowClassInfo(){
				Size = (uint)Marshal.SizeOf<WindowClassInfo>(),
				Style = classStyle,
				Procedure = MainWindowProcedureDelegate,
				Instance = instance,
				Cursor = LoadCursor(IntPtr.Zero, new IntPtr(32512)),
				ClassName = className
			};

			var atom = RegisterClass(ref info);

			if(atom == 0){
				throw new MulionException($"Unable to create window class ({GetErrorMessage(GetLastError())})");
			}

			RegenerateStyle();
			
			handle = CreateWindow(0, className, title, windowStyle, DefaultWindowPosition, DefaultWindowPosition, 0, 0, IntPtr.Zero, IntPtr.Zero, instance, IntPtr.Zero);

			if(handle == IntPtr.Zero){
				throw new MulionException($"Unable to create window ({GetErrorMessage(GetLastError())})");
			}

			var topLeft = new WinPoint();
			if(!ClientToScreen(handle, ref topLeft)){
				throw new MulionException($"Unable to retrieve client are for window ({GetErrorMessage(GetLastError())})");
			}

			var bottomRight = (WinPoint)(Point)Bounds.Size;
			if(!ClientToScreen(handle, ref bottomRight)){
				throw new MulionException($"Unable to retrieve client are for window ({GetErrorMessage(GetLastError())})");
			}

			bounds = new Rectangle((Point)topLeft, (Size)(Point)bottomRight - (Size)(Point)topLeft);
		}

		private WindowClassInfo.WindowProcedure MainWindowProcedureDelegate;
		private IntPtr MainWindowProcedure(IntPtr window, MessageType messageType, UIntPtr parameterA, IntPtr parameterB){
			switch(messageType){
				case MessageType.Destroy:
					closed = true;
					OnClose();
					return IntPtr.Zero;

				case MessageType.Move:
					bounds.Location = ParameterToPoint(parameterB);
					OnMove();
					break;

				case MessageType.Resize:
					bounds.Size = (Size)ParameterToPoint(parameterB);
					OnResize();
					break;

				case MessageType.MouseLeftDown:
					OnMouseDown(0);
					break;

				case MessageType.MouseRightDown:
					OnMouseDown(1);
					break;

				case MessageType.MouseMiddleDown:
					OnMouseDown(2);
					break;

				case MessageType.MouseLeftUp:
					OnMouseUp(0);
					break;

				case MessageType.MouseRightUp:
					OnMouseUp(1);
					break;

				case MessageType.MouseMiddleUp:
					OnMouseUp(2);
					break;

				case MessageType.KeyDown:
					OnKeyDown(VirtualKeyCodeToKey((KeyCode)(uint)parameterA));
					break;

				case MessageType.KeyUp:
					OnKeyUp(VirtualKeyCodeToKey((KeyCode)(uint)parameterA));
					break;

				case MessageType.Close:
					OnClose();
					break;

				case MessageType.MouseMove:
					{
						var newLocation = ParameterToPoint(parameterB);
						OnMouseMove(lastKnownMouseLocation, newLocation);
						lastKnownMouseLocation = newLocation;
					}
					break;
			}

			return DefaultWindowProcedure(window, messageType, parameterA, parameterB);
		}

		public void Dispose(){
			DestroyWindow(handle);
		}

		private void RegenerateStyle(){
			windowStyle = WindowStyle.PopUp | WindowStyle.MinimizeBox;

			if(!fullscreen){
				windowStyle |= WindowStyle.SystemMenu | WindowStyle.TitleBar;
			}

			if(resizable){
				windowStyle |= WindowStyle.MaximizeBox | WindowStyle.Resizable;
			}

			if(visible){
				windowStyle |= WindowStyle.Visible;
			}
		}

		public override Task SetTitle(string title){
			if(!SetWindowTitle(handle, title)){
				throw new MulionException($"Unable to set window title ({GetErrorMessage(GetLastError())})");
			}

			this.title = title;

			return Task.CompletedTask;
		}

		public override Task SetEnabled(bool enabled){
			if(!SetWindowEnabled(handle, enabled)){
				throw new MulionException($"Unable to set window enabled state ({GetErrorMessage(GetLastError())})");
			}

			this.enabled = enabled;

			return Task.CompletedTask;
		}

		public override Task SetVisible(bool visible){
			if(!ShowWindow(handle, visible)){
				throw new MulionException($"Unable to set window visible state ({GetErrorMessage(GetLastError())})");
			}

			this.visible = visible;

			return Task.CompletedTask;
		}

		public override Task SetClosed(bool closed){
			if(closed == true){
				DestroyWindow(handle);
			}

			this.closed = closed;

			return Task.CompletedTask;
		}

		public override Task SetFullscreen(bool fullscreen){
			if(fullscreen != this.fullscreen){
				RegenerateStyle();

				if(fullscreen){
					boundsBeforeFullscreen = Bounds;
				}

				if(SetWindowLong(handle, LongValue.Style, (int)windowStyle) == 0){
					throw new MulionException($"Unable to set window fullscreen state ({GetErrorMessage(GetLastError())})");
				}

				if(fullscreen){
					bounds = new Rectangle(0, 0, GetSystemMetrics(SystemMetric.ScreenWidth), GetSystemMetrics(SystemMetric.ScreenHeight));
				}else{
					bounds = boundsBeforeFullscreen;
				}

				this.fullscreen = fullscreen;
			}

			return Task.CompletedTask;
		}

		public override Task SetResizable(bool resizable){
			RegenerateStyle();

			if(SetWindowLong(handle, LongValue.Style, (int)windowStyle) == 0){
				throw new MulionException($"Unable to set window resizable state ({GetErrorMessage(GetLastError())})");
			}

			this.resizable = resizable;

			return Task.CompletedTask;
		}

		public override Task SetBounds(Rectangle bounds){
			var newClientArea = (WinRectangle)bounds;

			if(!GetWindowSizeForClientSize(ref newClientArea, windowStyle, 0, 0)){
				throw new MulionException($"Unable to determine correct window size for desired client area size ({GetErrorMessage(GetLastError())})");
			}

			if(!SetWindowPosition(handle, IntPtr.Zero, newClientArea.Left, newClientArea.Top, newClientArea.Right - newClientArea.Left, newClientArea.Bottom - newClientArea.Top, 0)){
				throw new MulionException($"Unable to set window position ({GetErrorMessage(GetLastError())})");
			}

			this.bounds = bounds;

			return Task.CompletedTask;
		}

		public override Task SetLocation(Point location){
			return SetBounds(new Rectangle(location, Bounds.Size));
		}

		public override Task SetSize(Size size){
			return SetBounds(new Rectangle(Bounds.Location, size));
		}
	}
}