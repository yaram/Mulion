using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Mulion{
	public static class Win32{
		public static readonly int DefaultWindowPosition = unchecked((int)0x80000000);
		public static readonly uint InfiniteTimeout = 0xFFFFFFFF;

		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern uint GetLastError();

		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "FormatMessageW")]
		private static extern uint FormatMessage(uint flags, IntPtr source, uint message, uint language, out IntPtr buffer, uint size, IntPtr arguments);

		public static string GetErrorMessage(uint errorCode){
			IntPtr pointer;
			if(FormatMessage(0x00000100 | 0x00001000, IntPtr.Zero, errorCode, 0, out pointer, 0, IntPtr.Zero) == 0){
				throw new Exception("Unable to get error message string (" + GetLastError() + ")");
			}

			return errorCode + ": " + Marshal.PtrToStringUni(pointer);
		}
		
		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetModuleHandleW")]
		public static extern IntPtr GetModuleHandle(string moduleName);

		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "CreateFileW", CharSet = CharSet.Unicode)]
		public static extern IntPtr CreateFile(string path, FileAccess access, FileSharing sharing, IntPtr security, FileCreation creation, FileFlagsAttributes flagsAndAttributes, IntPtr templateFile);

		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "FindFirstChangeNotificationW", CharSet = CharSet.Unicode)]
		public static extern IntPtr WatchDirectory(string directoryPath, bool watchSubdirectories, WatchFilter filters);

		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "FindNextChangeNotification")]
		public static extern bool ConsumeDirectoryChange(IntPtr handle);

		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "ReadDirectoryChangesW")]
		private static extern bool GetDirectoryChanges(IntPtr handle, byte[] buffer, uint bufferLength, bool watchSubdirectories, WatchFilter filters, out uint bytesReturned, ref AsyncInfo asyncInfo, IntPtr asyncDoneRoutine);

		public static bool GetDirectoryChanges(IntPtr handle, byte[] buffer, bool watchSubdirectories, WatchFilter filters, out uint bytesReturned, ref AsyncInfo info){
			return GetDirectoryChanges(handle, buffer, (uint)buffer.Length, watchSubdirectories, filters, out bytesReturned, ref info, IntPtr.Zero);
		}

		[DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
		public static extern IntPtr CreateEvent(IntPtr security, bool manualReset, bool initialState, string name);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "RegisterClassExW")]
		public static extern ushort RegisterClass(ref WindowClassInfo classInfo);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern int GetSystemMetrics(SystemMetric metric);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "DefWindowProcW")]
		public static extern IntPtr DefaultWindowProcedure(IntPtr window, MessageType messageType, UIntPtr parameterA, IntPtr parameterB);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "LoadCursorW")]
		public static extern IntPtr LoadCursor(IntPtr instance, IntPtr cursorName);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "AdjustWindowRectEx")]
		public static extern bool GetWindowSizeForClientSize(ref WinRectangle rectangle, WindowStyle style, int menu, uint extendedStyle);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "EnableWindow")]
		public static extern bool SetWindowEnabled(IntPtr window, bool enabled);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern int SetWindowLong(IntPtr window, LongValue value, int newLong);
		
		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode)]
		public static extern IntPtr CreateWindow(uint extendedWindowStyle, string className, string windowName, WindowStyle windowStyle, int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr parameters);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool DestroyWindow(IntPtr window);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool ShowWindow(IntPtr window, bool show);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
		public static extern bool SetWindowTitle(IntPtr window, string title);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetMessageW")]
		public static extern int GetMessage(out Message message, IntPtr window, uint filterMin, uint filterMax);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PeekMessageW")]
		public static extern bool PeekMessage(out Message message, IntPtr window, uint filterMin, uint filterMax, uint removeMessage);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern int TranslateMessage(ref Message message);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "DispatchMessageW")]
		public static extern IntPtr DispatchMessage(ref Message message);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern void PostQuitMessage(int exitCode);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "TrackMouseEvent")]
		public static extern bool TrackMouse(ref MouseTrackingInfo info);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetClientRect")]
		public static extern bool GetClientArea(IntPtr window, out WinRectangle area);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetWindowRect")]
		public static extern bool GetWindowArea(IntPtr window, out WinRectangle area);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SetWindowPos")]
		public static extern bool SetWindowPosition(IntPtr window, IntPtr insertAfter, int x, int y, int width, int height, uint flags);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetDC")]
		public static extern IntPtr GetDeviceContext(IntPtr window);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "InvalidateRect")]
		public static extern bool InvalidateRectangle(IntPtr window, IntPtr rectangle, bool erase);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool RedrawWindow(IntPtr window, IntPtr rectangle, IntPtr region, uint flags);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern short GetKeyState(KeyCode keyCode);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		private static extern int MsgWaitForMultipleObjects(uint count, IntPtr[] handles, bool waitForAll, uint timeout, WakeMask wakeMask);

		public static int MsgWaitForMultipleObjects(IntPtr[] handles, bool waitForAll, uint timeout, WakeMask wakeMask){
			return MsgWaitForMultipleObjects((uint)handles.Length, handles, waitForAll, timeout, wakeMask);
		}

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool OpenClipboard(IntPtr window);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr GetClipboardData(uint format);

		public static string GetClipboardText(){
			return Marshal.PtrToStringUni(GetClipboardData(13));
		}

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr SetClipboardData(uint format, IntPtr data);

		public static bool SetClipboardText(string text){
			return SetClipboardData(13, Marshal.StringToHGlobalUni(text)) != IntPtr.Zero;
		}

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool CloseClipboard();

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "SetCapture")]
		public static extern IntPtr CaptureMouse(IntPtr window);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "ReleaseCapture")]
		public static extern bool ReleaseMouse();

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool ScreenToClient(IntPtr window, ref WinPoint point);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool ClientToScreen(IntPtr window, ref WinPoint point);

		[DllImport("User32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "MessageBoxW", CharSet = CharSet.Unicode)]
		public static extern int MessageBox(IntPtr window, string message, string title, MessageBoxType type);

		[DllImport("Gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern int ChoosePixelFormat(IntPtr deviceContext, ref PixelFormatDescriptor pixelFormatDescriptor);

		[DllImport("Gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool SetPixelFormat(IntPtr deviceContext, int pixelFormat, ref PixelFormatDescriptor pixelFormatDescriptor);

		[DllImport("Gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern IntPtr GetStockObject(int objectId);

		[DllImport("Gdi32.dll", CallingConvention = CallingConvention.Winapi)]
		public static extern bool SwapBuffers(IntPtr deviceContext);
		
		[DllImport("Comdlg32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetOpenFileNameW")]
		public static extern bool OpenFileDialog(ref FileDialogInfo openFileInfo);
		
		[DllImport("Comdlg32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "GetSaveFileNameW")]
		public static extern bool SaveFileDialog(ref FileDialogInfo openFileInfo);

		[DllImport("Comdlg32.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "CommDlgExtendedError")]
		public static extern uint GetDialogError();

		public static Point ParameterToPoint(IntPtr parameter){
			var value = parameter.ToInt64();

			return new Point((short)(value & 0xFFFF), (short)((value >> 16) & 0xFFFF));
		}

		public static Key VirtualKeyCodeToKey(KeyCode keyCode){
			switch(keyCode){
				case KeyCode.Escape:
					return Key.Escape;
				case KeyCode.Space:
					return Key.Space;
				case KeyCode.Left:
					return Key.Left;
				case KeyCode.Up:
					return Key.Up;
				case KeyCode.Right:
					return Key.Right;
				case KeyCode.Down:
					return Key.Down;
				case KeyCode.Delete:
					return Key.Delete;
				case KeyCode.Enter:
					return Key.Enter;
				case KeyCode.Back:
					return Key.Backspace;
				case KeyCode.End:
					return Key.End;
				case KeyCode.Home:
					return Key.Home;
				case (KeyCode)'A':
					return Key.A;
				case (KeyCode)'B':
					return Key.B;
				case (KeyCode)'C':
					return Key.C;
				case (KeyCode)'D':
					return Key.D;
				case (KeyCode)'E':
					return Key.E;
				case (KeyCode)'F':
					return Key.F;
				case (KeyCode)'G':
					return Key.G;
				case (KeyCode)'H':
					return Key.H;
				case (KeyCode)'I':
					return Key.I;
				case (KeyCode)'J':
					return Key.J;
				case (KeyCode)'K':
					return Key.K;
				case (KeyCode)'L':
					return Key.L;
				case (KeyCode)'M':
					return Key.M;
				case (KeyCode)'N':
					return Key.N;
				case (KeyCode)'O':
					return Key.O;
				case (KeyCode)'P':
					return Key.P;
				case (KeyCode)'Q':
					return Key.Q;
				case (KeyCode)'R':
					return Key.R;
				case (KeyCode)'S':
					return Key.S;
				case (KeyCode)'T':
					return Key.T;
				case (KeyCode)'U':
					return Key.U;
				case (KeyCode)'V':
					return Key.V;
				case (KeyCode)'W':
					return Key.W;
				case (KeyCode)'X':
					return Key.X;
				case (KeyCode)'Y':
					return Key.Y;
				case (KeyCode)'Z':
					return Key.Z;
				default:
					return (Key)(-1);
			}
		}

		public enum KeyCode : uint{
			Back = 0x08,
			Enter = 0x0D,
			Shift = 0x10,
			Ctrl = 0x11,
			Alt = 0x12,
			Escape = 0x1B,
			Space = 0x20,
			End = 0x23,
			Home = 0x24,
			Left = 0x25,
			Up = 0x26,
			Right = 0x27,
			Down = 0x28,
			Delete = 0x2E,
		}

		[Flags]
		public enum WatchFilter : uint{
			File = 0x00000001,
			Directory = 0x00000002,
			Attribute = 0x00000004,
			FileSize = 0x00000008,
			LastWrite = 0x00000010,
			Security = 0x00000100,
			All = File | Directory | Attribute | FileSize | LastWrite | Security
		}

		[Flags]
		public enum WakeMask : uint{
			All = 0x04FF
		}

		[Flags]
		public enum WindowStyle : uint{
			MaximizeBox = 0x00010000,
			MinimizeBox = 0x00020000,
			Resizable = 0x00040000,
			SystemMenu = 0x00080000,
			TitleBar = 0x00C00000,
			StartMaximized = 0x01000000,
			Visible = 0x10000000,
			StartMinimized = 0x20000000,
			Child = 0x40000000,
			PopUp = 0x80000000,
		}

		[Flags]
		public enum WindowClassStyle : uint{
			VerticalRedraw = 0x0001,
			HorizontalRedraw = 0x0002,
			PrivateDeviceContext = 0x0020
		}

		public enum MessageType : uint{
			Destroy = 0x0002,
			Move = 0x0003,
			Resize = 0x0005,
			Paint = 0x000F,
			Close = 0x0010,
			Quit = 0x0012,
			KeyDown = 0x0100,
			KeyUp = 0x0101,
			Character = 0x0102,
			MouseMove = 0x0200,
			MouseLeftDown = 0x0201,
			MouseLeftUp = 0x0202,
			MouseRightDown = 0x0204,
			MouseRightUp = 0x0205,
			MouseMiddleDown = 0x0207,
			MouseMiddleUp = 0x0208,
			MouseWheel = 0x020A,
			MouseLeave = 0x02A3,
		}

		[Flags]
		public enum MouseTrackingFlags : uint{
			Hover = 0x00000001,
			Leave = 0x00000002,
			NonClient = 0x00000010,
			Query = 0x40000000,
			Cancel = 0x80000000
		}

		[Flags]
		public enum FileAccess : uint{
			Read = 0x00001,
			Write = 0x0002,
		}

		[Flags]
		public enum FileSharing : uint{
			Exclusive = 0x00000000,
			AllowRead = 0x00000001,
			AllowWrite = 0x00000002,
			AllowDelete = 0x00000004
		}
		
		public enum FileCreation : uint{
			CreateNew = 1,
			AlwaysCreate = 2,
			OpenExisting = 3,
			AlwaysOpen = 4,
			TruncateExisting = 5
		}
		
		public enum MessageBoxType : uint{
			Ok = 0
		}

		[Flags]
		public enum FileFlagsAttributes : uint{
			Normal = 0x00000080,

			BackupSemantics = 0x02000000,
			Asynchronous = 0x40000000
		}

		public enum LongValue : int{
			Style = -16
		}

		public enum SystemMetric : int{
			ScreenWidth = 0,
			ScreenHeight = 1,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WinPoint{
			public int X;
			public int Y;

			public static explicit operator Point(WinPoint point){
				return new Point(point.X, point.Y);
			}

			public static explicit operator WinPoint(Point point){
				return new WinPoint{
					X = point.X,
					Y = point.Y
				};
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WinRectangle{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public static explicit operator Rectangle(WinRectangle rect){
				return Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
			}

			public static explicit operator WinRectangle(Rectangle rect){
				return new WinRectangle{
					Left = rect.Left,
					Top = rect.Top,
					Right = rect.Right,
					Bottom = rect.Bottom
				};
			}
		}
	
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WindowClassInfo{
			[UnmanagedFunctionPointer(CallingConvention.Winapi)]
			public delegate IntPtr WindowProcedure(IntPtr window, MessageType messageType, UIntPtr parameterA, IntPtr parameterB);

			public uint Size;
			public WindowClassStyle Style;
			public WindowProcedure Procedure;
			public int ExtraClassBytes;
			public int ExtraWindowBytes;
			public IntPtr Instance;
			public IntPtr Icon;
			public IntPtr Cursor;
			public IntPtr Background;
			public IntPtr MenuName;
			public string ClassName;
			public IntPtr SmallIcon;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Message{
			public IntPtr Window;
			public MessageType Type;
			public UIntPtr WParameter;
			public IntPtr LParameter;
			public uint Time;
			public int X;
			public int Y;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MouseTrackingInfo{
			public uint Size;
			public MouseTrackingFlags Flags;
			public IntPtr Window;
			public uint HoverTime;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct PixelFormatDescriptor{
			public ushort Size;
			public ushort Version;
			public uint Flags;
			public byte PixelType;
			public byte ColorBits;
			public byte RedBits;
			public byte RedShift;
			public byte GreenBits;
			public byte GreenShift;
			public byte BlueBits;
			public byte BlueShift;
			public byte AlphaBits;
			public byte AlphaShift;
			public byte AccumulationBits;
			public byte AccumulationRedBits;
			public byte AccumulationGreenBits;
			public byte AccumulationBlueBits;
			public byte AccumulationAlphaBits;
			public byte DepthBits;
			public byte StencilBits;
			public byte AuxiliaryBuffers;
			public byte LayerType;
			public byte Reserved;
			public uint LayerMask;
			public uint VisibleMask;
			public uint DamageMask;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct FileDialogInfo{
			public uint Size;
			public IntPtr OwningWindow;
			public IntPtr Instance;
			public string Filter;
			public string CustomFilter;
			public uint MaxCustomFilter;
			public uint FilterIndex;
			public string FilePath;
			public uint MaxFilePathLength;
			public string FileName;
			public uint MaxFileNameLength;
			public string InitialDirectory;
			public string DialogTitle;
			public uint Flags;
			public ushort FileOffset;
			public ushort FileExtension;
			public string DefaultExtension;
			public IntPtr CustomData;
			public IntPtr Hook;
			public string TemplateName;
			public IntPtr Reserved;
			public uint Reserved2;
			public uint ExtendedFlags;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct AsyncInfo{
			public UIntPtr Internal;
			public UIntPtr InternalHigh;
			public uint Offset;
			public uint OffsetHigh;
			public IntPtr Event;
		}
	}
}