using System;

namespace Mulion{
	public class EventLoop{
		public event Action Quit;
		IEventLoopBackend backend;

		public EventLoop(){
			IEventLoopBackend GetBackend(){
				if(Environment.OSVersion.Platform == PlatformID.Win32NT){
					return new Windows.EventLoopBackend();
				}

				throw new MulionException("No suitable event loop backend found");
			}

			backend = GetBackend();

			backend.Quit = () => Quit?.Invoke();
		}

		public void PollEvents(){
			backend.PollEvents();
		}

		public void RunForever(){
			backend.RunForever();
		}

		public void OnQuit(){
			backend.OnQuit();
		}
	}
}