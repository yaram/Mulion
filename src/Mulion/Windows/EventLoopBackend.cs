using System;
using static Mulion.Windows.Win32;

namespace Mulion.Windows{
	class EventLoopBackend : IEventLoopBackend{
		public Action Quit{private get; set;}

		public void PollEvents(){
			if(GetMessage(out var message, IntPtr.Zero, 0, 0) != -1){
				HandleMessage(message);
			}else{
				throw new MulionException($"Error: {GetErrorMessage(GetLastError())}");
			}
		}

		public void RunForever(){
			while(GetMessage(out var message, IntPtr.Zero, 0, 0) != -1){
				HandleMessage(message);
			}

			throw new MulionException($"Error: {GetErrorMessage(GetLastError())}");
		}

		private void HandleMessage(Message message){
			TranslateMessage(ref message);
			DispatchMessage(ref message);

			switch(message.Type){
				case MessageType.Quit:
					Quit?.Invoke();
					break;
			}
		}

		public void OnQuit(){
			PostQuitMessage(0);
		}
	}
}