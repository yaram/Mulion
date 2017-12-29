using System;
using System.Drawing;
using System.Runtime.InteropServices;
using static Mulion.Win32;

namespace Mulion.Windows{
	public class WindowBackend : IWindowBackend, IDisposable{
		private static int windowCount;
		
		Point lastKnownMouseLocation;
		bool visible;
		public bool Visible{
			get => visible;

			set{
				visible = value;

				ShowWindow(handle, value);
			}
		}
		IntPtr handle;
		IntPtr instance;
		public IntPtr NativeHandle => handle;
		bool closed;
		public bool Closed{
			get => closed;

			set{
				closed = value;

				if(closed == true){
					DestroyWindow(handle);
				}
			}
		} 
		WindowStyle windowStyle;
		Rectangle bounds;
		public Rectangle Bounds{
			get => bounds;

			set{
				bounds = value;
				
				var newClientArea = (WinRectangle)bounds;

				if(!GetWindowSizeForClientSize(ref newClientArea, windowStyle, 0, 0)){
					throw new MulionException($"Unable to determine correct window size for desired client area size ({GetErrorMessage(GetLastError())})");
				}

				if(!SetWindowPosition(handle, IntPtr.Zero, newClientArea.Left, newClientArea.Top, newClientArea.Right - newClientArea.Left, newClientArea.Bottom - newClientArea.Top, 0)){
					throw new MulionException($"Unable to set window position ({GetErrorMessage(GetLastError())})");
				}
			}
		}
		bool enabled;
		public bool Enabled{
			get => enabled;

			set{
				enabled = value;
				
				if(!SetWindowEnabled(handle, enabled)){
					throw new MulionException($"Unable to set window enabled state ({GetErrorMessage(GetLastError())})");
				}
			}
		}
		string title;
		public string Title{
			get => title;

			set{
				title = value;
				
				if(!SetWindowTitle(handle, title)){
					throw new MulionException($"Unable to set window title ({GetErrorMessage(GetLastError())})");
				}
			}
		}
		bool fullscreen;
		Rectangle boundsBeforeFullscreen;
		public bool Fullscreen{
			get => fullscreen;

			set{
				if(value != fullscreen){
					fullscreen = value;

					GenerateStyle();

					if(fullscreen){
						boundsBeforeFullscreen = Bounds;
					}

					if(SetWindowLong(handle, LongValue.Style, (int)windowStyle) == 0){
						throw new MulionException($"Unable to set window fullscreen state ({GetErrorMessage(GetLastError())})");
					}

					if(fullscreen){
						Bounds = new Rectangle(0, 0, GetSystemMetrics(SystemMetric.ScreenWidth), GetSystemMetrics(SystemMetric.ScreenHeight));
					}else{
						Bounds = boundsBeforeFullscreen;
					}
				}
			}
		}
		bool resizable;
		public bool Resizable{
			get => resizable;

			set{
				resizable = value;

				GenerateStyle();

				if(SetWindowLong(handle, LongValue.Style, (int)windowStyle) == 0){
					throw new MulionException($"Unable to set window resizable state ({GetErrorMessage(GetLastError())})");
				}
			}
		}
		
		public Action Move{get; set;}
		public Action Resize{get; set;}
		public Action Close{get; set;}
		public Action<int> MouseDown{get; set;}
		public Action<int> MouseUp{get; set;}
		public Action<Key> KeyDown{get; set;}
		public Action<Key> KeyUp{get; set;}
		public Action<Point, Point> MouseMove{get; set;}

		public WindowBackend(){
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

			GenerateStyle();
			
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
					Close?.Invoke();
					return IntPtr.Zero;

				case MessageType.Move:
					bounds.Location = ParameterToPoint(parameterB);
					Move?.Invoke();
					break;

				case MessageType.Resize:
					bounds.Size = (Size)ParameterToPoint(parameterB);
					Resize?.Invoke();
					break;

				case MessageType.MouseLeftDown:
					MouseDown?.Invoke(0);
					break;

				case MessageType.MouseRightDown:
					MouseDown?.Invoke(1);
					break;

				case MessageType.MouseMiddleDown:
					MouseDown?.Invoke(2);
					break;

				case MessageType.MouseLeftUp:
					MouseUp?.Invoke(0);
					break;

				case MessageType.MouseRightUp:
					MouseUp?.Invoke(1);
					break;

				case MessageType.MouseMiddleUp:
					MouseUp?.Invoke(2);
					break;

				case MessageType.KeyDown:
					KeyDown?.Invoke(VirtualKeyCodeToKey((KeyCode)(uint)parameterA));
					break;

				case MessageType.KeyUp:
					KeyUp?.Invoke(VirtualKeyCodeToKey((KeyCode)(uint)parameterA));
					break;

				case MessageType.Close:
					Close?.Invoke();
					break;

				case MessageType.MouseMove:
					{
						var newLocation = ParameterToPoint(parameterB);
						MouseMove?.Invoke(lastKnownMouseLocation, newLocation);
						lastKnownMouseLocation = newLocation;
					}
					break;
			}

			return DefaultWindowProcedure(window, messageType, parameterA, parameterB);
		}

		public void Dispose(){
			DestroyWindow(handle);
		}

		private void GenerateStyle(){
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
	}
}