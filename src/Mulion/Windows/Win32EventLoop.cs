using System;
using System.Threading.Tasks;
using static Mulion.Windows.Win32;

namespace Mulion.Windows{
	class Win32EventLoop : EventLoop{
		public override Task<Window> CreateWindow(){
			return Task.FromResult<Window>(new Win32Window());
		}

		public override void PollEvents(){
			if(GetMessage(out var message, IntPtr.Zero, 0, 0) != -1){
				HandleMessage(message);
			}else{
				throw new MulionException($"Error: {GetErrorMessage(GetLastError())}");
			}
		}

		public override void RunForever(){
			while(GetMessage(out var message, IntPtr.Zero, 0, 0) != -1){
				HandleMessage(message);
			}

			throw new MulionException($"Error: {GetErrorMessage(GetLastError())}");
		}

		private void HandleMessage(Message message){
			TranslateMessage(ref message);
			DispatchMessage(ref message);
		}

		public override void Quit(){
			PostQuitMessage(0);
		}
	}
}